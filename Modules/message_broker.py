"""
Message Broker - Central communication hub for routing messages between
C# GUI and Python modules
"""

import queue
import threading
import json
from typing import Dict, Callable, Any
from enum import Enum

class MessageType(Enum):
    """Defines types of messages that can be sent"""
    COMMAND = "CMD"
    RESPONSE = "RESP"
    EVENT = "EVENT"
    ERROR = "ERR"

class Message:
    """Structured message format"""
    def __init__(self, msg_type: MessageType, module: str, action: str, data: Any = None, msg_id: str = None):
        self.type = msg_type
        self.module = module
        self.action = action
        self.data = data
        self.msg_id = msg_id or self._generate_id()
    
    def _generate_id(self):
        import uuid
        return str(uuid.uuid4())[:8]
    
    def to_json(self) -> str:
        """Convert message to JSON string for transmission"""
        return json.dumps({
            'type': self.type.value,
            'module': self.module,
            'action': self.action,
            'data': self.data,
            'msg_id': self.msg_id
        })
    
    @staticmethod
    def from_json(json_str: str) -> 'Message':
        """Parse JSON string to Message object"""
        data = json.loads(json_str)
        return Message(
            MessageType(data['type']),
            data['module'],
            data['action'],
            data.get('data'),
            data.get('msg_id')
        )

class MessageBroker:
    """
    Central message routing system
    - Receives messages from socket
    - Routes to appropriate module queue
    - Sends module responses back through socket
    """
    
    def __init__(self):
        # Module queues for incoming messages
        self.module_queues: Dict[str, queue.Queue] = {}
        
        # Outgoing message queue (to C# GUI)
        self.outgoing_queue = queue.Queue()
        
        # Lock for thread-safe operations
        self.lock = threading.Lock()
        
        # Socket reference
        self.socket = None
        
        # Keep broker running
        self.running = False
        
        # Threads
        self.receive_thread = None
        self.send_thread = None
    
    def register_module(self, module_name: str) -> queue.Queue:
        """
        Register a module and get its dedicated message queue
        
        Args:
            module_name: Name of the module (e.g., 'deauth', 'mitm')
            
        Returns:
            Queue for this module's incoming messages
        """
        with self.lock:
            if module_name not in self.module_queues:
                self.module_queues[module_name] = queue.Queue()
                print(f"[Broker] Registered module: {module_name}")
            return self.module_queues[module_name]
    
    def start(self, sock):
        """Start the broker with the given socket"""
        self.socket = sock
        self.running = True
        
        # Start receiving thread
        self.receive_thread = threading.Thread(target=self._receive_loop, daemon=True)
        self.receive_thread.start()
        
        # Start sending thread
        self.send_thread = threading.Thread(target=self._send_loop, daemon=True)
        self.send_thread.start()
        
        print("[Broker] Message broker started")
    
    def stop(self):
        """Stop the broker"""
        self.running = False
        print("[Broker] Message broker stopped")
    
    def _receive_loop(self):
        """
        Continuously receive messages from socket and route to modules
        """
        buffer = ""
        
        while self.running:
            try:
                # Receive data from socket
                data = self.socket.recv(4096).decode('utf-8')
                
                if not data:
                    print("[Broker] Connection closed")
                    break
                
                buffer += data
                
                # Process complete messages (assuming newline delimiter)
                while '\n' in buffer:
                    line, buffer = buffer.split('\n', 1)
                    if line.strip():
                        self._route_message(line.strip())
                        
            except ConnectionResetError:
                print("[Broker] Connection reset")
                break
            except Exception as e:
                print(f"[Broker] Receive error: {e}")
                if not self.running:
                    break
    
    def _route_message(self, raw_message: str):
        """Route incoming message to appropriate module queue"""
        try:
            message = Message.from_json(raw_message)
            
            with self.lock:
                if message.module in self.module_queues:
                    self.module_queues[message.module].put(message)
                    print(f"[Broker] Routed {message.action} to {message.module}")
                else:
                    print(f"[Broker] Unknown module: {message.module}")
                    # Send error back
                    error_msg = Message(
                        MessageType.ERROR,
                        message.module,
                        "unknown_module",
                        {"error": f"Module '{message.module}' not registered"},
                        message.msg_id
                    )
                    self.send_message(error_msg)
                    
        except json.JSONDecodeError as e:
            print(f"[Broker] Invalid JSON: {e}")
        except Exception as e:
            print(f"[Broker] Routing error: {e}")
    
    def _send_loop(self):
        """
        Continuously send messages from outgoing queue to socket
        """
        while self.running:
            try:
                # Block for up to 1 second waiting for message
                message = self.outgoing_queue.get(timeout=1.0)
                
                # Send message with newline delimiter
                json_msg = message.to_json() + '\n'
                self.socket.sendall(json_msg.encode('utf-8'))
                print(f"[Broker] Sent {message.action} from {message.module}")
                
            except queue.Empty:
                continue
            except Exception as e:
                print(f"[Broker] Send error: {e}")
                if not self.running:
                    break
    
    def send_message(self, message: Message):
        """
        Send a message from a module back to the C# GUI
        
        Args:
            message: Message object to send
        """
        self.outgoing_queue.put(message)

# Global singleton instance
_broker_instance = None

def get_broker() -> MessageBroker:
    """Get the global message broker instance"""
    global _broker_instance
    if _broker_instance is None:
        _broker_instance = MessageBroker()
    return _broker_instance
