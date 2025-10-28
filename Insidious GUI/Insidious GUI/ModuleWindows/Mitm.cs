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
        private bool isScanning = false;

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
                    
                case "scan_completed":
                    HandleScanCompleted(message);
                    break;
                    
                case "scan_error":
                    HandleScanError(message);
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
                
                // Update status
                this.Text = $"MITM - Poisoning {target}";
            }
        }

        private void HandleAttackProgress(Message message)
        {
            var json = JsonSerializer.Serialize(message.data);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            if (data.ContainsKey("packets_captured"))
            {
                // Could update a label showing packet count if you add one
                // packetCountLabel.Text = $"Packets: {data["packets_captured"]}";
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
            
            this.Text = "MITM";
            scanDevicesButton.Enabled = true;
            poisonAllButton.Enabled = true;
            poisonSelectedButton.Enabled = true;
        }

        private void HandleScanCompleted(Message message)
        {
            isScanning = false;
            
            try
            {
                // Parse the hosts list from the message
                var json = JsonSerializer.Serialize(message.data);
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                
                if (data.ContainsKey("hosts"))
                {
                    var hostsElement = data["hosts"];
                    
                    // Clear existing items
                    ipListBox.Items.Clear();
                    
                    // Add each host to the list
                    if (hostsElement.ValueKind == JsonValueKind.Array)
                    {
                        var hosts = hostsElement.EnumerateArray().ToList();
                        
                        foreach (var host in hosts)
                        {
                            string ipAddress = host.GetString();
                            ipListBox.Items.Add(ipAddress);
                        }
                        
                        // Update button and show result
                        scanDevicesButton.Text = "Scan For Addresses";
                        scanDevicesButton.Enabled = true;
                        
                        MessageBox.Show(
                            $"Scan complete! Found {hosts.Count} devices.",
                            "Scan Complete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error parsing scan results: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                
                scanDevicesButton.Text = "Scan For Addresses";
                scanDevicesButton.Enabled = true;
            }
        }

        private void HandleScanError(Message message)
        {
            isScanning = false;
            
            var json = JsonSerializer.Serialize(message.data);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            string errorMsg = data.ContainsKey("error") ? data["error"].ToString() : "Unknown error";
            
            ipListBox.Items.Clear();
            scanDevicesButton.Text = "Scan For Addresses";
            scanDevicesButton.Enabled = true;
            
            MessageBox.Show(
                $"Scan failed: {errorMsg}",
                "Scan Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private async void scanDevicesButton_Click(object sender, EventArgs e)
        {
            if (isScanning)
            {
                MessageBox.Show("Scan already in progress", "Info");
                return;
            }

            try
            {
                isScanning = true;
                
                // Update button
                scanDevicesButton.Text = "Scanning...";
                scanDevicesButton.Enabled = false;
                
                // Clear current list
                ipListBox.Items.Clear();
                ipListBox.Items.Add("Scanning network...");
                
                // Send scan command
                await Form1.Bridge.SendCommandAsync("mitm", "scan");
                
                // The HandleScanCompleted event will populate the list when done
            }
            catch (Exception ex)
            {
                isScanning = false;
                scanDevicesButton.Text = "Scan For Addresses";
                scanDevicesButton.Enabled = true;
                
                MessageBox.Show($"Error starting scan: {ex.Message}", "Error");
            }
        }

        private async void startPoisonButton_Click(object sender, EventArgs e)
        {
            if (isPoisoning)
            {
                MessageBox.Show("MITM attack already in progress", "Info");
                return;
            }

            // Check if we have scanned devices
            if (ipListBox.Items.Count == 0)
            {
                MessageBox.Show(
                    "Please scan for devices first",
                    "No Devices",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var result = MessageBox.Show(
                $"Start MITM attack on all devices?\n\n" +
                $"This will poison {ipListBox.Items.Count} devices.",
                "Confirm Attack",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    isPoisoning = true;
                    
                    // Get all IPs from the list
                    var targetIps = new List<string>();
                    foreach (var item in ipListBox.Items)
                    {
                        targetIps.Add(item.ToString());
                    }
                    
                    var data = new { target_ips = targetIps };
                    
                    await Form1.Bridge.SendCommandAsync("mitm", "poison_all", data);
                    
                    MessageBox.Show("MITM attack started on all devices", "Info");
                    
                    // Disable buttons during attack
                    scanDevicesButton.Enabled = false;
                    poisonAllButton.Enabled = false;
                    poisonSelectedButton.Enabled = false;
                }
                catch (Exception ex)
                {
                    isPoisoning = false;
                    MessageBox.Show($"Error starting attack: {ex.Message}", "Error");
                }
            }
        }

        private async void poisonSelectedButton_Click(object sender, EventArgs e)
        {
            if (isPoisoning)
            {
                MessageBox.Show("MITM attack already in progress", "Info");
                return;
            }

            // Check if an IP is selected
            if (ipListBox.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a target IP address",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string selectedIp = ipListBox.SelectedItem.ToString();

            var result = MessageBox.Show(
                $"Start MITM attack on {selectedIp}?",
                "Confirm Attack",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    isPoisoning = true;
                    
                    var data = new { target_ip = selectedIp };
                    
                    await Form1.Bridge.SendCommandAsync("mitm", "poison_selected", data);
                    
                    MessageBox.Show($"MITM attack started on {selectedIp}", "Info");
                    
                    // Disable buttons during attack
                    scanDevicesButton.Enabled = false;
                    poisonAllButton.Enabled = false;
                    poisonSelectedButton.Enabled = false;
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
                await Form1.Bridge.SendCommandAsync("mitm", "stop");
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
    }
}
