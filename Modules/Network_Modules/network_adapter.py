"""
This module stores data on the network adapter and provides methods to manage it.
"""

import net_tools as adapter
import subprocess, shlex

# ADAPTER DATA
ADAPTER_STATUS = "up" # (up, down, monitor)
ADAPTER_TYPE = "wifi"
ADAPTER_NAME = "wlan0"
ADAPTER_MAC = None
ADAPTER_IP = None

def set_adapter_status(status):
    global ADAPTER_STATUS
    subprocess.run(["sudo", "ip", "link", "set", ADAPTER_NAME, "down"], check=True)
    subprocess.run(["sudo", "iw", "dev", ADAPTER_NAME, "set", "type", status], check=True)
    subprocess.run(["sudo", "ip", "link", "set", ADAPTER_NAME, "up"], check=True)
    ADAPTER_STATUS = status
    print("Adapter is now in " + ADAPTER_STATUS + " mode.")

def set_adapter_type(type):
    global ADAPTER_TYPE
    ADAPTER_TYPE = type

def scan_for_aps():
    """Return list of networks as dicts: {'ssid','bssid','signal','chan'}."""


def scan_for_devices_connected(net):
    pass