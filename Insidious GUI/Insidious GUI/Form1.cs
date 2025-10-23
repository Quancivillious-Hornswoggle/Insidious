using System.Diagnostics;
using System.Net.Sockets;    

namespace Insidious_GUI
{
    public partial class Form1 : Form
    {
        public static TcpClient bridge;

        // Windows

        Deauth deauthWindow = null;
        Mitm mitm = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Run python bridge and then connect to it
            Debug.WriteLine("Connecting To Bridge");

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
            if (mitm == null || mitm.IsDisposed)
            {
                mitm = new Mitm();
                mitm.Show();
            }
            else
            {
                mitm.BringToFront();
            }
        }

        private void packetCaptureButton_Click(object sender, EventArgs e)
        {

        }
    }
}
