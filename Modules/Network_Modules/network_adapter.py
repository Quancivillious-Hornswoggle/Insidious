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

def scan_for_ap():
    """Return list of networks as dicts: {'ssid','bssid','signal','chan'}."""
    # Try nmcli (preferred)
    try:
        out = subprocess.run(
            shlex.split(f"nmcli -t -f SSID,BSSID,SIGNAL,CHAN dev wifi list ifname {ADAPTER_NAME}"),
            capture_output=True, text=True, check=True
        ).stdout.strip()
        nets = []
        for line in out.splitlines():
            if not line:
                continue
            ssid, bssid, sig, chan = line.split(":", 3)
            nets.append({"ssid": ssid or None, "bssid": bssid or None,
                         "signal": int(sig) if sig.isdigit() else None, "chan": chan or None})
        return nets
    except Exception:
        # Fallback to iwlist (may require sudo)
        try:
            out = subprocess.run(
                shlex.split(f"sudo iwlist {ADAPTER_NAME} scan"),
                capture_output=True, text=True, check=True
            ).stdout
            nets = []
            cur = {}
            for L in out.splitlines():
                l = L.strip()
                if l.startswith("Cell "):
                    if cur: nets.append(cur)
                    cur = {"ssid": None, "bssid": None, "signal": None, "chan": None}
                    parts = l.split()
                    if len(parts) >= 5:
                        cur["bssid"] = parts[4]
                elif "ESSID:" in l:
                    cur["ssid"] = l.split("ESSID:")[1].strip().strip('"')
                elif "Signal level=" in l:
                    try:
                        cur["signal"] = int(l.split("Signal level=")[1].split()[0])
                    except:
                        pass
                elif "Channel:" in l:
                    cur["chan"] = l.split("Channel:")[1].strip()
            if cur: nets.append(cur)
            return nets
        except Exception as e:
            raise RuntimeError("No scan tool available or scan failed.") from e


scan_for_ap()
nets = scan_for_ap()
for net in nets:
    print(net["ssid"], net["bssid"], net["signal"])
