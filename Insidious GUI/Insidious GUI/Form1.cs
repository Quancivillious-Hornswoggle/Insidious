using Insidious_GUI.ModuleWindows;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Insidious_GUI
{
    public partial class Form1 : Form
    {
        // Global bridge manager - accessible from all windows
        public static BridgeManager Bridge { get; private set; }

        // Windows
        Deauth deauthWindow = null;
        Mitm mitmWindow = null;
        DNS dns = null;
        Server server = null; 

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Make console read-only and enable scrolling
            consoleTextBox.ReadOnly = true;
            consoleTextBox.ScrollBars = ScrollBars.Vertical;
            consoleTextBox.WordWrap = true;
            
            // Initialize bridge manager
            LogToConsole("[System] Initializing Bridge Manager...");
            Bridge = new BridgeManager();

            // Subscribe to global events
            Bridge.EventReceived += Bridge_EventReceived;
            Bridge.ErrorReceived += Bridge_ErrorReceived;
            Bridge.ResponseReceived += Bridge_ResponseReceived;
            Bridge.CommandSent += Bridge_CommandSent;

            // Try to connect
            LogToConsole("[System] Connecting to Python bridge...");
            
            bool connected = await Bridge.ConnectAsync("127.0.0.1", 65535, retries: 10);
            
            if (connected)
            {
                LogToConsole("[System] ✓ Connected successfully!");
                this.Text = "Insidious - Connected";
            }
            else
            {
                LogToConsole("[System] ✗ Connection failed!");
                this.Text = "Insidious - Connection Failed";
                MessageBox.Show(
                    "Failed to connect to Python bridge. Make sure python_bridge.py is running.",
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Bridge_CommandSent(object sender, MessageSentEventArgs e)
        {
            LogToConsole($"[→ CMD] {e.Message.module}.{e.Message.action}");
        }

        private void Bridge_ResponseReceived(object sender, MessageReceivedEventArgs e)
        {
            LogToConsole($"[← RESP] {e.Message.module}.{e.Message.action}");
        }

        private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
        {
            LogToConsole($"[← EVENT] {e.Message.module}.{e.Message.action}");
        }

        private void Bridge_ErrorReceived(object sender, MessageReceivedEventArgs e)
        {
            LogToConsole($"[← ERROR] {e.Message.module}.{e.Message.action}");
            
            // Show error to user
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowError(e.Message)));
            }
            else
            {
                ShowError(e.Message);
            }
        }

        private void LogToConsole(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => LogToConsole(message)));
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            consoleTextBox.AppendText($"[{timestamp}] {message}\r\n");
            
            // Auto-scroll to bottom
            consoleTextBox.SelectionStart = consoleTextBox.Text.Length;
            consoleTextBox.ScrollToCaret();
        }

        private void ShowError(Message message)
        {
            string errorMsg = "Unknown error";
            
            if (message.data != null)
            {
                // Try to extract error message from data
                var dataStr = message.data.ToString();
                errorMsg = dataStr;
            }

            MessageBox.Show(
                $"Error in {message.module}: {errorMsg}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private void wifiBruteButton_Click(object sender, EventArgs e)
        {
            // TODO: Implement wifi brute window
            MessageBox.Show("WiFi Brute module not yet implemented", "Info");
        }

        private void wifiDeauthButton_Click(object sender, EventArgs e)
        {
            if (deauthWindow == null || deauthWindow.IsDisposed)
            {
                deauthWindow = new Deauth();
                deauthWindow.Show();
            }
            else
            {
                deauthWindow.BringToFront();
            }
        }

        private void dnsSpoofButton_Click(object sender, EventArgs e)
        {
            if (dns == null || dns.IsDisposed)
            {
                dns = new DNS();
                dns.Show();
            }
            else
            {
                dns.BringToFront();
            }
        }

        private void serverHostButton_Click(object sender, EventArgs e)
        {
            if (server == null || server.IsDisposed)
            {
                server = new Server();
                server.Show();
            }
            else
            {
                server.BringToFront();
            }
        }

        private void mitmButton_Click(object sender, EventArgs e)
        {
            if (mitmWindow == null || mitmWindow.IsDisposed)
            {
                mitmWindow = new Mitm();
                mitmWindow.Show();
            }
            else
            {
                mitmWindow.BringToFront();
            }
        }

        private void packetCaptureButton_Click(object sender, EventArgs e)
        {
            // TODO: Implement packet capture window
            MessageBox.Show("Packet Capture module not yet implemented", "Info");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            // Disconnect bridge when closing
            Bridge?.Disconnect();
        }
    }
}
