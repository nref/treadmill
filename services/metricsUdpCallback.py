import json
import socket

from treadmillMetric import TreadmillMetric

class MetricsUdpCallback:
    
    def __init__(self, addr):
        self.url = addr
        ipport = self.url.decode().split(':')
        self.UDP_IP = ipport[0]
        self.UDP_PORT = int(ipport[1])
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)

    def handle_metric_changed(self, metric, value):
        if metric == TreadmillMetric.Timestamp or metric == TreadmillMetric.Distance:
            return # Don't care

        data = { "metric": metric, "value": value } 
        self.send_udp(json.dumps(data, default=str))

    def send_udp(self, msg):
        self.sock.sendto(bytes(msg, "utf-8"), (self.UDP_IP, self.UDP_PORT))