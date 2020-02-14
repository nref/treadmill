from http.server import HTTPServer

from treadmillRequestHandler import TreadmillRequestHandler

HOST = ''
PORT = 8000

class TreadmillServer:
    def __init__(self, tm, metrics):

        def create_handler(*args):
            TreadmillRequestHandler(tm, metrics, *args)

        server = HTTPServer((HOST, PORT), create_handler)
        print(f'Server up at {HOST}:{PORT}')

        try:
            server.serve_forever()
        except KeyboardInterrupt:
            pass

        print('Closing server...')
        server.server_close()