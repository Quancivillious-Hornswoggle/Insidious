"""
Network Deauthentication Module
Deauthenticates specified or all clients on a network
"""

import sys
import os
sys.path.insert(0, os.path.dirname(__file__))

from Network_Modules import network_adapter as iface
from base_module import BaseModule
from message_broker import Message
import time
import threading

class DeauthModule(BaseModule):
    def __init__(self):
        super().__init__("deauth")
        
        # Module state
        self.is_scanning = False
        self.access_points = []
        self.devices = []
        self.is_attacking = False
        
        # Register all command handlers
        self.register_handler("scan_ap", self.handle_scan_ap)
        self.register_handler("scan_devices", self.handle_scan_devices)
        self.register_handler("deauth_all", self.handle_deauth_all)
        self.register_handler("deauth_single", self.handle_deauth_single)
        self.register_handler("stop_attack", self.handle_stop_attack)
        self.register_handler("get_status", self.handle_get_status)
    
    def on_start(self):
        """Initialize module"""
        print(f"[{self.module_name}] Deauth module initialized")
    
    def on_stop(self):
        """Cleanup module"""
        if self.is_attacking:
            self.stop_attack()
        if iface.ADAPTER_STATUS == "monitor":
            iface.set_adapter_status("managed")
    
    # Command Handlers
    
    def handle_scan_ap(self, message: Message):
        """Scan for access points"""
        if self.is_scanning:
            return {"status": "already_scanning"}
        
        # Ensure adapter is in monitor mode
        if iface.ADAPTER_STATUS != "monitor":
            iface.set_adapter_status("monitor")
            self.send_event("status_update", {"status": "adapter_mode_changed", "mode": "monitor"})
        
        # Start scanning in background
        scan_thread = threading.Thread(target=self._scan_ap_worker, daemon=True)
        scan_thread.start()
        
        return {"status": "scan_started"}
    
    def _scan_ap_worker(self):
        """Background worker for AP scanning"""
        self.is_scanning = True
        self.send_event("scan_progress", {"status": "scanning", "progress": 0})
        
        try:
            # TODO: Implement actual AP scanning logic here
            # For now, simulating with dummy data
            time.sleep(2)
            
            # Simulate finding APs
            self.access_points = [
                {"bssid": "AA:BB:CC:DD:EE:FF", "ssid": "TestNetwork1", "channel": 6, "signal": -45},
                {"bssid": "11:22:33:44:55:66", "ssid": "TestNetwork2", "channel": 11, "signal": -60}
            ]
            
            # Send results as event
            self.send_event("scan_complete", {
                "status": "complete",
                "access_points": self.access_points
            })
            
        except Exception as e:
            self.send_event("scan_error", {"error": str(e)})
        finally:
            self.is_scanning = False
    
    def handle_scan_devices(self, message: Message):
        """Scan for devices on a specific AP"""
        target_bssid = message.data.get("bssid")
        
        if not target_bssid:
            return {"status": "error", "message": "BSSID required"}
        
        # Start device scan in background
        scan_thread = threading.Thread(target=self._scan_devices_worker, args=(target_bssid,), daemon=True)
        scan_thread.start()
        
        return {"status": "scan_started", "bssid": target_bssid}
    
    def _scan_devices_worker(self, bssid: str):
        """Background worker for device scanning"""
        try:
            # TODO: Implement actual device scanning
            time.sleep(2)
            
            # Simulate finding devices
            self.devices = [
                {"mac": "AA:11:22:33:44:55", "vendor": "Apple", "signal": -50},
                {"mac": "BB:66:77:88:99:AA", "vendor": "Samsung", "signal": -55}
            ]
            
            self.send_event("devices_found", {
                "bssid": bssid,
                "devices": self.devices
            })
            
        except Exception as e:
            self.send_event("scan_error", {"error": str(e)})
    
    def handle_deauth_all(self, message: Message):
        """Deauthenticate all devices on target AP"""
        target_bssid = message.data.get("bssid")
        
        if not target_bssid:
            return {"status": "error", "message": "BSSID required"}
        
        if self.is_attacking:
            return {"status": "error", "message": "Attack already in progress"}
        
        # Start attack
        self.is_attacking = True
        attack_thread = threading.Thread(
            target=self._deauth_worker, 
            args=(target_bssid, None), 
            daemon=True
        )
        attack_thread.start()
        
        return {"status": "attack_started", "target": target_bssid}
    
    def handle_deauth_single(self, message: Message):
        """Deauthenticate single device"""
        target_bssid = message.data.get("bssid")
        target_mac = message.data.get("mac")
        
        if not target_bssid or not target_mac:
            return {"status": "error", "message": "BSSID and MAC required"}
        
        if self.is_attacking:
            return {"status": "error", "message": "Attack already in progress"}
        
        # Start attack
        self.is_attacking = True
        attack_thread = threading.Thread(
            target=self._deauth_worker, 
            args=(target_bssid, target_mac), 
            daemon=True
        )
        attack_thread.start()
        
        return {"status": "attack_started", "target": f"{target_bssid} -> {target_mac}"}
    
    def _deauth_worker(self, bssid: str, target_mac: str = None):
        """Background worker for deauth attack"""
        try:
            packet_count = 0
            
            while self.is_attacking:
                # TODO: Implement actual deauth packet sending
                # For now, just simulate
                time.sleep(0.1)
                packet_count += 10
                
                # Send progress updates every second
                if packet_count % 100 == 0:
                    self.send_event("attack_progress", {
                        "packets_sent": packet_count,
                        "target": target_mac if target_mac else "all"
                    })
            
            self.send_event("attack_stopped", {
                "total_packets": packet_count
            })
            
        except Exception as e:
            self.send_event("attack_error", {"error": str(e)})
            self.is_attacking = False
    
    def handle_stop_attack(self, message: Message):
        """Stop ongoing attack"""
        if not self.is_attacking:
            return {"status": "no_attack_running"}
        
        self.is_attacking = False
        return {"status": "stopping"}
    
    def handle_get_status(self, message: Message):
        """Get current module status"""
        return {
            "adapter_mode": iface.ADAPTER_STATUS,
            "is_scanning": self.is_scanning,
            "is_attacking": self.is_attacking,
            "access_points_count": len(self.access_points),
            "devices_count": len(self.devices)
        }

# Initialize module (called by bridge)
def init(host_socket):
    """Legacy init function - now handled by bridge"""
    pass

# Global instance
_module = None

def get_module() -> DeauthModule:
    global _module
    if _module is None:
        _module = DeauthModule()
    return _module
