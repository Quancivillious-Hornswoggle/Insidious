import http.server
import socketserver
import sys
import os
import threading

# Ensure proper import paths
modules_dir = os.path.dirname(os.path.abspath(__file__))
if modules_dir not in sys.path:
    sys.path.insert(0, modules_dir)
parent_dir = os.path.dirname(modules_dir)
if parent_dir not in sys.path:
    sys.path.insert(0, parent_dir)

# Import with full path for singleton consistency
try:
    from Modules.Network_Modules import network_adapter as iface
    from Modules.Network_Modules import net_tools as network
    from Modules.base_module import BaseModule
    from Modules.message_broker import Message
except ImportError:
    from Network_Modules import network_adapter as iface
    from Network_Modules import net_tools as network
    from base_module import BaseModule
    from message_broker import Message


class ServerModule(BaseModule):
    Handler = http.server.SimpleHTTPRequestHandler
    PORT = 80

    def __init__(self):
        super().__init__("server")

        # Module state
        self.is_running = False
        self.host_ip = None
        self.site_directory = None
        self.httpd = None  # Reference to the server instance
        self.server_thread = None  # Reference to the server thread

        # Register command handlers
        self.register_handler("start", self.handle_start_server)
        self.register_handler("stop", self.handle_stop_server)

    def on_start(self):
        """Initialize module"""
        print(f"[{self.module_name}] Server module initialized")
        # Ensure adapter is in managed mode
        if iface.ADAPTER_STATUS == "monitor":
            iface.set_adapter_status("managed")

    def on_stop(self):
        """Cleanup module"""
        if self.is_running:
            self.is_running = False

    # Command Handlers

    def handle_start_server(self, message: Message):
        if self.is_running:
            return {"status": "server_already_running"}

        self.is_running = True
        self.host_ip = iface.ADAPTER_IP
        self.site_directory = message.data.get("site_directory")

        # Create a thread to run the server
        self.server_thread = threading.Thread(target=self._start_server, daemon=True)
        self.server_thread.start()

        return {"status": "started_server"}

    def _start_server(self):
        # Change the current working directory for SimpleHTTPRequestHandler
        if self.site_directory and os.path.isdir(self.site_directory):
            os.chdir(self.site_directory)

        with socketserver.TCPServer(("", self.PORT), self.Handler) as self.httpd:
            print(f"[{self.module_name}] Serving from '{os.getcwd()}' at port {self.PORT}")
            # serve_forever() polls for the shutdown() method, which is called in the stop handler
            self.httpd.serve_forever(poll_interval=1)

    def handle_stop_server(self, message: Message):
        if not self.is_running:
            return {"status": "stopped_server"}

        self.is_running = False
        print(f"[{self.module_name}] Stopping server...")

        if self.httpd:
            self.httpd.shutdown()
            self.httpd.server_close()

        if self.server_thread and self.server_thread.is_alive():
            self.server_thread.join(timeout=5)  # Wait for the thread to finish

        print(f"[{self.module_name}] Server stopped.")
        return {"status": "stopped_server"}


# Global instance
_module = None


def get_module() -> ServerModule:
    global _module
    if _module is None:
        _module = ServerModule()
    return _module

