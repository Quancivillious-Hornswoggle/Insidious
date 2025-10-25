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
    public partial class Deauth : Form
    {
        private bool isScanning = false;
        private bool isAttacking = false;

        public Deauth()
        {
            InitializeComponent();
            
            // Subscribe to deauth-specific events
            Form1.Bridge.EventReceived += Bridge_EventReceived;
        }

        private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
        {
            // Only handle events for deauth module
            if (e.Message.module != "deauth")
                return;

            // Handle events on UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleDeauthEvent(e.Message)));
            }
            else
            {
                HandleDeauthEvent(e.Message);
            }
        }

        private void HandleDeauthEvent(Message message)
        {
            switch (message.action)
            {
                case "scan_complete":
                    HandleScanComplete(message);
                    break;
                    
                case "scan_progress":
                    HandleScanProgress(message);
                    break;
                    
                case "devices_found":
                    HandleDevicesFound(message);
                    break;
                    
                case "attack_progress":
                    HandleAttackProgress(message);
                    break;
                    
                case "attack_stopped":
                    HandleAttackStopped(message);
                    break;
                    
                case "status_update":
                    // Handle adapter mode changes, etc.
                    break;
            }
        }

        private void HandleScanComplete(Message message)
        {
            isScanning = false;
            
            // Parse access points from message data
            var json = JsonSerializer.Serialize(message.data);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            if (data.ContainsKey("access_points"))
            {
                var apsJson = JsonSerializer.Serialize(data["access_points"]);
                var accessPoints = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(apsJson);
                
                // Update UI with access points
                // TODO: Update your ListBox/ListView with AP data
                // Example:
                // apListBox.Items.Clear();
                // foreach (var ap in accessPoints)
                // {
                //     apListBox.Items.Add($"{ap["ssid"]} - {ap["bssid"]} (Ch: {ap["channel"]})");
                // }
                
                MessageBox.Show($"Scan complete! Found {accessPoints.Count} access points.", "Scan Complete");
            }
        }

        private void HandleScanProgress(Message message)
        {
            // Update progress bar or status label
            // TODO: Update UI with scan progress
        }

        private void HandleDevicesFound(Message message)
        {
            // Parse devices from message data
            var json = JsonSerializer.Serialize(message.data);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            if (data.ContainsKey("devices"))
            {
                var devicesJson = JsonSerializer.Serialize(data["devices"]);
                var devices = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(devicesJson);
                
                // Update UI with devices
                // TODO: Update your ListBox/ListView with device data
                
                MessageBox.Show($"Found {devices.Count} devices!", "Devices Found");
            }
        }

        private void HandleAttackProgress(Message message)
        {
            // Update attack progress
            var json = JsonSerializer.Serialize(message.data);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            if (data.ContainsKey("packets_sent"))
            {
                // TODO: Update label showing packet count
                // statusLabel.Text = $"Packets sent: {data["packets_sent"]}";
            }
        }

        private void HandleAttackStopped(Message message)
        {
            isAttacking = false;
            
            var json = JsonSerializer.Serialize(message.data);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            if (data.ContainsKey("total_packets"))
            {
                MessageBox.Show($"Attack stopped. Total packets: {data["total_packets"]}", "Attack Stopped");
            }
        }

        private async void scanAPButton_Click(object sender, EventArgs e)
        {
            if (isScanning)
            {
                MessageBox.Show("Scan already in progress", "Info");
                return;
            }

            try
            {
                isScanning = true;
                // TODO: Disable button or show scanning indicator
                // scanAPButton.Enabled = false;

                // Send scan command (don't wait for response, we'll get events)
                await Form1.Bridge.SendCommandAsync("deauth", "scan_ap");
                
                MessageBox.Show("AP scan started. Results will appear when complete.", "Scanning");
            }
            catch (Exception ex)
            {
                isScanning = false;
                MessageBox.Show($"Error starting scan: {ex.Message}", "Error");
            }
        }

        private async void scanDevicesButton_Click(object sender, EventArgs e)
        {
            // TODO: Get selected BSSID from your AP list
            string selectedBssid = "AA:BB:CC:DD:EE:FF"; // Replace with actual selection

            if (string.IsNullOrEmpty(selectedBssid))
            {
                MessageBox.Show("Please select an access point first", "Info");
                return;
            }

            try
            {
                // Send scan devices command with BSSID
                var data = new { bssid = selectedBssid };
                await Form1.Bridge.SendCommandAsync("deauth", "scan_devices", data);
                
                MessageBox.Show($"Scanning devices on {selectedBssid}...", "Scanning");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error scanning devices: {ex.Message}", "Error");
            }
        }

        private async void deauthAllButton_Click(object sender, EventArgs e)
        {
            // TODO: Get selected BSSID from your AP list
            string selectedBssid = "AA:BB:CC:DD:EE:FF"; // Replace with actual selection

            if (string.IsNullOrEmpty(selectedBssid))
            {
                MessageBox.Show("Please select an access point first", "Info");
                return;
            }

            if (isAttacking)
            {
                MessageBox.Show("Attack already in progress", "Info");
                return;
            }

            var result = MessageBox.Show(
                $"Start deauth attack on {selectedBssid}?",
                "Confirm Attack",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    isAttacking = true;
                    var data = new { bssid = selectedBssid };
                    await Form1.Bridge.SendCommandAsync("deauth", "deauth_all", data);
                    
                    // TODO: Enable stop button
                }
                catch (Exception ex)
                {
                    isAttacking = false;
                    MessageBox.Show($"Error starting attack: {ex.Message}", "Error");
                }
            }
        }

        private async void deauthSingleButton_Click(object sender, EventArgs e)
        {
            // TODO: Get selected BSSID and device MAC
            string selectedBssid = "AA:BB:CC:DD:EE:FF"; // Replace with actual selection
            string selectedMac = "11:22:33:44:55:66"; // Replace with actual selection

            if (string.IsNullOrEmpty(selectedBssid) || string.IsNullOrEmpty(selectedMac))
            {
                MessageBox.Show("Please select an access point and device first", "Info");
                return;
            }

            if (isAttacking)
            {
                MessageBox.Show("Attack already in progress", "Info");
                return;
            }

            var result = MessageBox.Show(
                $"Start deauth attack on device {selectedMac}?",
                "Confirm Attack",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    isAttacking = true;
                    var data = new { bssid = selectedBssid, mac = selectedMac };
                    await Form1.Bridge.SendCommandAsync("deauth", "deauth_single", data);
                }
                catch (Exception ex)
                {
                    isAttacking = false;
                    MessageBox.Show($"Error starting attack: {ex.Message}", "Error");
                }
            }
        }

        private async void stopAttackButton_Click(object sender, EventArgs e)
        {
            if (!isAttacking)
            {
                MessageBox.Show("No attack in progress", "Info");
                return;
            }

            try
            {
                await Form1.Bridge.SendCommandAsync("deauth", "stop_attack");
                MessageBox.Show("Stopping attack...", "Info");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping attack: {ex.Message}", "Error");
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
