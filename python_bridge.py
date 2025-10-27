"""
This file is used to bridge C# and python.
This runs a local server to be connected to by the C# program
and then proceeds to run and pass other arguments to needed python
program
"""

import socket
import time
import sys
import os

# Add Modules directory to path
sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'Modules'))

from Modules.message_broker import get_broker
from Modules import deauth, mitm, dns, server, packet_capture, wifi_brute

def _safe_shutdown(sock):
    """
    This method is just to try to ensure a safe shutdown
    """
    if not sock:
        return
    try:
        sock.shutdown(socket.SHUT_RDWR)
    except Exception:
        pass
    try:
        sock.close()
    except Exception:
        pass

def init_server():
    """
    Initialize server to accept connection and then handle it
    """
    # Run in loop in case error happened
    while True:
        server_socket = None
        try:
            print("Initializing server...")
            # Set up server info
            server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            host = '0.0.0.0'
            port = 65535
            server_socket.bind((host, port))
            server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            # Only accept 1 connection
            server_socket.listen(1)
            print(f"Server listening on {host}:{port}")
            
            # Start listening for host (should be nearly instant)
            main_socket, addr = server_socket.accept()
            print(f"Connection accepted from {addr}")
            
            # Once connected handle that connection
            main(server_socket, main_socket)

            # Close connections
            server_socket.close()
            main_socket.close()
            print("Closed connections")
            time.sleep(1)
            return
        except KeyboardInterrupt:
            try:
                _safe_shutdown(server_socket)
            except Exception as e:
                print(e)
            return
        except Exception as e:
            print(f"Server error: {e}")
            time.sleep(1)
        finally:
            # make sure the socket is released on any exit from try
            _safe_shutdown(server_socket)

def main(ss, ms):
    """
    Main method that handles the bridge using message broker pattern
    """
    print("Initializing message broker and modules...")
    
    try:
        # Get the message broker
        broker = get_broker()
        
        # Initialize all module instances
        modules = {
            'deauth': deauth.get_module(),
            'mitm': mitm.get_module(),
            #'dns' : dns.get_module(),
            # 'server': server.get_module(),
            # 'packet_capture': packet_capture.get_module(),
            # 'wifi_brute': wifi_brute.get_module(),
        }
        
        # Start all modules
        for name, module in modules.items():
            module.start()
            print(f"Started module: {name}")
        
        # Start the message broker with the socket
        broker.start(ms)
        
        print("=" * 50)
        print("Bridge is ready and listening for commands!")
        print("=" * 50)
        
        # Keep main thread alive and monitor connection
        while True:
            # Check if socket is still connected
            try:
                # Send keepalive or just sleep
                time.sleep(5)
                
                # You could periodically check socket health here
                # For now, just keep running
                
            except KeyboardInterrupt:
                print("\nShutting down...")
                break
            except Exception as e:
                print(f"Main loop error: {e}")
                break
        
    except Exception as e:
        print(f"Fatal error: {e}")
        import traceback
        traceback.print_exc()
    finally:
        # Cleanup
        print("Cleaning up...")
        
        # Stop all modules
        for name, module in modules.items():
            try:
                module.stop()
            except Exception as e:
                print(f"Error stopping {name}: {e}")
        
        # Stop broker
        try:
            broker.stop()
        except Exception as e:
            print(f"Error stopping broker: {e}")

if __name__ == "__main__":
    init_server()
