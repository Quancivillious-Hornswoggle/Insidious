"""
Network Deauthentication Module
Deauthenticates specified or all clients on a network
"""

from Network_Modules import network_adapter as iface

def init(host_socket):
    pass

def run(cmd):
    if iface.ADAPTER_STATUS != "monitor":
        iface.set_adapter_status("monitor")

    match cmd:
        case "ScanAP":
            pass
        case "ScanDevices":
            pass
        case "DeauthAll":
            pass
        case "DeauthSingle":
            pass
        case "Stop":
            iface.set_adapter_status("managed")
        case _:
            pass