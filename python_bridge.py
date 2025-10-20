"""
This file is used to bridge C# and python.
This runs a local server to be connected to by the C# program
and then proceeds to run and pass other arguments to needed python
program
"""

import socket, threading, time

from Modules import wifi_brute, deauth, mitm, dns, server, packet_capture

"""
This method is just to try to ensure a safe shutdown
"""
def _safe_shutdown(sock):
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

"""
Initialize server to accept connection and then handle it
"""
def init_server():
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
            # Start listening for host (should be nearly instant)
            main_socket, addr = server_socket.accept()
            # Once connected handle that John
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
            print(e)
            time.sleep(1)
        finally:
            # make sure the socket is released on any exit from try
            _safe_shutdown(server_socket)

"""
Main method that handles the bridge
"""
def main(ss, ms):
    print("Initializing modules/threads")
    # Initialize all modules within a thread
    module_threads = []
    for mod in (wifi_brute, deauth, mitm, dns, server, packet_capture):
        t = threading.Thread(target=mod.init, args=(ms,), daemon=True)
        module_threads.append(t)
        t.start()
    print("Ready")

    # Create continuous loop
    while True:
        try:
            # Receive a command from the main host
            cmd = ms.recv(1024)
            print(str(cmd))
            rtn_msg = None

            # Run the command specified
            match cmd:
                case 1:
                    pass
                case 2:
                    pass
                case 3:
                    pass
                case 4:
                    pass
                case 5:
                    pass
                case 6:
                    pass
                case 'X':
                    ms.close()
                    ss.close()
                    rtn_msg = "Stopped modules."
                    return
                case _:
                    rtn_msg = "Error running command. ???"
        except ConnectionResetError:
            return

        # Send return message if one
        ms.send(rtn_msg.encode('utf-8'))

if __name__ == "__main__":
    init_server()