# Command & Reply Cheat Sheet

## Quick Reference for Sending Messages

This guide shows you exactly what code to use for sending and receiving messages in both C# and Python.

---

## C# Side (GUI Windows)

### 1. Send a Command (Fire and Forget)

Use this when you just want to tell Python to do something and don't need to wait for a response.

```csharp
// Basic command with no data
await Form1.Bridge.SendCommandAsync("deauth", "scan_ap");

// Command with data
var data = new { 
    bssid = "AA:BB:CC:DD:EE:FF",
    channel = 6 
};
await Form1.Bridge.SendCommandAsync("deauth", "scan_devices", data);

// Full example in a button click
private async void ScanButton_Click(object sender, EventArgs e)
{
    try
    {
        await Form1.Bridge.SendCommandAsync("deauth", "scan_ap");
        MessageBox.Show("Scan started!");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}");
    }
}
```

### 2. Send a Command and Wait for Response

Use this when you need data back immediately (like "get status").

```csharp
// Send and wait for response
var response = await Form1.Bridge.SendCommandAsync(
    "deauth", 
    "get_status", 
    waitForResponse: true
);

// Access the response data
var json = JsonSerializer.Serialize(response.data);
var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

// Full example
private async void GetStatusButton_Click(object sender, EventArgs e)
{
    try
    {
        var response = await Form1.Bridge.SendCommandAsync(
            "deauth", 
            "get_status", 
            waitForResponse: true
        );
        
        // Parse response
        var json = JsonSerializer.Serialize(response.data);
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        
        // Use the data
        string status = data["is_scanning"].ToString();
        MessageBox.Show($"Is scanning: {status}");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}");
    }
}
```

### 3. Receive Events from Python

Use this to get real-time updates (like progress updates, scan results, etc.)

```csharp
// In your window constructor
public Deauth()
{
    InitializeComponent();
    
    // Subscribe to events
    Form1.Bridge.EventReceived += Bridge_EventReceived;
}

// Event handler - filters and routes events
private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
{
    // Only handle events for YOUR module
    if (e.Message.module != "deauth")
        return;
    
    // Handle on UI thread
    if (InvokeRequired)
    {
        Invoke(new Action(() => HandleDeauthEvent(e.Message)));
    }
    else
    {
        HandleDeauthEvent(e.Message);
    }
}

// Process the event
private void HandleDeauthEvent(Message message)
{
    switch (message.action)
    {
        case "scan_complete":
            // Parse data
            var json = JsonSerializer.Serialize(message.data);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            // Example: Get access points list
            var apsJson = JsonSerializer.Serialize(data["access_points"]);
            var accessPoints = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(apsJson);
            
            // Update UI
            MessageBox.Show($"Found {accessPoints.Count} access points!");
            break;
            
        case "scan_progress":
            // Update progress bar
            var progressData = JsonSerializer.Deserialize<Dictionary<string, object>>(
                JsonSerializer.Serialize(message.data)
            );
            int percent = Convert.ToInt32(progressData["progress"]);
            progressBar.Value = percent;
            break;
            
        case "attack_stopped":
            MessageBox.Show("Attack stopped!");
            break;
    }
}

// IMPORTANT: Unsubscribe when window closes!
protected override void OnFormClosing(FormClosingEventArgs e)
{
    Form1.Bridge.EventReceived -= Bridge_EventReceived;
    base.OnFormClosing(e);
}
```

### 4. Common C# Patterns

#### Pattern A: Simple Command
```csharp
private async void SimpleButton_Click(object sender, EventArgs e)
{
    await Form1.Bridge.SendCommandAsync("module_name", "action_name");
}
```

#### Pattern B: Command with Input
```csharp
private async void CommandWithInputButton_Click(object sender, EventArgs e)
{
    string userInput = textBox1.Text;
    
    var data = new { 
        target = userInput,
        option = true
    };
    
    await Form1.Bridge.SendCommandAsync("module_name", "action_name", data);
}
```

#### Pattern C: Get Data and Display
```csharp
private async void GetDataButton_Click(object sender, EventArgs e)
{
    var response = await Form1.Bridge.SendCommandAsync(
        "module_name", 
        "get_data", 
        waitForResponse: true
    );
    
    var json = JsonSerializer.Serialize(response.data);
    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
    
    listBox1.Items.Clear();
    foreach (var item in (JsonElement)data["items"])
    {
        listBox1.Items.Add(item.ToString());
    }
}
```

---

## Python Side (Modules)

### 1. Register Command Handlers

In your module's `__init__` method:

```python
class MyModule(BaseModule):
    def __init__(self):
        super().__init__("my_module")
        
        # Register handlers for different actions
        self.register_handler("scan_ap", self.handle_scan_ap)
        self.register_handler("start_attack", self.handle_start_attack)
        self.register_handler("stop_attack", self.handle_stop_attack)
        self.register_handler("get_status", self.handle_get_status)
```

### 2. Handle Commands and Send Responses

#### Option A: Simple Response (Quick Operations)
```python
def handle_get_status(self, message):
    """Handler that returns immediately"""
    
    # Do something quick
    status = {
        "is_running": self.is_running,
        "packets_sent": self.packet_count,
        "target": self.target
    }
    
    # Return response data
    return status
    
    # The BaseModule will automatically send this as a RESPONSE message
```

#### Option B: Background Task (Long Operations)
```python
def handle_scan_ap(self, message):
    """Handler that starts background work"""
    
    # Start background thread
    threading.Thread(target=self._scan_worker, daemon=True).start()
    
    # Return immediate response
    return {"status": "scan_started"}
    
    # The background worker will send events when done

def _scan_worker(self):
    """Background worker"""
    # Do the actual work
    results = scan_for_access_points()
    
    # Send event when done
    self.send_event("scan_complete", {"access_points": results})
```

#### Option C: Access Command Data
```python
def handle_deauth_single(self, message):
    """Handler that uses data from the command"""
    
    # Get data from the message
    target_bssid = message.data.get("bssid")
    target_mac = message.data.get("mac")
    
    # Validate
    if not target_bssid or not target_mac:
        # Send error
        self.send_error("BSSID and MAC are required")
        return {"status": "error", "message": "Missing parameters"}
    
    # Start attack
    self.start_attack(target_bssid, target_mac)
    
    return {"status": "attack_started", "target": target_mac}
```

### 3. Send Events (Real-Time Updates)

```python
# Send a simple event
self.send_event("scan_started", {"timestamp": time.time()})

# Send progress update
self.send_event("scan_progress", {
    "progress": 50,
    "current": "Scanning channel 6"
})

# Send completion event with data
self.send_event("scan_complete", {
    "access_points": [
        {"ssid": "Network1", "bssid": "AA:BB:CC:DD:EE:FF"},
        {"ssid": "Network2", "bssid": "11:22:33:44:55:66"}
    ],
    "count": 2
})

# Send error event
self.send_event("scan_error", {
    "error": "No wireless adapter found"
})
```

### 4. Send Explicit Response

```python
# Usually you just return data and it's sent automatically
# But you can also send explicitly:

self.send_response("action_name", {
    "status": "success",
    "data": result
}, message.msg_id)
```

### 5. Send Error

```python
# Send an error message
self.send_error("Something went wrong", message.msg_id)

# Or in a try-catch
try:
    result = do_something()
    return {"status": "success", "result": result}
except Exception as e:
    self.send_error(str(e))
    return {"status": "error"}
```

### 6. Common Python Patterns

#### Pattern A: Quick Response
```python
def handle_get_something(self, message):
    return {"data": self.some_data}
```

#### Pattern B: Long Task with Progress
```python
def handle_long_task(self, message):
    threading.Thread(target=self._long_task_worker, daemon=True).start()
    return {"status": "started"}

def _long_task_worker(self):
    for i in range(100):
        # Do work
        time.sleep(0.1)
        
        # Send progress
        if i % 10 == 0:
            self.send_event("progress", {"percent": i})
    
    # Send completion
    self.send_event("complete", {"result": "done"})
```

#### Pattern C: Validate Input and Process
```python
def handle_action(self, message):
    # Get data
    required_field = message.data.get("required_field")
    
    # Validate
    if not required_field:
        self.send_error("required_field is missing")
        return {"status": "error"}
    
    # Process
    result = process(required_field)
    
    # Return
    return {"status": "success", "result": result}
```

---

## Complete Examples

### Example 1: Scan Access Points

**C# (Button Click):**
```csharp
private async void ScanAPButton_Click(object sender, EventArgs e)
{
    try
    {
        // Send scan command
        await Form1.Bridge.SendCommandAsync("deauth", "scan_ap");
        
        // Update UI
        scanButton.Enabled = false;
        statusLabel.Text = "Scanning...";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}");
    }
}

// Event handler to receive results
private void HandleDeauthEvent(Message message)
{
    if (message.action == "scan_complete")
    {
        // Parse results
        var json = JsonSerializer.Serialize(message.data);
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        var apsJson = JsonSerializer.Serialize(data["access_points"]);
        var accessPoints = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(apsJson);
        
        // Update UI
        listBoxAPs.Items.Clear();
        foreach (var ap in accessPoints)
        {
            string display = $"{ap["ssid"]} - {ap["bssid"]} (Ch: {ap["channel"]})";
            listBoxAPs.Items.Add(display);
        }
        
        scanButton.Enabled = true;
        statusLabel.Text = $"Found {accessPoints.Count} access points";
    }
}
```

**Python (Handler):**
```python
def handle_scan_ap(self, message):
    """Start AP scan"""
    # Start background scan
    threading.Thread(target=self._scan_worker, daemon=True).start()
    
    # Return immediate response
    return {"status": "scan_started"}

def _scan_worker(self):
    """Background worker that scans for APs"""
    try:
        # Do the actual scanning
        access_points = []
        
        # ... scanning code here ...
        # For example:
        access_points = [
            {"ssid": "Network1", "bssid": "AA:BB:CC:DD:EE:FF", "channel": 6, "signal": -45},
            {"ssid": "Network2", "bssid": "11:22:33:44:55:66", "channel": 11, "signal": -60}
        ]
        
        # Send results
        self.send_event("scan_complete", {
            "status": "complete",
            "access_points": access_points
        })
        
    except Exception as e:
        self.send_event("scan_error", {"error": str(e)})
```

### Example 2: Start Attack with Parameters

**C# (Button Click):**
```csharp
private async void StartAttackButton_Click(object sender, EventArgs e)
{
    // Get selected item
    if (listBoxAPs.SelectedItem == null)
    {
        MessageBox.Show("Please select an access point");
        return;
    }
    
    string selectedItem = listBoxAPs.SelectedItem.ToString();
    string bssid = ExtractBSSID(selectedItem); // Your method to extract BSSID
    
    // Confirm
    var result = MessageBox.Show(
        $"Start attack on {bssid}?",
        "Confirm",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning
    );
    
    if (result == DialogResult.Yes)
    {
        try
        {
            // Send attack command with data
            var data = new { bssid = bssid };
            await Form1.Bridge.SendCommandAsync("deauth", "deauth_all", data);
            
            // Update UI
            startButton.Enabled = false;
            stopButton.Enabled = true;
            statusLabel.Text = "Attack in progress...";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }
}
```

**Python (Handler):**
```python
def handle_deauth_all(self, message):
    """Start deauth attack on all devices"""
    # Get target from message
    target_bssid = message.data.get("bssid")
    
    # Validate
    if not target_bssid:
        self.send_error("BSSID required")
        return {"status": "error", "message": "BSSID required"}
    
    # Start attack
    self.is_attacking = True
    self.target_bssid = target_bssid
    threading.Thread(target=self._attack_worker, daemon=True).start()
    
    return {"status": "attack_started", "target": target_bssid}

def _attack_worker(self):
    """Background attack worker"""
    packet_count = 0
    
    try:
        while self.is_attacking:
            # Send deauth packets
            # ... attack code here ...
            
            packet_count += 10
            time.sleep(0.1)
            
            # Send progress every 100 packets
            if packet_count % 100 == 0:
                self.send_event("attack_progress", {
                    "packets_sent": packet_count,
                    "target": self.target_bssid
                })
        
        # Attack stopped
        self.send_event("attack_stopped", {
            "total_packets": packet_count
        })
        
    except Exception as e:
        self.send_event("attack_error", {"error": str(e)})
        self.is_attacking = False
```

### Example 3: Get Status (Request-Response)

**C# (Button Click):**
```csharp
private async void RefreshStatusButton_Click(object sender, EventArgs e)
{
    try
    {
        // Send command and wait for response
        var response = await Form1.Bridge.SendCommandAsync(
            "deauth",
            "get_status",
            waitForResponse: true
        );
        
        // Parse response
        var json = JsonSerializer.Serialize(response.data);
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        
        // Display status
        string status = $"Status:\n";
        status += $"Adapter Mode: {data["adapter_mode"]}\n";
        status += $"Is Scanning: {data["is_scanning"]}\n";
        status += $"Is Attacking: {data["is_attacking"]}\n";
        status += $"Access Points Found: {data["access_points_count"]}\n";
        
        MessageBox.Show(status, "Module Status");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}");
    }
}
```

**Python (Handler):**
```python
def handle_get_status(self, message):
    """Return current module status"""
    return {
        "adapter_mode": iface.ADAPTER_STATUS,
        "is_scanning": self.is_scanning,
        "is_attacking": self.is_attacking,
        "access_points_count": len(self.access_points),
        "devices_count": len(self.devices)
    }
```

---

## Message Type Quick Reference

| Type | C# Sends | Python Sends | When to Use |
|------|----------|--------------|-------------|
| COMMAND | ✓ | ✗ | C# tells Python to do something |
| RESPONSE | ✗ | ✓ | Python replies to a command |
| EVENT | ✗ | ✓ | Python sends unsolicited update |
| ERROR | ✗ | ✓ | Python reports an error |

---

## Summary

**C# → Python (Commands):**
```csharp
// Fire and forget
await Form1.Bridge.SendCommandAsync("module", "action");

// With data
await Form1.Bridge.SendCommandAsync("module", "action", data);

// Wait for response
var response = await Form1.Bridge.SendCommandAsync("module", "action", waitForResponse: true);
```

**Python → C# (Responses/Events):**
```python
# Return response (automatic)
return {"status": "success", "data": result}

# Send event
self.send_event("event_name", {"key": "value"})

// Send error
self.send_error("error message")
```

**C# Receive Events:**
```csharp
Form1.Bridge.EventReceived += (s, e) => {
    if (e.Message.module == "your_module") {
        // Handle event
    }
};
```
