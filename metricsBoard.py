import signal 
import threading

from datetime import datetime, timedelta
from time import sleep
from subprocess import Popen, PIPE

from metricsBoardBase import MetricsBoardBase

class MetricsBoard(MetricsBoardBase):

    def read_forever(self):
        """
            Start a process to read the configured pins as I2C SCL and SDA 
            without being addressed and do not ack.

            It must run as root, so run this script as root.
        """
        proc = Popen(["./i2c"], stdout=PIPE)

        t = threading.Thread(target=self.read_process, args=(proc, self.handle_data))
        t.start()

        while self.run:
            sleep(0.1)
        
        # i2c expects SIGINT to close normally.
        # Note, it handles any signal except SIGKILL gracefully
        # by resetting GPIO states
        proc.send_signal(signal.SIGINT)
        t.join()

    def read_process(self, proc, handle_data):
        """
            Read each line from the given process and call the given handler
        """

        for line in iter(proc.stdout.readline, b''):
            if self.run:
                handle_data(line.decode('ascii'))