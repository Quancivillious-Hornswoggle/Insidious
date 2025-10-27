# 🚀 Ultra-Quick Copy-Paste Guide

## I want to... (from C#)

### ➤ Send a simple command
```csharp
await Form1.Bridge.SendCommandAsync("deauth", "scan_ap");
```

### ➤ Send a command with data
```csharp
var data = new { bssid = "AA:BB:CC:DD:EE:FF" };
await Form1.Bridge.SendCommandAsync("deauth", "scan_devices", data);
```

### ➤ Get data from Python
```csharp
var response = await Form1.Bridge.SendCommandAsync("deauth", "get_status", waitForResponse: true);
var json = JsonSerializer.Serialize(response.data);
var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
```

### ➤ Listen for events
```csharp
// In constructor
Form1.Bridge.EventReceived += Bridge_EventReceived;

// Event handler
private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
{
    if (e.Message.module != "deauth") return;
    
    if (InvokeRequired)
        Invoke(new Action(() => HandleEvent(e.Message)));
    else
        HandleEvent(e.Message);
}
```

---

## I want to... (from Python)

### ➤ Handle a command
```python
# In __init__
self.register_handler("my_action", self.handle_my_action)

# Handler
def handle_my_action(self, message):
    return {"status": "success"}
```

### ➤ Send an event to C#
```python
self.send_event("scan_complete", {"results": results})
```

### ➤ Do long task in background
```python
def handle_scan(self, message):
    threading.Thread(target=self._scan_worker, daemon=True).start()
    return {"status": "started"}

def _scan_worker(self):
    # do work
    self.send_event("scan_complete", {"results": results})
```

### ➤ Get data from command
```python
def handle_action(self, message):
    value = message.data.get("key")
    return {"received": value}
```

---

## Complete Working Example

### C# Window
```csharp
public partial class MyWindow : Form
{
    public MyWindow()
    {
        InitializeComponent();
        Form1.Bridge.EventReceived += Bridge_EventReceived;
    }
    
    private async void ScanButton_Click(object sender, EventArgs e)
    {
        await Form1.Bridge.SendCommandAsync("deauth", "scan_ap");
    }
    
    private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
    {
        if (e.Message.module != "deauth") return;
        
        if (InvokeRequired)
            Invoke(new Action(() => HandleEvent(e.Message)));
        else
            HandleEvent(e.Message);
    }
    
    private void HandleEvent(Message message)
    {
        if (message.action == "scan_complete")
        {
            MessageBox.Show("Scan done!");
        }
    }
    
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        Form1.Bridge.EventReceived -= Bridge_EventReceived;
        base.OnFormClosing(e);
    }
}
```

### Python Module
```python
class MyModule(BaseModule):
    def __init__(self):
        super().__init__("deauth")
        self.register_handler("scan_ap", self.handle_scan_ap)
    
    def on_start(self):
        print("Module started")
    
    def on_stop(self):
        print("Module stopped")
    
    def handle_scan_ap(self, message):
        threading.Thread(target=self._scan_worker, daemon=True).start()
        return {"status": "scanning"}
    
    def _scan_worker(self):
        time.sleep(2)  # simulate work
        self.send_event("scan_complete", {"results": ["AP1", "AP2"]})
```

---

## That's It!

Three files to remember:
1. **COMMAND_REFERENCE.md** - Detailed examples
2. **QUICKSTART.md** - Step-by-step guide  
3. **ARCHITECTURE.md** - How it all works

Run `python test_architecture.py` to verify everything works!
