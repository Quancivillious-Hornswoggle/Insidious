using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Insidious_GUI.ModuleWindows
{
    public partial class Server : Form
    {
        private System.Diagnostics.Process? pythonProcess;
        private bool isHosting = false;
        public Server()
        {
            InitializeComponent();
        }

        private void Server_Load(object sender, EventArgs e)
        {
            stopServerButton.Enabled = false;
            startServerButton.Enabled = true;
        }

        private async void startServerButton_Click(object sender, EventArgs e)
        {
            if (isHosting) return;

            folderBrowserDialog.ShowDialog();
            var sitePath = folderBrowserDialog.SelectedPath;

            // Check if a path was selected
            if (string.IsNullOrEmpty(sitePath))
            {
                return;
            }

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "python3",
                    Arguments = $"-m http.server 80 --directory \"{sitePath}\" --bind {GetLocalIPAddress()}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                pythonProcess = System.Diagnostics.Process.Start(startInfo);

                if (pythonProcess != null)
                {
                    isHosting = true;
                    addressLabel.Text = GetLocalIPAddress();
                    startServerButton.Enabled = false;
                    stopServerButton.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isHosting = false;
            }
        }

        private async void stopServerButton_Click(object sender, EventArgs e)
        {
            if (!isHosting) return;

            try
            {
                if (pythonProcess != null && !pythonProcess.HasExited)
                {
                    pythonProcess.Kill(); // Forcefully terminate the process
                    pythonProcess.WaitForExit(); // Wait for it to close
                    Console.WriteLine("Server stopped.");
                }

                isHosting = false;
                addressLabel.Text = "";
                startServerButton.Enabled = true;
                stopServerButton.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to stop server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
