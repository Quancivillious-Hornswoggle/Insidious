"""
Network Tools Module
Provides functions for network scanning and MAC address retrieval
"""

import socket
import subprocess
import re
import ipaddress
from typing import List, Optional
from concurrent.futures import ThreadPoolExecutor, as_completed


def get_local_ip() -> str:
    """Get the local IP address of the machine"""
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    try:
        # Connect to a public IP address (e.g., Google's DNS server)
        s.connect(("8.8.8.8", 80))
        local_ip = s.getsockname()[0]
    except Exception:
        local_ip = "127.0.0.1"
    finally:
        s.close()
    return local_ip


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


def get_mac_address(ip: str, retries: int = 3) -> Optional[str]:
    """
    Get the MAC address for a specified IP address

    Args:
        ip: IP address to lookup
        retries: Number of times to retry pinging/checking ARP

    Returns:
        MAC address string or None if not found
    """
    for attempt in range(retries):
        try:
            # Ping the IP to ensure it's in the ARP cache
            subprocess.run(
                ["ping", "-c", "1", "-W", "2", ip],
                stdout=subprocess.DEVNULL,
                stderr=subprocess.DEVNULL,
                timeout=3
            )

            # Small delay to let ARP cache update
            import time
            time.sleep(0.2)

            # Try method 1: arp -n command
            result = subprocess.run(
                ["arp", "-n", ip],
                capture_output=True,
                text=True,
                timeout=2
            )

            # Parse the output for MAC address
            lines = result.stdout.split('\n')
            for line in lines:
                if ip in line:
                    # Match MAC address pattern
                    mac_match = re.search(r'([0-9a-fA-F]{2}[:-]){5}[0-9a-fA-F]{2}', line)
                    if mac_match:
                        mac = mac_match.group(0).lower()
                        # Make sure it's not incomplete (not all zeros or incomplete)
                        if mac != "00:00:00:00:00:00" and "incomplete" not in line.lower():
                            return mac

            # Try method 2: ip neigh command
            result = subprocess.run(
                ["ip", "neigh", "show", ip],
                capture_output=True,
                text=True,
                timeout=2
            )

            # Look for MAC in output
            mac_match = re.search(r'([0-9a-fA-F]{2}[:-]){5}[0-9a-fA-F]{2}', result.stdout)
            if mac_match and "FAILED" not in result.stdout and "INCOMPLETE" not in result.stdout:
                return mac_match.group(0).lower()

            # Try method 3: Read /proc/net/arp
            try:
                with open('/proc/net/arp', 'r') as f:
                    for line in f:
                        if ip in line:
                            parts = line.split()
                            if len(parts) >= 4:
                                mac = parts[3].lower()
                                if mac != "00:00:00:00:00:00" and len(mac) == 17:
                                    return mac
            except Exception:
                pass

        except Exception as e:
            print(f"Attempt {attempt + 1} failed: {e}")
            if attempt < retries - 1:
                import time
                time.sleep(0.5)

    print(f"Could not resolve MAC address for {ip} after {retries} attempts")
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


def get_router_ip(interface: str = "wlan0") -> Optional[str]:
    """
    Get the router/gateway IP address for a specific interface

    Args:
        interface: Network interface name (default: "wlan0")

    Returns:
        Router IP address or None if not found
    """
    try:
        # First try to get default route for specific interface
        result = subprocess.run(
            ["ip", "route", "show", "default", "dev", interface],
            capture_output=True,
            text=True,
            timeout=2
        )

        # Parse gateway IP from output
        match = re.search(r'default via ([\d.]+)', result.stdout)
        if match:
            return match.group(1)

        # If no default route, try to get gateway from interface's network config
        result = subprocess.run(
            ["ip", "route", "show", "dev", interface],
            capture_output=True,
            text=True,
            timeout=2
        )

        # Look for any route with via (gateway)
        for line in result.stdout.split('\n'):
            if 'via' in line:
                match = re.search(r'via ([\d.]+)', line)
                if match:
                    return match.group(1)

        # Try getting from all routes and filter by interface
        result = subprocess.run(
            ["ip", "route"],
            capture_output=True,
            text=True,
            timeout=2
        )

        for line in result.stdout.split('\n'):
            if interface in line and 'default via' in line:
                match = re.search(r'default via ([\d.]+)', line)
                if match:
                    return match.group(1)

    except Exception as e:
        print(f"Error getting router IP for {interface}: {e}")

    return None


def get_router_mac(interface: str = "wlan0") -> Optional[str]:
    """
    Get the MAC address of the router for a specific interface

    Args:
        interface: Network interface name (default: "wlan0")

    Returns:
        MAC address string or None if not found
    """
    try:
        router_ip = get_router_ip(interface)
        if router_ip:
            print(f"Gateway IP ({interface}): {router_ip}")
            return get_mac_address(router_ip)
        else:
            print(f"Could not determine router IP for {interface}")
    except Exception as e:
        print(f"Error getting router MAC: {e}")

    return None


# Example usage
if __name__ == "__main__":
    print("=== Network Tools Demo ===\n")

    # Get local network info
    print(f"Local IP: {get_local_ip()}")
    print(f"Network Range: {get_network_range()}\n")

    # Scan network
    print("Scanning network for active IPs...")
    active_ips = scan_network()
    print(f"\nFound {len(active_ips)} active IPs:")
    for ip in active_ips:
        print(f"  - {ip}")

    # Get MAC addresses
    print("\n--- MAC Address Lookups ---")

    # Get wlan0 MAC
    wlan_mac = get_adapter_mac("Wi-Fi")
    print(f"wlan0 MAC: {wlan_mac}")

    # Get router MAC
    router_mac = get_router_mac()
    print(f"Router MAC: {router_mac}")

    # Get MAC for first active IP (if any)
    if active_ips:
        test_ip = active_ips[0]
        mac = get_mac_address(test_ip)
        print(f"MAC for {test_ip}: {mac}")