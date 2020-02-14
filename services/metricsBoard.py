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
            # Pipe metrics data from i2c
            # i2c must run as root, so run this script as root.
            proc = Popen(["./i2c/bin/i2c"], stdout=PIPE)
            self.t = threading.Thread(target=self.read_process, args=(proc, self.handle_data))
        else:
            # Read metrics data broadcast on a socket
            # Requires i2c to be started separately
            self.sock = socket(AF_INET, SOCK_DGRAM)
            self.sock.bind(('', 7887))

            self.t = threading.Thread(target=self.read_socket, args=(self.handle_data,))

        self.t.start()

        while self.run:
            sleep(0.1)
        
        if use_process:
            # i2c expects SIGINT to close normally.
            # Note, it handles any signal except SIGKILL gracefully
            # by resetting GPIO states
            proc.send_signal(signal.SIGINT)

        self.sock.close()
        self.t.join()

    def read_socket(self, handle_data):
        NEWLINE = 10

        buffer = b''
        while self.run:
            # TODO this prevents SIGINT shutdown
            msg, addr = self.sock.recvfrom(1024)
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