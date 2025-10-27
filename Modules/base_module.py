"""
Base Module Class - All modules should inherit from this
Provides common functionality for message handling and communication
"""

import queue
import threading
from abc import ABC, abstractmethod
from typing import Callable, Dict
from message_broker import Message, MessageType, get_broker

class BaseModule(ABC):
    """
    Base class for all modules
    
    Provides:
    - Automatic message queue handling
    - Command registration and routing
    - Convenient methods for sending responses/events
    """
    
    def __init__(self, module_name: str):
        self.module_name = module_name
        self.broker = get_broker()
        
        # Get our dedicated message queue from broker
        self.message_queue = self.broker.register_module(module_name)
        
        # Command handlers
        self.handlers: Dict[str, Callable] = {}
        
        # Module running state
        self.running = False
        
        # Processing thread
        self.process_thread = None
    
    def register_handler(self, action: str, handler: Callable):
        """
        Register a handler function for a specific action
        
        Args:
            action: Action name (e.g., 'scan_ap', 'start', 'stop')
            handler: Function to call when action is received
                     Should accept (message: Message) and return response data or None
        """
        self.handlers[action] = handler
        print(f"[{self.module_name}] Registered handler: {action}")
    
    def start(self):
        """Start the module's message processing loop"""
        if self.running:
            return
        
        self.running = True
        self.process_thread = threading.Thread(target=self._process_loop, daemon=True)
        self.process_thread.start()
        print(f"[{self.module_name}] Module started")
        
        # Call module-specific initialization
        self.on_start()
    
    def stop(self):
        """Stop the module"""
        self.running = False
        self.on_stop()
        print(f"[{self.module_name}] Module stopped")
    
    def _process_loop(self):
        """Main message processing loop"""
        while self.running:
            try:
                # Wait for message with timeout
                message = self.message_queue.get(timeout=1.0)
                
                # Process the message
                self._handle_message(message)
                
            except queue.Empty:
                # No message, continue loop
                continue
            except Exception as e:
                print(f"[{self.module_name}] Processing error: {e}")
    
    def _handle_message(self, message: Message):
        """Handle incoming message by routing to appropriate handler"""
        print(f"[{self.module_name}] Handling: {message.action}")
        
        try:
            if message.action in self.handlers:
                # Call the registered handler
                response_data = self.handlers[message.action](message)
                
                # Send response if handler returned data
                if response_data is not None:
                    self.send_response(message.action, response_data, message.msg_id)
            else:
                print(f"[{self.module_name}] No handler for: {message.action}")
                self.send_error(f"Unknown action: {message.action}", message.msg_id)
                
        except Exception as e:
            print(f"[{self.module_name}] Handler error: {e}")
            self.send_error(str(e), message.msg_id)
    
    def send_response(self, action: str, data: any, msg_id: str = None):
        """Send a response message back to GUI"""
        message = Message(
            MessageType.RESPONSE,
            self.module_name,
            action,
            data,
            msg_id
        )
        self.broker.send_message(message)
    
    def send_event(self, action: str, data: any):
        """Send an event (unsolicited update) to GUI"""
        message = Message(
            MessageType.EVENT,
            self.module_name,
            action,
            data
        )
        self.broker.send_message(message)
    
    def send_error(self, error: str, msg_id: str = None):
        """Send an error message to GUI"""
        message = Message(
            MessageType.ERROR,
            self.module_name,
            "error",
            {"error": error},
            msg_id
        )
        self.broker.send_message(message)
    
    # Abstract methods that modules should implement
    
    @abstractmethod
    def on_start(self):
        """Called when module starts - override to initialize"""
        pass
    
    @abstractmethod
    def on_stop(self):
        """Called when module stops - override to cleanup"""
        pass
