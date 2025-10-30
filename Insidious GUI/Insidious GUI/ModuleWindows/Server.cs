using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Insidious_GUI.ModuleWindows
{
    public partial class Server : Form
    {
        private bool isHosting = false;
        public Server()
        {
            InitializeComponent();
            Form1.Bridge.EventReceived += Bridge_EventReceived;
        }

        private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[MITM] Event: {e.Message.module}.{e.Message.action}");

            if (e.Message.module != "mitm")
                return;

            if (InvokeRequired)
                Invoke(new Action(() => HandleServerEvent(e.Message)));
            else
                HandleServerEvent(e.Message);
        }

        private void HandleServerEvent(Message message)
        {
            System.Diagnostics.Debug.WriteLine($"[MITM] Handling: {message.action}");
        }

        private void Server_Load(object sender, EventArgs e)
        {

        }

        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.Bridge.EventReceived -= Bridge_EventReceived;
            base.OnFormClosing(e);
        }

        private async void startServerButton_Click(object sender, EventArgs e)
        {
            if (isHosting) return;

            folderBrowserDialog.ShowDialog();
            var sitePath = folderBrowserDialog.SelectedPath;

            isHosting = true;
            await Form1.Bridge.SendCommandAsync("server", "start", new { site_directory = sitePath });
        }

        private async void stopServerButton_Click(object sender, EventArgs e)
        {
            isHosting = false;
            await Form1.Bridge.SendCommandAsync("server", "stop");
        }
    }
}
