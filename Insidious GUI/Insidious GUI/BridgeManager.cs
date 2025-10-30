using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Insidious_GUI
{
    /// <summary>
    /// Message types matching Python MessageType enum
    /// </summary>
    public enum MessageType
    {
        CMD,
        RESP,
        EVENT,
        ERR
    }

    /// <summary>
    /// Structured message format
    /// </summary>
    public class Message
    {
        public string type { get; set; }
        public string module { get; set; }
        public string action { get; set; }
        public object data { get; set; }
        public string msg_id { get; set; }

        // Parameterless constructor for deserialization
        public Message()
        {
        }

        // Constructor for creating new messages
        public Message(MessageType msgType, string module, string action, object data = null)
        {
            this.type = msgType.ToString();
            this.module = module;
            this.action = action;
            this.data = data;
            this.msg_id = Guid.NewGuid().ToString().Substring(0, 8);
        }
    }

    /// <summary>
    /// Bridge communication manager - handles all Python bridge communication
    /// </summary>
    public class BridgeManager
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        private bool isRunning = false;
        private StringBuilder receiveBuffer = new StringBuilder();

        // Event handlers for different message types
        public event EventHandler<MessageReceivedEventArgs> ResponseReceived;
        public event EventHandler<MessageReceivedEventArgs> EventReceived;
        public event EventHandler<MessageReceivedEventArgs> ErrorReceived;
        public event EventHandler<MessageSentEventArgs> CommandSent;

        // Pending response handlers (for request-response pattern)
        private Dictionary<string, TaskCompletionSource<Message>> pendingResponses = 
            new Dictionary<string, TaskCompletionSource<Message>>();

        public bool IsConnected => client?.Connected ?? false;

        /// <summary>
        /// Connect to the Python bridge
        /// </summary>
        public async Task<bool> ConnectAsync(string host = "127.0.0.1", int port = 65535, int retries = 10)
        {
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    client = new TcpClient();
                    await client.ConnectAsync(host, port);
                    stream = client.GetStream();
                    
                    // Start receive thread
                    isRunning = true;
                    receiveThread = new Thread(ReceiveLoop);
                    receiveThread.IsBackground = true;
                    receiveThread.Start();

                    Console.WriteLine($"Connected to bridge at {host}:{port}");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection attempt {i + 1} failed: {ex.Message}");
                    await Task.Delay(1000);
                }
            }

            return false;
        }

        /// <summary>
        /// Send a command and optionally wait for response
        /// </summary>
        public async Task<Message> SendCommandAsync(string module, string action, object data = null, bool waitForResponse = false)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to bridge");

            var message = new Message(MessageType.CMD, module, action, data);
            
            TaskCompletionSource<Message> responseTask = null;
            
            if (waitForResponse)
            {
                responseTask = new TaskCompletionSource<Message>();
                lock (pendingResponses)
                {
                    pendingResponses[message.msg_id] = responseTask;
                }
            }

            // Serialize and send
            string json = JsonSerializer.Serialize(message);
            byte[] buffer = Encoding.UTF8.GetBytes(json + "\n");
            
            await stream.WriteAsync(buffer, 0, buffer.Length);
            await stream.FlushAsync();

            // Fire command sent event
            CommandSent?.Invoke(this, new MessageSentEventArgs(message));

            System.Diagnostics.Debug.WriteLine($"Sent: {module}.{action}");

            // Wait for response if requested
            if (waitForResponse)
            {
                // Timeout after 30 seconds
                var timeoutTask = Task.Delay(30000);
                var completedTask = await Task.WhenAny(responseTask.Task, timeoutTask);
                
                lock (pendingResponses)
                {
                    pendingResponses.Remove(message.msg_id);
                }

                if (completedTask == timeoutTask)
                {
                    throw new TimeoutException($"No response received for {module}.{action}");
                }

                return await responseTask.Task;
            }

            return null;
        }

        /// <summary>
        /// Continuous receive loop
        /// </summary>
        private void ReceiveLoop()
        {
            byte[] buffer = new byte[4096];

            while (isRunning && IsConnected)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Connection closed by server");
                        break;
                    }

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    receiveBuffer.Append(data);

                    // Process complete messages (newline delimited)
                    ProcessReceivedData();
                }
                catch (Exception ex)
                {
                    if (isRunning)
                    {
                        Console.WriteLine($"Receive error: {ex.Message}");
                    }
                    break;
                }
            }

            isRunning = false;
        }

        /// <summary>
        /// Process received data and extract complete messages
        /// </summary>
        private void ProcessReceivedData()
        {
            string bufferContent = receiveBuffer.ToString();
            int newlineIndex;

            while ((newlineIndex = bufferContent.IndexOf('\n')) != -1)
            {
                string messageJson = bufferContent.Substring(0, newlineIndex).Trim();
                bufferContent = bufferContent.Substring(newlineIndex + 1);

                if (!string.IsNullOrEmpty(messageJson))
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"[Bridge] Raw JSON: {messageJson}");
                        
                        var message = JsonSerializer.Deserialize<Message>(messageJson);
                        
                        System.Diagnostics.Debug.WriteLine($"[Bridge] Parsed: module={message.module}, action={message.action}, type={message.type}");
                        System.Diagnostics.Debug.WriteLine($"[Bridge] Data type: {message.data?.GetType()}");
                        System.Diagnostics.Debug.WriteLine($"[Bridge] Data value: {message.data}");
                        
                        HandleReceivedMessage(message);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Bridge] Parse error: {ex.Message}");
                        Console.WriteLine($"Failed to parse message: {ex.Message}");
                    }
                }
            }

            receiveBuffer.Clear();
            receiveBuffer.Append(bufferContent);
        }

        /// <summary>
        /// Handle a received message
        /// </summary>
        private void HandleReceivedMessage(Message message)
        {
            System.Diagnostics.Debug.WriteLine($"Received: {message.module}.{message.action} ({message.type})");

            // Check if this is a response to a pending request
            if (message.type == "RESP" && !string.IsNullOrEmpty(message.msg_id))
            {
                lock (pendingResponses)
                {
                    if (pendingResponses.TryGetValue(message.msg_id, out var tcs))
                    {
                        tcs.SetResult(message);
                        pendingResponses.Remove(message.msg_id);
                        return; // Don't fire event for handled responses
                    }
                }
            }

            // Fire appropriate event
            var eventArgs = new MessageReceivedEventArgs(message);

            switch (message.type)
            {
                case "RESP":
                    ResponseReceived?.Invoke(this, eventArgs);
                    break;
                case "EVENT":
                    EventReceived?.Invoke(this, eventArgs);
                    break;
                case "ERR":
                    ErrorReceived?.Invoke(this, eventArgs);
                    break;
            }
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void Disconnect()
        {
            isRunning = false;
            
            try
            {
                stream?.Close();
                client?.Close();
            }
            catch { }

            Console.WriteLine("Disconnected from bridge");
        }
    }

    /// <summary>
    /// Event args for message received events
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        public Message Message { get; }

        public MessageReceivedEventArgs(Message message)
        {
            Message = message;
        }
    }

    /// <summary>
    /// Event args for message sent events
    /// </summary>
    public class MessageSentEventArgs : EventArgs
    {
        public Message Message { get; }

        public MessageSentEventArgs(Message message)
        {
            Message = message;
        }
    }
}
