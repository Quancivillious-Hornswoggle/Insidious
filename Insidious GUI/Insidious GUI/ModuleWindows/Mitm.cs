using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Insidious_GUI
{
    public partial class Mitm : Form
    {
        private bool isPoisoning = false;

        public Mitm()
        {
            InitializeComponent();

            // Subscribe to mitm-specific events
            Form1.Bridge.EventReceived += Bridge_EventReceived;
        }

        private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
        {
            // Only handle events for mitm module
            if (e.Message.module != "mitm")
                return;

            // Handle events on UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleMitmEvent(e.Message)));
            }
            else
            {
                HandleMitmEvent(e.Message);
            }
        }

        private void HandleMitmEvent(Message message)
        {
            switch (message.action)
            {
                case "attack_status":
                    HandleAttackStatus(message);
                    break;

                case "attack_progress":
                    HandleAttackProgress(message);
                    break;

                case "attack_stopped":
                    HandleAttackStopped(message);
                    break;

                case "status_update":
                    // Handle adapter mode changes
                    break;
            }
        }

        private void HandleAttackStatus(Message message)
        {
            var json = JsonSerializer.Serialize(message.data);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (data.ContainsKey("status") && data["status"].ToString() == "poisoning")
            {
                string target = data.ContainsKey("target") ? data["target"].ToString() : "unknown";
                string gateway = data.ContainsKey("gateway") ? data["gateway"].ToString() : "unknown";

                // TODO: Update status label
                // statusLabel.Text = $"Poisoning: {target} <-> {gateway}";
            }
        }

        private void HandleAttackProgress(Message message)
        {
            var json = JsonSerializer.Serialize(message.data);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (data.ContainsKey("packets_captured"))
            {
                // TODO: Update packet count label
                // packetCountLabel.Text = $"Packets captured: {data["packets_captured"]}";
            }
        }

        private void HandleAttackStopped(Message message)
        {
            isPoisoning = false;

            var json = JsonSerializer.Serialize(message.data);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (data.ContainsKey("total_packets"))
            {
                MessageBox.Show(
                    $"MITM attack stopped. Total packets captured: {data["total_packets"]}",
                    "Attack Stopped"
                );
            }

            // TODO: Re-enable start button, disable stop button
        }

        private async void startPoisonButton_Click(object sender, EventArgs e)
        {
            if (isPoisoning)
            {
                MessageBox.Show("MITM attack already in progress", "Info");
                return;
            }

            // TODO: Get target IP and gateway IP from your UI controls
            string targetIp = "192.168.1.100"; // Replace with actual input
            string gatewayIp = "192.168.0.1"; // Replace with actual input

            if (string.IsNullOrEmpty(targetIp) || string.IsNullOrEmpty(gatewayIp))
            {
                MessageBox.Show("Please enter target IP and gateway IP", "Info");
                return;
            }

            var result = MessageBox.Show(
                $"Start MITM attack?\nTarget: {targetIp}\nGateway: {gatewayIp}",
                "Confirm Attack",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    isPoisoning = true;

                    var data = new
                    {
                        target_ip = targetIp,
                        gateway_ip = gatewayIp
                    };

                    await Form1.Bridge.SendCommandAsync("mitm", "poison_all", data);

                    MessageBox.Show("MITM attack started", "Info");

                    // TODO: Disable start button, enable stop button
                }
                catch (Exception ex)
                {
                    isPoisoning = false;
                    MessageBox.Show($"Error starting attack: {ex.Message}", "Error");
                }
            }
        }

        private async void stopPoisonButton_Click(object sender, EventArgs e)
        {
            if (!isPoisoning)
            {
                MessageBox.Show("No MITM attack in progress", "Info");
                return;
            }

            try
            {
                await Form1.Bridge.SendCommandAsync("mitm", "stop_poison");
                MessageBox.Show("Stopping MITM attack...", "Info");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping attack: {ex.Message}", "Error");
            }
        }

        private async void getStatusButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Use waitForResponse to get immediate status
                var response = await Form1.Bridge.SendCommandAsync("mitm", "get_status", waitForResponse: true);

                var json = JsonSerializer.Serialize(response.data);
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                string status = $"MITM Module Status:\n\n";
                status += $"Is Poisoning: {data["is_poisoning"]}\n";
                status += $"Target IP: {data["target_ip"]}\n";
                status += $"Gateway IP: {data["gateway_ip"]}\n";
                status += $"Packets Captured: {data["packets_captured"]}\n";
                status += $"Adapter Mode: {data["adapter_mode"]}\n";

                MessageBox.Show(status, "Module Status");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting status: {ex.Message}", "Error");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Unsubscribe from events
            Form1.Bridge.EventReceived -= Bridge_EventReceived;
        }

        private void scanDevicesButton_Click(object sender, EventArgs e)
        {

        }

        private void poisonAllButton_Click(object sender, EventArgs e)
        {

        }
    }
}
