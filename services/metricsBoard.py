import signal 
import threading

from datetime import datetime, timedelta
from time import sleep
from socket import *
from subprocess import Popen, PIPE

from metricsBoardBase import MetricsBoardBase

class MetricsBoard(MetricsBoardBase):

    def read_forever(self, use_process = False):

        if use_process:
            # Start a process to read metrics data.
            # It must run as root, so run this script as root.
            proc = Popen(["./i2c/bin/i2c"], stdout=PIPE)
            t = threading.Thread(target=self.read_process, args=(proc, self.handle_data))
        else:
            # Read metrics data broadcast on a socket
            t = threading.Thread(target=self.read_socket, args=(self.handle_data,))
        t.start()

        while self.run:
            sleep(0.1)
        
        if use_process:
            # i2c expects SIGINT to close normally.
            # Note, it handles any signal except SIGKILL gracefully
            # by resetting GPIO states
            proc.send_signal(signal.SIGINT)
        t.join()

    def read_socket(self, handle_data):
        sock = socket(AF_INET, SOCK_DGRAM)
        sock.bind(('', 7887))
        NEWLINE = 10

        buffer = b''
        while self.run:
            msg, addr = sock.recvfrom(1024)
            buffer += msg
            
            while NEWLINE in buffer and self.run:
                index = buffer.index(NEWLINE)
                data = buffer[:index]
                buffer = buffer[index+1:]
                handle_data(data.decode("ascii"))

    def read_process(self, proc, handle_data):
        """
            Read each line from the given process and call the given handler
        """

        for line in iter(proc.stdout.readline, b''):
            if self.run:
                handle_data(line.decode('ascii'))