"""
This file is used to bridge C# and python.
This runs a local server to be connected to by the C# program
and then proceeds to run and pass other arguments to needed python
program
"""

import socket, threading
from Modules import wifi_brute, deauth, mitm, dns, server, packet_capture

"""
Initialize server to accept connection and then handle it
"""
def init_server():
    # Set up server info
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    host = '0.0.0.0'
    port = 65535
    server_socket.bind((host, port))
    # Only accept 1 connection
    server_socket.listen(1)
    # Start listening for host (should be nearly instant)
    main_socket, addr = server_socket.accept()
    # Once connected handle that John
    main(server_socket, main_socket)

def main(ss, ms):
    # Initialize all modules within a thread
    module_threads = []
    module_threads[0] = threading.Thread(target=wifi_brute.init(ms))
    module_threads[1] = threading.Thread(target=deauth.init(ms))
    module_threads[2] = threading.Thread(target=mitm.init(ms))
    module_threads[3] = threading.Thread(target=dns.init(ms))
    module_threads[4] = threading.Thread(target=server.init(ms))
    module_threads[5] = threading.Thread(target=packet_capture.init(ms))
    # Specify daemon to close if this closes and then start the thread
    for t in module_threads:
        t.daemon = True
        t.start()

    # Create continuous loop
    while True:
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

        # Send return message if one
        ms.send(rtn_msg.encode('utf-8'))


if __name__ == "__main__":
    init_server()