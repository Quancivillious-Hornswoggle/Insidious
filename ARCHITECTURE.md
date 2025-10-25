# Insidious - Architecture Documentation

## Overview
This project uses a **message broker pattern** to enable efficient communication between a C# GUI and Python backend modules. Multiple windows can simultaneously interact with different Python modules without conflicts.

## Architecture Components

### 1. Python Backend

#### Message Broker (`Modules/message_broker.py`)
- **Central routing hub** for all messages
- Maintains dedicated queues for each module
- Handles bidirectional communication (C# ↔ Python)
- Thread-safe message routing
- Supports multiple message types: COMMAND, RESPONSE, EVENT, ERROR

**Key Features:**
- Newline-delimited JSON messages
- Automatic message routing to correct module
- Outgoing message queue for responses/events
- Non-blocking communication

#### Base Module (`Modules/base_module.py`)
- **Abstract base class** for all Python modules
- Provides common functionality:
  - Automatic message queue handling
  - Command handler registration
  - Convenient methods for sending responses/events
  - Background message processing loop

**Module Lifecycle:**
1. Initialize with module name
2. Register command handlers
3. Start processing loop
4. Handle incoming commands
5. Send responses/events back to GUI

#### Example Module Structure (deauth.py, mitm.py)
```python
from base_module import BaseModule
from message_broker import Message

class MyModule(BaseModule):
    def __init__(self):
        super().__init__("module_name")
        
        # Register handlers
        self.register_handler("command_name", self.handle_command)
    
    def on_start(self):
        # Module initialization
        pass
    
    def on_stop(self):
        # Cleanup
        pass
    
    def handle_command(self, message: Message):
        # Process command
        # Return data for response (optional)
        return {"status": "success"}
```

### 2. C# Frontend

#### BridgeManager (`BridgeManager.cs`)
- **Communication manager** for Python bridge
- Handles TCP connection
- Sends commands and receives responses/events
- Thread-safe message handling
- Event-driven architecture

**Key Features:**
- Async/await pattern for commands
- Optional wait-for-response mechanism
- Event subscriptions for real-time updates
- Automatic JSON serialization/deserialization

#### Message Format
All messages use JSON with this structure:
```json
{
  "type": "CMD|RESP|EVENT|ERR",
  "module": "deauth|mitm|etc",
  "action": "scan_ap|start_poison|etc",
  "data": { /* action-specific data */ },
  "msg_id": "unique-id"
}
```

### 3. Communication Flow

#### Command Flow (Request-Response)
```
C# Window                BridgeManager              Python Bridge              Module
    |                          |                          |                       |
    |--SendCommandAsync()----->|                          |                       |
    |                          |--JSON Message---------->|                       |
    |                          |                          |--Route to Queue----->|
    |                          |                          |                       |
    |                          |                          |<--Process Command----|
    |                          |<--JSON Response---------|<--Return Data---------|
    |<--Task Completes---------|                          |                       |
```

#### Event Flow (Unsolicited Updates)
```
Module                    Python Bridge              BridgeManager            C# Window
    |                          |                          |                       |
    |--send_event()---------->|                          |                       |
    |                          |--JSON Event------------>|                       |
    |                          |                          |--Fire Event--------->|
    |                          |                          |                       |
    |                          |                          |           (Update UI)|
```

## Message Types

### COMMAND (CMD)
Sent from C# to Python to execute actions
```csharp
await Bridge.SendCommandAsync("deauth", "scan_ap");
```

### RESPONSE (RESP)
Python module's response to a command
```python
return {"status": "success", "data": results}
```

### EVENT (EVENT)
Unsolicited updates from Python modules
```python
self.send_event("scan_progress", {"progress": 50})
```

### ERROR (ERR)
Error messages
```python
self.send_error("Invalid parameters")
```

## Usage Examples

### Python Module: Registering Handlers

```python
class DeauthModule(BaseModule):
    def __init__(self):
        super().__init__("deauth")
        
        # Register handlers for different commands
        self.register_handler("scan_ap", self.handle_scan_ap)
        self.register_handler("stop_attack", self.handle_stop_attack)
    
    def handle_scan_ap(self, message: Message):
        # Start background scan
        threading.Thread(target=self._scan_worker, daemon=True).start()
        
        # Return immediate response
        return {"status": "scan_started"}
    
    def _scan_worker(self):
        # Do actual work
        results = perform_scan()
        
        # Send event when complete
        self.send_event("scan_complete", {"access_points": results})
```

### C# Window: Sending Commands

```csharp
public partial class Deauth : Form
{
    public Deauth()
    {
        InitializeComponent();
        
        // Subscribe to events from deauth module
        Form1.Bridge.EventReceived += Bridge_EventReceived;
    }
    
    private async void scanButton_Click(object sender, EventArgs e)
    {
        // Send command (fire and forget)
        await Form1.Bridge.SendCommandAsync("deauth", "scan_ap");
        
        // OR wait for response
        var response = await Form1.Bridge.SendCommandAsync(
            "deauth", 
            "get_status", 
            null, 
            waitForResponse: true
        );
    }
    
    private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
    {
        // Filter for this module
        if (e.Message.module != "deauth")
            return;
        
        // Handle on UI thread
        if (InvokeRequired)
            Invoke(new Action(() => HandleEvent(e.Message)));
        else
            HandleEvent(e.Message);
    }
    
    private void HandleEvent(Message message)
    {
        switch (message.action)
        {
            case "scan_complete":
                UpdateUIWithResults(message.data);
                break;
        }
    }
}
```

## Benefits of This Architecture

### ✅ **Concurrent Communication**
- Multiple windows can communicate with different modules simultaneously
- No blocking - all communication is asynchronous

### ✅ **Scalability**
- Easy to add new modules - just inherit from BaseModule
- Easy to add new commands - just register a handler

### ✅ **Thread Safety**
- Each module has its own message queue
- No race conditions or shared state issues

### ✅ **Real-time Updates**
- Modules can send events at any time
- Windows get instant updates without polling

### ✅ **Error Handling**
- Centralized error routing
- Timeouts for responses
- Graceful degradation

### ✅ **Debugging**
- All messages are logged
- Clear message flow
- Easy to trace issues

## Best Practices

### Python Modules
1. **Always inherit from BaseModule**
2. **Register all handlers in __init__**
3. **Use background threads for long operations**
4. **Send events for progress updates**
5. **Return data from handlers for immediate responses**

### C# Windows
1. **Subscribe to EventReceived in constructor**
2. **Unsubscribe in OnFormClosing**
3. **Filter events by module name**
4. **Always handle events on UI thread (Invoke)**
5. **Use async/await for all commands**

### Communication
1. **Use descriptive action names** (scan_ap, not scan1)
2. **Include all necessary data** in command data object
3. **Send progress events** for long-running operations
4. **Handle errors gracefully** in both Python and C#

## Adding a New Module

### 1. Create Python Module
```python
# Modules/new_module.py
from base_module import BaseModule

class NewModule(BaseModule):
    def __init__(self):
        super().__init__("new_module")
        self.register_handler("do_something", self.handle_do_something)
    
    def on_start(self):
        print("New module started")
    
    def on_stop(self):
        print("New module stopped")
    
    def handle_do_something(self, message):
        # Do work
        return {"result": "success"}

_module = None
def get_module():
    global _module
    if _module is None:
        _module = NewModule()
    return _module
```

### 2. Register in Bridge
```python
# python_bridge.py
from Modules import new_module

modules = {
    'deauth': deauth.get_module(),
    'mitm': mitm.get_module(),
    'new_module': new_module.get_module(),  # Add here
}
```

### 3. Create C# Window
```csharp
public partial class NewModuleWindow : Form
{
    public NewModuleWindow()
    {
        InitializeComponent();
        Form1.Bridge.EventReceived += Bridge_EventReceived;
    }
    
    private async void doSomethingButton_Click(object sender, EventArgs e)
    {
        var data = new { param1 = "value" };
        await Form1.Bridge.SendCommandAsync("new_module", "do_something", data);
    }
    
    private void Bridge_EventReceived(object sender, MessageReceivedEventArgs e)
    {
        if (e.Message.module == "new_module")
        {
            // Handle events
        }
    }
}
```

## Troubleshooting

### Connection Issues
- Ensure python_bridge.py is running before starting GUI
- Check port 65535 is not blocked
- Verify IP address (127.0.0.1 or actual IP)

### Messages Not Routing
- Check module name matches exactly (case-sensitive)
- Verify handler is registered
- Check broker is started with socket

### Events Not Received
- Ensure window is subscribed to EventReceived
- Check module name filter
- Verify Invoke() is used for UI updates

### Timeouts
- Long operations should return immediately and send events later
- Don't block in command handlers
- Use background threads for work

## File Structure
```
Insidious/
├── python_bridge.py              # Main bridge entry point
├── Modules/
│   ├── message_broker.py         # Central message routing
│   ├── base_module.py            # Base class for modules
│   ├── deauth.py                 # Deauth module
│   ├── mitm.py                   # MITM module
│   └── ...                       # Other modules
└── Insidious GUI/
    └── Insidious GUI/
        ├── BridgeManager.cs      # C# communication manager
        ├── Form1.cs              # Main form
        └── ModuleWindows/
            ├── Deauth.cs         # Deauth window
            ├── Mitm.cs           # MITM window
            └── ...               # Other windows
```

## Performance Considerations

- Each module runs in its own thread
- Message queues prevent blocking
- JSON parsing is fast for small messages
- TCP socket provides reliable delivery
- Consider batching updates for high-frequency events

## Security Notes

- Communication is currently unencrypted (localhost only)
- Add authentication for remote connections
- Validate all input data in command handlers
- Sanitize data before sending to GUI
