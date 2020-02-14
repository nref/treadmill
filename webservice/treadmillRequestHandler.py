from http.server import BaseHTTPRequestHandler

class TreadmillRequestHandler(BaseHTTPRequestHandler):

    def __init__(self, tm, metrics, *args):
        
        self.get_routes = {
            '/incline/setpoint' : tm.incline_setpoint,
            '/incline/feedback' : tm.incline_setpoint,
            '/incline/expected' : tm.incline_expected,
            '/incline/min' : tm.incline_min,
            '/incline/max' : tm.incline_max,
            '/speed/setpoint' : tm.speed_setpoint,
            '/speed/feedback' : tm.speed_feedback,
            '/speed/expected' : tm.speed_expected,
            '/speed/min' : tm.speed_min,
            '/speed/max' : tm.speed_max,
            '/distance' : tm.distance,
            '/timestamp' : tm.timestamp,
            '/state' : tm.state,
        }

        self.post_routes = {
            '/incline/setpoint' : tm.go_to_incline,
            '/speed/setpoint' : tm.go_to_speed,
            '/start' : tm.start_workout,
            '/pause' : tm.pause,
            '/resume' : tm.resume,
            '/end' : tm.end_workout,
        }

        self.route_takes_arg = {
            '/incline/setpoint' : True,
            '/speed/setpoint' : True,
        }

        BaseHTTPRequestHandler.__init__(self, *args)

    def takes_arg(self, route):
        return route in self.route_takes_arg and self.route_takes_arg[route]
        
    def do_POST(self):
        content_length = int(self.headers['Content-Length'])
        content = self.rfile.read(content_length)
        response = self.route_post(content)
        self.wfile.write(response)

    def do_GET(self):
        response = self.route_get()
        self.wfile.write(response)

    def route_post(self, data):

        if self.path not in self.post_routes:
            return self.not_found()
        
        try:

            if self.takes_arg(self.path):
                response = self.post_routes[self.path](float(data))
            else:
                response = self.post_routes[self.path]()
            
        except ValueError as e:
            return self.bad_request(str(e))

        return self.ok(response)

    def route_get(self):
        if self.path not in self.get_routes:
            return self.not_found()

        return self.ok(self.get_routes[self.path])

    def not_found(self):
        self.send_response(404)
        return bytes('Not Found\n', 'utf-8')

    def bad_request(self, response):
        self.send_response(400)
        return bytes(f'{response}\n', 'utf-8')

    def ok(self, response):
        self.send_response(200)
        self.send_header('Content-type', 'text/html')
        self.end_headers()

        if response is None:
            response = 'ok'

        return bytes(f'{response}\n', 'utf-8')