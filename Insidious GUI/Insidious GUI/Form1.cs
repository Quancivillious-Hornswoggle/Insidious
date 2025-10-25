using System.Diagnostics;
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

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Initialize bridge manager
            Debug.WriteLine("Initializing Bridge Manager");
            Bridge = new BridgeManager();

            // Subscribe to global events (optional - for debugging/logging)
            Bridge.EventReceived += Bridge_EventReceived;
            Bridge.ErrorReceived += Bridge_ErrorReceived;

            // Try to connect
            Debug.WriteLine("Connecting to Python bridge...");
            
            bool connected = await Bridge.ConnectAsync("127.0.0.1", 65535, retries: 10);
            
            if (connected)
            {
                Debug.WriteLine("Connected successfully!");
                this.Text = "Insidious - Connected";
            }
            else
            {
                Debug.WriteLine("Connection failed!");
                this.Text = "Insidious - Connection Failed";
                MessageBox.Show(
                    "Failed to connect to Python bridge. Make sure python_bridge.py is running.",
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
        {
            // Global event handler - could log all events
            Debug.WriteLine($"[EVENT] {e.Message.module}.{e.Message.action}");
        }

        private void Bridge_ErrorReceived(object sender, MessageReceivedEventArgs e)
        {
            // Global error handler
            Debug.WriteLine($"[ERROR] {e.Message.module}: {e.Message.data}");
            
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
            // TODO: Implement DNS spoof window
            MessageBox.Show("DNS Spoof module not yet implemented", "Info");
        }

        private void serverHostButton_Click(object sender, EventArgs e)
        {
            // TODO: Implement server host window
            MessageBox.Show("Server Host module not yet implemented", "Info");
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
