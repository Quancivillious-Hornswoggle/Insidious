# Quick Start Guide - Insidious Multi-Module Architecture

## What Changed?

### Before ❌
- Single blocking socket - only one module could communicate at a time
- No message routing
- Hard to add new modules
- Windows couldn't receive updates simultaneously

### After ✅
- Message broker pattern - all modules communicate independently
- Automatic message routing
- Easy to add new modules
- Multiple windows receive real-time updates simultaneously

## Running Your Project

### 1. Start the Python Bridge
```bash
cd C:\Users\yello\Insidious
python python_bridge.py
```

Expected output:
```
Initializing server...
Server listening on 0.0.0.0:65535
```

### 2. Start the C# GUI
Open Visual Studio and run the project (F5)

Expected output in Debug console:
```
Connecting to Python bridge...
Connected successfully!
```

### 3. Open Module Windows
- Click "WiFi Deauth" to open deauth window
- Click "MITM Attack" to open MITM window
- Both work simultaneously!

## File Changes Summary

### New Files Created
```
Modules/
  ├── message_broker.py      # Central message routing system
  └── base_module.py         # Base class for all modules

Insidious GUI/Insidious GUI/
  └── BridgeManager.cs       # C# communication manager

ARCHITECTURE.md              # Detailed documentation
QUICKSTART.md               # This file
```

### Modified Files
```
python_bridge.py            # Updated to use message broker
Modules/deauth.py           # Converted to use BaseModule
Modules/mitm.py             # Converted to use BaseModule
Insidious GUI/Insidious GUI/
  ├── Form1.cs              # Updated to use BridgeManager
  ├── ModuleWindows/
      ├── Deauth.cs         # Updated for event-based communication
      └── Mitm.cs           # Updated for event-based communication
```

## How to Use in Your Code

### Sending Commands from C#

```csharp
// Fire and forget (don't wait for response)
await Form1.Bridge.SendCommandAsync("deauth", "scan_ap");

// Wait for response
var response = await Form1.Bridge.SendCommandAsync(
    "mitm", 
    "get_status", 
    waitForResponse: true
);

// Send with data
var data = new { target_ip = "192.168.1.100", gateway_ip = "192.168.1.1" };
await Form1.Bridge.SendCommandAsync("mitm", "start_poison", data);
```

### Receiving Events in C#

```csharp
public MyWindow()
{
    InitializeComponent();
    
    // Subscribe to events
    Form1.Bridge.EventReceived += Bridge_EventReceived;
}

private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
{
    // Filter for your module
    if (e.Message.module != "deauth")
        return;
    
    // Handle on UI thread
    if (InvokeRequired) {
        Invoke(new Action(() => HandleEvent(e.Message)));
    } else {
        HandleEvent(e.Message);
    }
}

private void HandleEvent(Message message)
{
    switch (message.action)
    {
        case "scan_complete":
            MessageBox.Show("Scan finished!");
            break;
        case "attack_progress":
            // Update UI with progress
            break;
    }
}

// IMPORTANT: Unsubscribe when closing
protected override void OnFormClosing(FormClosingEventArgs e)
{
    Form1.Bridge.EventReceived -= Bridge_EventReceived;
    base.OnFormClosing(e);
}
```

### Adding Command Handlers in Python

```python
class DeauthModule(BaseModule):
    def __init__(self):
        super().__init__("deauth")
        
        # Register handlers for actions
        self.register_handler("scan_ap", self.handle_scan_ap)
        self.register_handler("start_attack", self.handle_start_attack)
    
    def handle_scan_ap(self, message):
        # Start scan in background
        threading.Thread(target=self._scan_worker).start()
        
        # Return immediate response
        return {"status": "scanning_started"}
    
    def _scan_worker(self):
        # Do the actual work
        results = scan_for_access_points()
        
        # Send event when done
        self.send_event("scan_complete", {"access_points": results})
```

## Common Tasks

### Task 1: Add a Button That Sends a Command

**C# (in your window):**
```csharp
private async void MyButton_Click(object sender, EventArgs e)
{
    try
    {
        await Form1.Bridge.SendCommandAsync("deauth", "my_action");
        MessageBox.Show("Command sent!");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}");
    }
}
```

**Python (in deauth.py):**
```python
def __init__(self):
    super().__init__("deauth")
    self.register_handler("my_action", self.handle_my_action)

def handle_my_action(self, message):
    # Do something
    return {"status": "success"}
```

### Task 2: Send Real-Time Updates

**Python:**
```python
def _long_running_task(self):
    for i in range(100):
        # Do work
        time.sleep(0.1)
        
        # Send progress update every iteration
        self.send_event("progress_update", {"percent": i})
    
    # Send completion event
    self.send_event("task_complete", {"result": "success"})
```

**C#:**
```csharp
private void HandleEvent(Message message)
{
    if (message.action == "progress_update")
    {
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(
            JsonSerializer.Serialize(message.data)
        );
        int percent = Convert.ToInt32(data["percent"]);
        progressBar.Value = percent;
    }
    else if (message.action == "task_complete")
    {
        MessageBox.Show("Task completed!");
    }
}
```

### Task 3: Get Data from Python

**C#:**
```csharp
private async void GetDataButton_Click(object sender, EventArgs e)
{
    try
    {
        var response = await Form1.Bridge.SendCommandAsync(
            "deauth", 
            "get_access_points", 
            waitForResponse: true
        );
        
        // Parse the response
        var json = JsonSerializer.Serialize(response.data);
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        
        // Use the data
        var apList = data["access_points"];
        // ... update UI ...
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}");
    }
}
```

**Python:**
```python
def handle_get_access_points(self, message):
    return {
        "access_points": self.access_points,
        "count": len(self.access_points)
    }
```

## Converting Your Remaining Modules

You still need to convert:
- dns.py
- server.py
- packet_capture.py
- wifi_brute.py

### Template for Converting a Module

```python
from base_module import BaseModule
from message_broker import Message
import threading

class YourModule(BaseModule):
    def __init__(self):
        super().__init__("your_module_name")
        
        # Register handlers
        self.register_handler("action1", self.handle_action1)
        self.register_handler("action2", self.handle_action2)
    
    def on_start(self):
        """Called when module starts"""
        print(f"[{self.module_name}] Module initialized")
    
    def on_stop(self):
        """Called when module stops"""
        # Cleanup here
        pass
    
    def handle_action1(self, message):
        """Handle action1 command"""
        # For quick actions, return immediately
        return {"status": "success"}
    
    def handle_action2(self, message):
        """Handle action2 command"""
        # For long actions, start background thread
        threading.Thread(target=self._action2_worker).start()
        return {"status": "started"}
    
    def _action2_worker(self):
        """Background worker for action2"""
        try:
            # Do work
            result = do_something()
            
            # Send events
            self.send_event("action2_complete", {"result": result})
        except Exception as e:
            self.send_event("action2_error", {"error": str(e)})

# Global instance
_module = None

def get_module():
    global _module
    if _module is None:
        _module = YourModule()
    return _module
```

Then add to `python_bridge.py`:
```python
from Modules import your_module

modules = {
    'deauth': deauth.get_module(),
    'mitm': mitm.get_module(),
    'your_module': your_module.get_module(),  # Add this
}
```

## Testing Checklist

- [ ] Python bridge starts without errors
- [ ] C# GUI connects successfully
- [ ] Can open multiple module windows
- [ ] Sending commands works
- [ ] Receiving events works
- [ ] Multiple windows receive events simultaneously
- [ ] Closing windows doesn't crash
- [ ] Stopping attacks/scans works

## Debugging Tips

### See All Messages
**Python console shows:**
```
[Broker] Routed scan_ap to deauth
[deauth] Handling: scan_ap
[Broker] Sent scan_started from deauth
```

**C# Debug.WriteLine shows:**
```
Sent: deauth.scan_ap
Received: deauth.scan_started (RESP)
```

### Module Not Responding
1. Check module is registered in `python_bridge.py`
2. Check `module.start()` is called
3. Verify handler is registered: `self.register_handler(...)`
4. Check for exceptions in Python console

### Events Not Showing in C#
1. Verify subscription: `Form1.Bridge.EventReceived +=`
2. Check module name filter: `if (e.Message.module != "...")`
3. Check InvokeRequired for UI thread safety

## Next Steps

1. **Test the current setup** - Make sure deauth and mitm work
2. **Convert remaining modules** - Use the template above
3. **Add UI controls** - Connect buttons to actual data
4. **Implement actual functionality** - Replace TODO comments

## Need Help?

Common issues and solutions in `ARCHITECTURE.md` under "Troubleshooting" section.

---
**Quick Reference:**

| Task | C# Code | Python Code |
|------|---------|-------------|
| Send command | `await Form1.Bridge.SendCommandAsync("module", "action")` | Handler returns response |
| Get response | `await Form1.Bridge.SendCommandAsync("module", "action", waitForResponse: true)` | Handler returns data dict |
| Send event | N/A | `self.send_event("event_name", data)` |
| Subscribe | `Form1.Bridge.EventReceived +=` | N/A |
| Unsubscribe | `Form1.Bridge.EventReceived -=` | N/A |
