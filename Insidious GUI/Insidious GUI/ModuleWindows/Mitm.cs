using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Insidious_GUI
{
    public partial class Mitm : Form
    {
        private bool isPoisoning = false;
        private bool isScanning = false;

        public Mitm()
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
                Invoke(new Action(() => HandleMitmEvent(e.Message)));
            else
                HandleMitmEvent(e.Message);
        }

        private void HandleMitmEvent(Message message)
        {
            System.Diagnostics.Debug.WriteLine($"[MITM] Handling: {message.action}");

            switch (message.action)
            {
                case "scan_completed":
                    HandleScanCompleted(message);
                    break;
                case "scan_error":
                    HandleScanError(message);
                    break;
            }
        }

        private void HandleScanCompleted(Message message)
        {
            isScanning = false;

            try
            {
                System.Diagnostics.Debug.WriteLine("[MITM] === SCAN COMPLETED ===");
                System.Diagnostics.Debug.WriteLine($"[MITM] Data type: {message.data?.GetType()}");

                ipListBox.Items.Clear();

                // Simple: message.data should be a JsonElement
                if (message.data is JsonElement element)
                {
                    System.Diagnostics.Debug.WriteLine($"[MITM] JsonElement kind: {element.ValueKind}");

                    if (element.TryGetProperty("hosts", out JsonElement hostsArray))
                    {
                        System.Diagnostics.Debug.WriteLine($"[MITM] Hosts kind: {hostsArray.ValueKind}");

                        if (hostsArray.ValueKind == JsonValueKind.Array)
                        {
                            int count = 0;
                            foreach (JsonElement hostElement in hostsArray.EnumerateArray())
                            {
                                string ip = hostElement.GetString();
                                System.Diagnostics.Debug.WriteLine($"[MITM] IP: {ip}");
                                ipListBox.Items.Add(ip);
                                count++;
                            }

                            System.Diagnostics.Debug.WriteLine($"[MITM] Added {count} IPs");
                            scanDevicesButton.Text = "Scan For Addresses";
                            scanDevicesButton.Enabled = true;
                            MessageBox.Show($"Found {count} devices!", "Scan Complete");
                            return;
                        }
                    }
                }

                throw new Exception("Could not parse hosts from message");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MITM] ERROR: {ex.Message}");
                scanDevicesButton.Text = "Scan For Addresses";
                scanDevicesButton.Enabled = true;
                ipListBox.Items.Add("Scan failed - check console");
                MessageBox.Show($"Error: {ex.Message}", "Scan Error");
            }
        }

        private void HandleScanError(Message message)
        {
            isScanning = false;
            scanDevicesButton.Text = "Scan For Addresses";
            scanDevicesButton.Enabled = true;
            ipListBox.Items.Clear();
            MessageBox.Show("Scan failed - check Python console", "Error");
        }

        private async void scanDevicesButton_Click(object sender, EventArgs e)
        {
            if (isScanning) return;

            isScanning = true;
            scanDevicesButton.Text = "Scanning...";
            scanDevicesButton.Enabled = false;
            ipListBox.Items.Clear();
            ipListBox.Items.Add("Scanning...");

            await Form1.Bridge.SendCommandAsync("mitm", "scan");
        }

        private async void startPoisonButton_Click(object sender, EventArgs e)
        {
            if (isPoisoning || ipListBox.Items.Count == 0) return;

            var targetIps = new List<string>();
            foreach (var item in ipListBox.Items)
                targetIps.Add(item.ToString());

            await Form1.Bridge.SendCommandAsync("mitm", "poison_all", new { target_ips = targetIps });
            isPoisoning = true;
        }

        private async void poisonSelectedButton_Click(object sender, EventArgs e)
        {
            doSButton.Enabled = false;

            if (isPoisoning || ipListBox.SelectedItem == null) return;

            string ip = ipListBox.SelectedItem.ToString();
            await Form1.Bridge.SendCommandAsync("mitm", "poison_selected", new { target_ip = ip });
            isPoisoning = true;
        }

        private async void stopPoisonButton_Click(object sender, EventArgs e)
        {
            if (!isPoisoning) return;
            await Form1.Bridge.SendCommandAsync("mitm", "stop");
            isPoisoning = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Form1.Bridge.EventReceived -= Bridge_EventReceived;
            base.OnFormClosing(e);
        }

        private async void doSButton_Click(object sender, EventArgs e)
        {
            doSButton.Enabled = !doSButton.Enabled;
            packetPassthroughButton.Enabled = !packetPassthroughButton.Enabled;

            await Form1.Bridge.SendCommandAsync("mitm", "dos");
        }

        private async void packetPassthroughButton_Click(object sender, EventArgs e)
        {
            doSButton.Enabled = !doSButton.Enabled;
            packetPassthroughButton.Enabled = !packetPassthroughButton.Enabled;

            await Form1.Bridge.SendCommandAsync("mitm", "passthrough");
        }
    }
}
