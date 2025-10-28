#!/usr/bin/env python3
"""
Quick test to verify net_tools is getting the correct interface IP
"""

import sys
import os
sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'Modules'))

from Modules.Network_Modules import net_tools, network_adapter

print("="*60)
print("Network Interface Test")
print("="*60)

print(f"\nAdapter Name: {network_adapter.ADAPTER_NAME}")
print(f"Adapter Status: {network_adapter.ADAPTER_STATUS}")

print(f"\n--- WiFi Interface (wlan0) ---")
wlan_ip = net_tools.get_local_ip("wlan0")
print(f"  IP: {wlan_ip}")
wlan_range = net_tools.get_network_range("wlan0")
print(f"  Network Range: {wlan_range}")
wlan_mac = net_tools.get_adapter_mac("wlan0")
print(f"  MAC: {wlan_mac}")

print(f"\n--- Ethernet Interface (eth0) ---")
try:
    eth_ip = net_tools.get_local_ip("eth0")
    print(f"  IP: {eth_ip}")
    eth_range = net_tools.get_network_range("eth0")
    print(f"  Network Range: {eth_range}")
except Exception as e:
    print(f"  Not available: {e}")

print(f"\n--- Router Info (wlan0) ---")
router_ip = net_tools.get_router_ip("wlan0")
print(f"  Gateway IP: {router_ip}")
router_mac = net_tools.get_router_mac("wlan0")
print(f"  Gateway MAC: {router_mac}")

print("\n" + "="*60)
print("✓ Test complete!")
print("="*60)
