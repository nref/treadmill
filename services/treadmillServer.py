import socket
import threading

from http.server import HTTPServer
from time import sleep

from treadmillRequestHandler import TreadmillRequestHandler

HOST = ''
PORT = 8000

class HealthController:
    def __init__(self):
        self.listeners = {}
        self.run = True
        self.t = threading.Thread(target=self.update_listeners)
        self.t.start()
        
    def close(self):
        self.run = False
        for addr, (sock, (ip, port)) in self.listeners.items():
            sock.close()
        self.t.join()

    def add_listener(self, addr):
        if addr in self.listeners:
            return

        print(f'Adding health listener {addr}')
        ipport = addr.decode().split(':')
        sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
        self.listeners[addr] = (sock, (ipport[0], int(ipport[1])))

    def update_listeners(self):
        while (self.run):
            for addr, (sock, (ip, port)) in self.listeners.items():
                print(f'Reporting health to {addr}')
                sock.sendto(bytes("ok", "utf-8"), (ip, port))
            sleep(5)

class TreadmillServer:
    def __init__(self, tm, metrics):

        self.healthController = HealthController()

        def create_handler(*args):
            TreadmillRequestHandler(tm, metrics, self.healthController, *args)

        server = HTTPServer((HOST, PORT), create_handler)
        print(f'Server up at {HOST}:{PORT}')

        try:
            server.serve_forever()
        except KeyboardInterrupt:
            pass

        print('Closing server...')
        server.server_close()