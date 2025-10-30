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
from Modules.Network_Modules import network_adapter as iface

# Add Modules directory to path
sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'Modules'))

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

    iface.ADAPTER_NAME = "Wi-Fi"

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
            import traceback
            traceback.print_exc()
            time.sleep(1)
        finally:
            # make sure the socket is released on any exit from try
            _safe_shutdown(server_socket)

def main(ss, ms):
    """
    Main method that handles the bridge using message broker pattern
    """
    print("\n" + "="*60)
    print("Initializing message broker and modules...")
    print("="*60)
    
    modules = {}
    broker = None
    
    try:
        # STEP 1: Import the broker (but don't start it yet)
        print("\n[1] Importing message broker...")
        from Modules.message_broker import get_broker
        broker = get_broker()
        print("    ✓ Broker instance created")
        
        # STEP 2: Import module classes
        print("\n[2] Importing module classes...")
        try:
            from Modules import deauth
            print("    ✓ deauth imported")
        except Exception as e:
            print(f"    ✗ deauth import failed: {e}")
            import traceback
            traceback.print_exc()
        
        try:
            from Modules import mitm
            print("    ✓ mitm imported")
        except Exception as e:
            print(f"    ✗ mitm import failed: {e}")
            import traceback
            traceback.print_exc()

        try:
            from Modules import dns
            print("    ✓ server imported")
        except Exception as e:
            print(f"    ✗ server import failed: {e}")
            import traceback
            traceback.print_exc()

        
        # STEP 3: Create module instances (this registers them)
        print("\n[3] Creating module instances...")
        
        try:
            deauth_mod = deauth.get_module()
            modules['deauth'] = deauth_mod
            print(f"    ✓ deauth module created: {deauth_mod.module_name}")
        except Exception as e:
            print(f"    ✗ Failed to create deauth module: {e}")
            import traceback
            traceback.print_exc()
        
        try:
            mitm_mod = mitm.get_module()
            modules['mitm'] = mitm_mod
            print(f"    ✓ mitm module created: {mitm_mod.module_name}")
        except Exception as e:
            print(f"    ✗ Failed to create mitm module: {e}")
            import traceback
            traceback.print_exc()

        try:
            dns_mod = dns.get_module()
            modules['server'] = dns_mod
            print(f"    ✓ server module created: {dns_mod.module_name}")
        except Exception as e:
            print(f"    ✗ Failed to create server module: {e}")
            import traceback
            traceback.print_exc()
        
        # STEP 4: Verify registration
        print("\n[4] Verifying broker registration...")
        print(f"    Checking broker instance (id: {id(broker)})")
        registered_modules = broker.list_registered_modules()
        print(f"    Module queues: {registered_modules}")
        
        if len(broker.module_queues) == 0:
            print("    ✗✗✗ ERROR: No modules registered! ✗✗✗")
            print("    Check the error messages above for import failures")
            return
        else:
            print(f"    ✓ {len(broker.module_queues)} module(s) registered successfully")
        
        # STEP 5: Start modules
        print("\n[5] Starting modules...")
        for name, module in modules.items():
            try:
                module.start()
                print(f"    ✓ Started module: {name}")
            except Exception as e:
                print(f"    ✗ Failed to start {name}: {e}")
        
        # STEP 6: Start broker
        print("\n[6] Starting message broker...")
        broker.start(ms)
        print("    ✓ Broker started and listening")
        
        print("\n" + "="*60)
        print("✓ Bridge is ready and listening for commands!")
        print("="*60)
        print(f"\nActive modules: {list(modules.keys())}")
        print("Waiting for commands from C#...\n")
        
        # Keep main thread alive and monitor connection
        while True:
            try:
                time.sleep(5)
            except KeyboardInterrupt:
                print("\nShutting down...")
                break
            except Exception as e:
                print(f"Main loop error: {e}")
                break
        
    except Exception as e:
        print(f"\n✗✗✗ FATAL ERROR: {e}")
        import traceback
        traceback.print_exc()
    finally:
        # Cleanup
        print("\nCleaning up...")
        
        # Stop all modules
        for name, module in modules.items():
            try:
                module.stop()
                print(f"Stopped {name}")
            except Exception as e:
                print(f"Error stopping {name}: {e}")
        
        # Stop broker
        if broker:
            try:
                broker.stop()
                print("Stopped broker")
            except Exception as e:
                print(f"Error stopping broker: {e}")

if __name__ == "__main__":
    init_server()
