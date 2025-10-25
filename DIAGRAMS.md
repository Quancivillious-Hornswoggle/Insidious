# System Architecture Diagram

## High-Level Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        C# GUI Application                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐         │
│  │ Main Window  │  │ Deauth Win   │  │  MITM Win    │         │
│  │   (Form1)    │  │              │  │              │         │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘         │
│         │                  │                  │                  │
│         └──────────────────┼──────────────────┘                  │
│                            │                                     │
│                     ┌──────▼──────┐                             │
│                     │ BridgeManager│                             │
│                     │              │                             │
│                     │ - Send/Recv  │                             │
│                     │ - Events     │                             │
│                     │ - Responses  │                             │
│                     └──────┬───────┘                             │
└────────────────────────────┼─────────────────────────────────────┘
                             │
                    TCP Socket (Port 65535)
                             │
┌────────────────────────────▼─────────────────────────────────────┐
│                     Python Bridge Process                         │
├──────────────────────────────────────────────────────────────────┤
│                                                                   │
│                     ┌──────────────────┐                         │
│                     │  Message Broker  │                         │
│                     │                  │                         │
│                     │ - Route messages │                         │
│                     │ - Manage queues  │                         │
│                     │ - Send/Recv loop │                         │
│                     └────────┬─────────┘                         │
│                              │                                    │
│          ┌───────────────────┼───────────────────┐               │
│          │                   │                   │               │
│    ┌─────▼─────┐      ┌─────▼─────┐      ┌─────▼─────┐        │
│    │  Deauth   │      │   MITM    │      │    DNS    │        │
│    │  Module   │      │  Module   │      │  Module   │        │
│    │           │      │           │      │           │        │
│    │ - Queue   │      │ - Queue   │      │ - Queue   │        │
│    │ - Thread  │      │ - Thread  │      │ - Thread  │        │
│    │ - Handlers│      │ - Handlers│      │ - Handlers│        │
│    └───────────┘      └───────────┘      └───────────┘        │
│                                                                   │
└───────────────────────────────────────────────────────────────────┘
```

## Message Flow Diagram

### Scenario: User clicks "Scan AP" button

```
┌──────────────┐         ┌──────────────┐         ┌──────────────┐
│ Deauth Window│         │    Broker    │         │Deauth Module │
└──────┬───────┘         └──────┬───────┘         └──────┬───────┘
       │                        │                        │
       │  1. SendCommandAsync   │                        │
       │  ("deauth","scan_ap")  │                        │
       ├───────────────────────►│                        │
       │                        │                        │
       │                        │  2. Route to queue     │
       │                        ├───────────────────────►│
       │                        │                        │
       │                        │                        │  3. Process
       │                        │                        │     command
       │                        │                        ├──┐
       │                        │                        │  │
       │                        │  4. Response           │◄─┘
       │                        │     {"status":"started"}│
       │  5. Response event     │◄───────────────────────┤
       │◄───────────────────────┤                        │
       │                        │                        │
       │                        │                        │  6. Scanning
       │                        │                        │     (background)
       │                        │                        ├──┐
       │                        │                        │  │
       │                        │  7. Progress event     │◄─┘
       │                        │◄───────────────────────┤
       │  8. Event received     │                        │
       │◄───────────────────────┤                        │
       │  (update progress bar) │                        │
       │                        │                        │
       │                        │  9. Complete event     │
       │                        │◄───────────────────────┤
       │  10. Event received    │                        │
       │◄───────────────────────┤                        │
       │  (display results)     │                        │
       │                        │                        │
```

## Threading Model

```
┌─────────────────────────────────────────────────────────────────┐
│                          Python Process                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Main Thread                                                     │
│  └─► Accepts connection                                         │
│      Starts broker                                              │
│      Monitors connection                                         │
│                                                                  │
│  Broker Receive Thread (Daemon)                                 │
│  └─► Continuously reads from socket                             │
│      Parses messages                                            │
│      Routes to module queues                                    │
│                                                                  │
│  Broker Send Thread (Daemon)                                    │
│  └─► Continuously reads from outgoing queue                     │
│      Sends messages to socket                                   │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ Module Threads (One per module, Daemon)                  │   │
│  ├─────────────────────────────────────────────────────────┤   │
│  │                                                           │   │
│  │  Deauth Thread                                           │   │
│  │  └─► Reads from deauth queue                             │   │
│  │      Calls registered handlers                           │   │
│  │      Sends responses/events                              │   │
│  │                                                           │   │
│  │  MITM Thread                                             │   │
│  │  └─► Reads from mitm queue                               │   │
│  │      Calls registered handlers                           │   │
│  │      Sends responses/events                              │   │
│  │                                                           │   │
│  │  DNS Thread                                              │   │
│  │  └─► Reads from dns queue                                │   │
│  │      Calls registered handlers                           │   │
│  │      Sends responses/events                              │   │
│  │                                                           │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ Worker Threads (Created as needed, Daemon)               │   │
│  ├─────────────────────────────────────────────────────────┤   │
│  │                                                           │   │
│  │  AP Scan Worker                                          │   │
│  │  └─► Scans for access points                             │   │
│  │      Sends progress events                               │   │
│  │      Sends completion event                              │   │
│  │                                                           │   │
│  │  Device Scan Worker                                      │   │
│  │  └─► Scans for devices                                   │   │
│  │      Sends events                                        │   │
│  │                                                           │   │
│  │  Deauth Attack Worker                                    │   │
│  │  └─► Sends deauth packets                                │   │
│  │      Sends progress events                               │   │
│  │                                                           │   │
│  │  MITM Poison Worker                                      │   │
│  │  └─► ARP poisoning                                       │   │
│  │      Captures packets                                    │   │
│  │      Sends events                                        │   │
│  │                                                           │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                          C# Process                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  UI Thread                                                       │
│  └─► Handles all UI interactions                                │
│      Button clicks                                              │
│      Updates UI elements                                        │
│      Invokes methods on correct thread                          │
│                                                                  │
│  Bridge Receive Thread (Background)                             │
│  └─► Continuously reads from socket                             │
│      Parses JSON messages                                       │
│      Fires events on UI thread                                  │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

## Data Flow Example

### Multiple Windows Receiving Simultaneous Updates

```
Time: T0
─────────────────────────────────────────────────────────────────

User opens Deauth window → Subscribes to events
User opens MITM window → Subscribes to events


Time: T1
─────────────────────────────────────────────────────────────────

Deauth Window: Click "Scan AP"
   │
   └─► BridgeManager.SendCommandAsync("deauth", "scan_ap")
          │
          └─► TCP Socket → Message Broker → Deauth Module Queue
                                                   │
                                             Process in Deauth Thread


Time: T2 (simultaneously)
─────────────────────────────────────────────────────────────────

MITM Window: Click "Start Poison"
   │
   └─► BridgeManager.SendCommandAsync("mitm", "start_poison")
          │
          └─► TCP Socket → Message Broker → MITM Module Queue
                                                   │
                                             Process in MITM Thread


Time: T3-T10
─────────────────────────────────────────────────────────────────

Deauth Module (Background Worker):
   │
   ├─► Send Event: scan_progress (25%)
   │      │
   │      └─► Message Broker → TCP Socket → BridgeManager
   │                                             │
   │                                    EventReceived fires
   │                                             │
   │                              ┌──────────────┴───────────┐
   │                              │                          │
   │                        Deauth Window             MITM Window
   │                       (module=="deauth")        (module!="deauth")
   │                          Updates UI                 Ignores
   │
   ├─► Send Event: scan_progress (50%)
   │      └─► [Same flow as above]
   │
   └─► Send Event: scan_complete
          └─► [Same flow as above]


MITM Module (Background Worker):
   │
   ├─► Send Event: attack_progress (packets: 100)
   │      │
   │      └─► Message Broker → TCP Socket → BridgeManager
   │                                             │
   │                                    EventReceived fires
   │                                             │
   │                              ┌──────────────┴───────────┐
   │                              │                          │
   │                        Deauth Window             MITM Window
   │                       (module!="mitm")          (module=="mitm")
   │                          Ignores                   Updates UI
   │
   ├─► Send Event: attack_progress (packets: 200)
   │      └─► [Same flow as above]
   │
   └─► Continues poisoning...


Result: Both windows update independently without blocking!
```

## Queue System Detail

```
┌──────────────────────────────────────────────────────────────┐
│                      Message Broker                           │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  Module Queues (thread-safe):                                │
│                                                               │
│  deauth_queue:  [msg1] [msg2] [msg3] ────► Deauth Thread    │
│                                                               │
│  mitm_queue:    [msg1] [msg2] ────────────► MITM Thread     │
│                                                               │
│  dns_queue:     [msg1] ───────────────────► DNS Thread      │
│                                                               │
│                                                               │
│  Outgoing Queue (thread-safe):                               │
│                                                               │
│  outgoing:      [event1] [resp1] [event2] ─► Send Thread    │
│                                                │              │
│                                                └──► Socket    │
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

## Module State Machines

### Deauth Module States

```
     ┌─────────┐
     │  IDLE   │
     └────┬────┘
          │
          │ scan_ap command
          ▼
     ┌─────────┐
     │SCANNING │◄───────┐
     └────┬────┘        │
          │             │ (continues scanning)
          │ results     │
          ▼             │
     ┌─────────┐        │
     │  READY  │────────┘
     └────┬────┘
          │
          │ deauth_all/deauth_single
          ▼
     ┌─────────┐
     │ATTACKING│◄───────┐
     └────┬────┘        │
          │             │ (continues attacking)
          │ stop_attack │
          ▼             │
     ┌─────────┐        │
     │STOPPING │────────┘
     └────┬────┘
          │
          ▼
     ┌─────────┐
     │  IDLE   │
     └─────────┘
```

## Key Benefits of This Architecture

```
┌────────────────────────────────────────────────────────────┐
│                         BENEFITS                            │
├────────────────────────────────────────────────────────────┤
│                                                             │
│  ✓ Non-blocking Communication                              │
│    └─► Multiple modules work simultaneously                │
│                                                             │
│  ✓ Scalable                                                │
│    └─► Easy to add new modules                             │
│                                                             │
│  ✓ Decoupled                                               │
│    └─► Modules don't know about each other                 │
│                                                             │
│  ✓ Event-driven                                            │
│    └─► Real-time updates to all windows                    │
│                                                             │
│  ✓ Thread-safe                                             │
│    └─► No race conditions or deadlocks                     │
│                                                             │
│  ✓ Type-safe Messages                                      │
│    └─► Structured JSON with validation                     │
│                                                             │
│  ✓ Request-Response Pattern                                │
│    └─► Can wait for responses when needed                  │
│                                                             │
│  ✓ Error Handling                                          │
│    └─► Graceful error propagation                          │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## Performance Characteristics

```
Typical Latencies (localhost):
─────────────────────────────────
Command → Response:     1-5 ms
Event delivery:         1-3 ms
Message parsing:       <1 ms

Thread overhead:
─────────────────────────────────
Base threads:           3 (Main + Broker Send + Broker Receive)
Module threads:         N (one per module)
Worker threads:         Dynamic (created as needed)

Memory usage:
─────────────────────────────────
Per module queue:       ~1 KB (empty)
Per message:           ~500 bytes (JSON)
Total overhead:        ~5-10 MB

Network bandwidth:
─────────────────────────────────
Per message:           ~200-1000 bytes
Peak throughput:       ~10,000 msgs/sec (theoretical)
Typical usage:         ~10-100 msgs/sec
```
