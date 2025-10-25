# Insidious - Multi-Module Communication Architecture

## Overview

This document explains the new message broker architecture that allows multiple GUI windows and Python modules to communicate simultaneously and efficiently.

## Architecture Components

### 1. Message Broker (Python)
**File:** `Modules/message_broker.py`

The central hub that routes messages between C# GUI and Python modules.

**Key Features:**
- Thread-safe message routing
- Dedicated queues for each module
- Asynchronous send/receive operations
- JSON-based message format

**Message Structure:**
```json
{
  "type": "CMD|RESP|EVENT|ERR",
  "module": "deauth|mitm|dns|etc",
  "action": "scan_ap|start_attack|etc",
  "data": {...},
  "msg_id": "unique-id"
}
```

### 2. Base Module (Python)
**File:** `Modules/base_module.py`

Abstract base class that all Python modules inherit from.

**Provides:**
- Automatic message queue handling
- Command handler registration
- Convenient methods for sending responses/events
- Background message processing loop

**Usage Example:**
```python
class MyModule(BaseModule):
    def __init__(self):
        super().__init__("my_module")
        self.register_handler("do_something", self.handle_do_something)
    
    def handle_do_something(self, message: Message):
        # Process command
        result = {"status": "success"}
        return result  # Automatically sent as response
```

### 3. Bridge Manager (C#)
**File:** `Insidious GUI/Insidious GUI/BridgeManager.cs`

Manages TCP connection and message routing in C#.

**Features:**
- Async connection management
- Request-response pattern with timeouts
- Event-based message handling
- Thread-safe communication

**Usage Example:**
```csharp
// Send command and don't wait
await Form1.Bridge.SendCommandAsync("deauth", "scan_ap");

// Send command and wait for response
var response = await Form1.Bridge.SendCommandAsync(
    "mitm", 
    "get_status", 
    waitForResponse: true
);

// Subscribe to events
Form1.Bridge.EventReceived += (s, e) => {
    if (e.Message.module == "deauth") {
        // Handle deauth events
    }
};
```

## Communication Patterns

### Pattern 1: Fire-and-Forget Commands
**Use Case:** Start a scan, begin an attack

```csharp
// C# sends command
await Form1.Bridge.SendCommandAsync("deauth", "scan_ap");
```

```python
# Python handler processes
def handle_scan_ap(self, message):
    # Start scanning in background thread
    threading.Thread(target=self._scan_worker).start()
    return {"status": "scan_started"}
```

### Pattern 2: Request-Response
**Use Case:** Get current status, query data

```csharp
// C# sends and waits for response
var response = await Form1.Bridge.SendCommandAsync(
    "mitm", 
    "get_status", 
    waitForResponse: true
);
var data = response.data;
```

```python
# Python handler returns data immediately
def handle_get_status(self, message):
    return {
        "is_running": self.is_running,
        "packets_captured": self.packet_count
    }
```

### Pattern 3: Event Broadcasting
**Use Case:** Progress updates, real-time notifications

```python
# Python sends unsolicited events
def _scan_worker(self):
    for progress in range(0, 100, 10):
        self.send_event("scan_progress", {"progress": progress})
        time.sleep(1)
    
    self.send_event("scan_complete", {"results": results})
```

```csharp
// C# subscribes to events
Form1.Bridge.EventReceived += (s, e) => {
    if (e.Message.action == "scan_progress") {
        UpdateProgressBar(e.Message.data);
    }
};
```

## How Multiple Windows Work Simultaneously

### The Problem (Old Architecture)
- Single blocking socket
- Only one module could listen at a time
- No message routing
- Windows couldn't receive updates simultaneously

### The Solution (New Architecture)

1. **Message Broker Routes Everything**
   - All messages go through the broker
   - Broker maintains separate queues for each module
   - Each module processes its own queue independently

2. **Each Module Runs in Its Own Thread**
   - Modules don't block each other
   - Background workers handle long-running tasks
   - Modules can send events anytime

3. **Each Window Subscribes to Events**
   - Windows filter events by module name
   - Multiple windows can listen simultaneously
   - No blocking or interference

### Example Flow

```
C# Deauth Window          Message Broker          Python Deauth Module
     |                          |                          |
     |-- scan_ap command ------>|                          |
     |                          |-- route to deauth ------>|
     |                          |                          |-- process
     |<-- scan_started ---------|<-- response -------------|
     |                          |                          |
     |                          |                          |-- scanning...
     |                          |                          |
     |<-- scan_progress --------|<-- event ----------------|
     |<-- scan_progress --------|<-- event ----------------|
     |<-- scan_complete --------|<-- event ----------------|


Meanwhile, MITM window can send commands to MITM module:

C# MITM Window            Message Broker          Python MITM Module
     |                          |                          |
     |-- start_poison --------->|                          |
     |                          |-- route to mitm -------->|
     |<-- attack_started -------|<-- response -------------|
     |                          |                          |
     |<-- attack_progress ------|<-- event ----------------|
```

## Adding a New Module

### Step 1: Create Python Module
```python
# Modules/my_new_module.py
from base_module import BaseModule
from message_broker import Message

class MyNewModule(BaseModule):
    def __init__(self):
        super().__init__("my_new")
        
        # Register handlers
        self.register_handler("do_action", self.handle_do_action)
    
    def on_start(self):
        print("Module started")
    
    def on_stop(self):
        print("Module stopped")
    
    def handle_do_action(self, message):
        # Do something
        result = {"status": "done"}
        
        # Send events if needed
        self.send_event("action_complete", {"result": "success"})
        
        return result

# Global instance
_module = None

def get_module():
    global _module
    if _module is None:
        _module = MyNewModule()
    return _module
```

### Step 2: Register in Bridge
```python
# python_bridge.py
from Modules import my_new_module

modules = {
    'deauth': deauth.get_module(),
    'mitm': mitm.get_module(),
    'my_new': my_new_module.get_module(),  # Add here
}
```

### Step 3: Create C# Window
```csharp
public partial class MyNewWindow : Form
{
    public MyNewWindow()
    {
        InitializeComponent();
        Form1.Bridge.EventReceived += Bridge_EventReceived;
    }
    
    private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
    {
        if (e.Message.module != "my_new")
            return;
        
        // Handle events
    }
    
    private async void DoActionButton_Click(object sender, EventArgs e)
    {
        await Form1.Bridge.SendCommandAsync("my_new", "do_action");
    }
}
```

## Best Practices

### Python Modules

1. **Always use background threads for long operations**
   ```python
   def handle_long_task(self, message):
       threading.Thread(target=self._long_task_worker).start()
       return {"status": "started"}
   ```

2. **Send progress events for long operations**
   ```python
   def _long_task_worker(self):
       for i in range(100):
           self.send_event("progress", {"percent": i})
           time.sleep(0.1)
   ```

3. **Handle errors gracefully**
   ```python
   def handle_risky_action(self, message):
       try:
           result = do_something()
           return {"status": "success", "result": result}
       except Exception as e:
           self.send_error(str(e))
           return {"status": "error"}
   ```

### C# Windows

1. **Always check InvokeRequired for UI updates**
   ```csharp
   if (InvokeRequired) {
       Invoke(new Action(() => UpdateUI(data)));
   } else {
       UpdateUI(data);
   }
   ```

2. **Unsubscribe from events on close**
   ```csharp
   protected override void OnFormClosing(FormClosingEventArgs e)
   {
       Form1.Bridge.EventReceived -= Bridge_EventReceived;
       base.OnFormClosing(e);
   }
   ```

3. **Use async/await for all bridge operations**
   ```csharp
   private async void Button_Click(object sender, EventArgs e)
   {
       try {
           await Form1.Bridge.SendCommandAsync("module", "action");
       } catch (Exception ex) {
           MessageBox.Show($"Error: {ex.Message}");
       }
   }
   ```

## Testing the Architecture

### 1. Start Python Bridge
```bash
cd C:\Users\yello\Insidious
python python_bridge.py
```

You should see:
```
Initializing server...
Server listening on 0.0.0.0:65535
```

### 2. Start C# GUI
Run the Visual Studio project. You should see:
```
Connecting to Python bridge...
Connected successfully!
[Broker] Registered module: deauth
[Broker] Registered module: mitm
Bridge is ready and listening for commands!
```

### 3. Test Communication
- Open Deauth window
- Click "Scan AP" button
- Watch console for messages:
  ```
  [Broker] Routed scan_ap to deauth
  [deauth] Handling: scan_ap
  [Broker] Sent scan_started from deauth
  ```

## Troubleshooting

### Connection Fails
- Ensure python_bridge.py is running first
- Check IP address in Form1.cs matches (default: 127.0.0.1)
- Check firewall isn't blocking port 65535

### Module Not Receiving Messages
- Verify module is registered in python_bridge.py
- Check module name matches exactly (case-sensitive)
- Ensure module.start() is called

### Events Not Received in C#
- Verify event subscription: `Form1.Bridge.EventReceived +=`
- Check module name filter in event handler
- Ensure InvokeRequired check for UI updates

### JSON Parsing Errors
- All data must be JSON-serializable
- Use simple types: strings, numbers, lists, dictionaries
- Avoid circular references

## Performance Considerations

- **Message Queue**: Each module has unlimited queue size (consider adding limits if needed)
- **Threading**: Each module runs in its own thread, plus broker has 2 threads (send/receive)
- **Network**: Messages are newline-delimited JSON over TCP
- **Latency**: Typical message round-trip: 1-5ms on localhost

## Future Enhancements

1. **Message Compression**: Add zlib compression for large data
2. **Authentication**: Add token-based authentication
3. **Encryption**: TLS/SSL for secure communication
4. **Multiple Connections**: Support multiple GUI instances
5. **Message Replay**: Log messages for debugging
6. **Health Checks**: Periodic ping/pong to detect disconnects

---

**Last Updated:** December 2024
**Architecture Version:** 2.0
