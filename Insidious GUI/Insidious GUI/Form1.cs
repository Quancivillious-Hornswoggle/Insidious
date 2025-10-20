using System.Diagnostics;
using System.Net.Sockets;    

namespace Insidious_GUI
{
    public partial class Form1 : Form
    {
        private TcpClient bridge;

        // Windows

        Deauth deauthWindow = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Run python bridge and then connect to it
            Debug.WriteLine("Starting Bridge");

            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = @"..\..\..\..\..\python_bridge.py",
                UseShellExecute = false,   // required to hide window
                CreateNoWindow = false      // hide the terminal
            };

            //Process.Start(psi);

            //Debug.WriteLine("Bridge Started");

            // Connect to bridge
            while (bridge == null)
            {
                try
                {
                    bridge = new TcpClient("172.16.105.1", 65535);
                    Debug.WriteLine("Connected!");
                }
                catch
                {
                    Debug.WriteLine("Connection failed, retrying...");
                    Thread.Sleep(1000); // wait 1 second before retry
                }
            }

        }

        private void wifiBruteButton_Click(object sender, EventArgs e)
        {

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

        }

        private void serverHostButton_Click(object sender, EventArgs e)
        {

        }

        private void mitmButton_Click(object sender, EventArgs e)
        {

        }

        private void packetCaptureButton_Click(object sender, EventArgs e)
        {

        }
    }
}
