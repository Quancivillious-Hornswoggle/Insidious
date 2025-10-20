"""
This file is used to bridge C# and python.
This runs a local server to be connected to by the C# program
and then proceeds to run and pass other arguments to needed python
program
"""

import socket, mitm, server


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
    # Create continuous loop
    while True:
        cmd = ms.recv(1024)

        match cmd:
            case 1:
                pass
            case 2:
                pass
            case 3:
                pass
            case 'X':
                ms.close()
                ss.close()
                return


if __name__ == "__main__":
    init_server()