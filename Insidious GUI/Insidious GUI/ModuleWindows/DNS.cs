using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Insidious_GUI.ModuleWindows
{
    public partial class DNS : Form
    {
        private bool isSpoofing = false;
        public DNS()
        {
            InitializeComponent();
        }

        private void DNS_Load(object sender, EventArgs e)
        {
            forwardCheckedButton.Enabled = false;
            forwardAllButton.Enabled = false;
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

        private void getLocalIPButon_Click(object sender, EventArgs e)
        {
            forwardAddressTextBox.Text = GetLocalIPAddress();
            forwardAllButton.Enabled = true;
        }

        private void addIpButton_Click(object sender, EventArgs e)
        {
            if (spoofAddressTextBox.Text.Length > 0 || spoofAddressTextBox.Text != string.Empty || spoofAddressTextBox.Text != null)
                addressCheckedListBox.Items.Add(spoofAddressTextBox.Text);
            forwardCheckedButton.Enabled = true;
        }

        private void forwardIPTextBoxTextChanged(object sender, EventArgs e)
        {
            if (forwardAddressTextBox.Text.Length > 0 || forwardAddressTextBox.Text != string.Empty || forwardAddressTextBox.Text != null)
                forwardAllButton.Enabled = true;
            else
                forwardAllButton.Enabled = false;
        }

        private async void forwardCheckedButton_Click(object sender, EventArgs e)
        {
            if (isSpoofing)
            {
                MessageBox.Show("Already Spoofing.", "Error");
                return;
            }

            if (forwardAddressTextBox.Text.Length == 0 || forwardAddressTextBox.Text == string.Empty || forwardAddressTextBox.Text == null)
                MessageBox.Show("You did not type in an IP to forward to.", "Error");

            await Form1.Bridge.SendCommandAsync("dns", "spoof_selected", new { target_ip = forwardAddressTextBox.Text, spoof_targets = addressCheckedListBox.CheckedItems });
        }

        private async void forwardAllButton_Click(object sender, EventArgs e)
        {
            if (isSpoofing)
            {
                MessageBox.Show("Already Spoofing.", "Error");
                return;
            }

            await Form1.Bridge.SendCommandAsync("dns", "spoof_all", new { target_ip = forwardAddressTextBox.Text });
        }

        private async void stopButton_Click(object sender, EventArgs e)
        {
            await Form1.Bridge.SendCommandAsync("dns", "stop");
        }
    }
}
