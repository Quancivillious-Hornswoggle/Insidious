#!/usr/bin/env python3
"""
Network Tools Module
Provides functions for network scanning and MAC address retrieval
"""

import socket
import struct
import subprocess
import re
import ipaddress
from typing import List, Optional
from concurrent.futures import ThreadPoolExecutor, as_completed


def get_local_ip() -> str:
    """Get the local IP address of the machine"""
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        s.connect(("8.8.8.8", 80))
        local_ip = s.getsockname()[0]
        s.close()
        return local_ip
    except Exception:
        return "127.0.0.1"


def get_network_range() -> str:
    """Get the network range based on local IP (assumes /24 subnet)"""
    local_ip = get_local_ip()
    network = ipaddress.IPv4Network(f"{local_ip}/24", strict=False)
    return str(network)


def _ping_host(ip: str, timeout: int = 1) -> Optional[str]:
    """Ping a single host and return IP if active"""
    try:
        # Use -c 1 for one packet, -W for timeout in seconds
        result = subprocess.run(
            ["ping", "-c", "1", "-W", str(timeout), ip],
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL,
            timeout=timeout + 1
        )
        if result.returncode == 0:
            return ip
    except (subprocess.TimeoutExpired, Exception):
        pass
    return None


def scan_network(network_range: Optional[str] = None, max_workers: int = 50) -> List[str]:
    """
    Scan the network for all active IP addresses

    Args:
        network_range: Network range to scan (e.g., '192.168.1.0/24').
                      If None, automatically detects local network.
        max_workers: Number of concurrent threads for scanning

    Returns:
        List of active IP addresses
    """
    if network_range is None:
        network_range = get_network_range()

    network = ipaddress.IPv4Network(network_range, strict=False)
    active_ips = []

    print(f"Scanning network: {network_range}")

    # Create a list of all IPs to scan
    ips_to_scan = [str(ip) for ip in network.hosts()]

    # Use ThreadPoolExecutor for concurrent pinging
    with ThreadPoolExecutor(max_workers=max_workers) as executor:
        future_to_ip = {executor.submit(_ping_host, ip): ip for ip in ips_to_scan}

        for future in as_completed(future_to_ip):
            result = future.result()
            if result:
                active_ips.append(result)
                print(f"Found: {result}")

    active_ips.sort(key=lambda ip: ipaddress.IPv4Address(ip))
    return active_ips


def get_mac_address(ip: str) -> Optional[str]:
    """
    Get the MAC address for a specified IP address

    Args:
        ip: IP address to lookup

    Returns:
        MAC address string or None if not found
    """
    try:
        # First, ping the IP to ensure it's in the ARP cache
        subprocess.run(
            ["ping", "-c", "1", "-W", "1", ip],
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL,
            timeout=2
        )

        # Read the ARP cache
        result = subprocess.run(
            ["arp", "-n", ip],
            capture_output=True,
            text=True,
            timeout=2
        )

        # Parse the output for MAC address
        # Format: Address HWtype HWaddress Flags Mask Iface
        lines = result.stdout.split('\n')
        for line in lines:
            if ip in line:
                # Match MAC address pattern
                mac_match = re.search(r'([0-9a-fA-F]{2}[:-]){5}[0-9a-fA-F]{2}', line)
                if mac_match:
                    return mac_match.group(0).lower()
    except Exception as e:
        print(f"Error getting MAC for {ip}: {e}")

    return None


def get_adapter_mac(interface: str = "wlan0") -> Optional[str]:
    """
    Get the MAC address of a specified network adapter

    Args:
        interface: Network interface name (e.g., 'wlan0', 'eth0')

    Returns:
        MAC address string or None if not found
    """
    try:
        # Read from /sys/class/net/{interface}/address
        with open(f"/sys/class/net/{interface}/address", "r") as f:
            mac = f.read().strip().lower()
            return mac
    except FileNotFoundError:
        print(f"Interface {interface} not found")
    except Exception as e:
        print(f"Error reading MAC for {interface}: {e}")

    return None


def get_router_mac() -> Optional[str]:
    """
    Get the MAC address of the default gateway (router)

    Returns:
        MAC address string or None if not found
    """
    try:
        # Get default gateway IP
        result = subprocess.run(
            ["ip", "route", "show", "default"],
            capture_output=True,
            text=True,
            timeout=2
        )

        # Parse gateway IP from output
        # Format: default via 192.168.1.1 dev wlan0 proto dhcp metric 600
        match = re.search(r'default via ([\d.]+)', result.stdout)
        if match:
            gateway_ip = match.group(1)
            print(f"Gateway IP: {gateway_ip}")
            return get_mac_address(gateway_ip)
    except Exception as e:
        print(f"Error getting router MAC: {e}")

    return None