import pigpio
import signal
import threading
from time import sleep
from datetime import datetime, timedelta
from enum import Enum


class ClockState(Enum):
    FALLING = 0
    RISING = 1
    STEADY = 2

do_read_forever = True

def read_forever(pi, SCL, SDA, sniffer):
    scl_value = 0
    sda_value = 0

    read_forever = True
    while read_forever:
        scl_temp = pi.read(SCL)
        sda_temp = pi.read(SDA)

        if scl_temp != scl_value:
            scl_value = scl_temp
            sniffer.handle_edge(SCL, scl_value)
            
        if sda_temp != sda_value:
            sda_value = sda_temp
            sniffer.handle_edge(SDA, sda_value)
        print(f"{scl_value} {sda_value}")
        #sleep(0.001)

class Sniffer:
    """
    A class to passively monitor activity on an I2C bus. 
    (Messages are not ACKed)
    
    Tested work on an I2C bus running at 100kbps.

    Be sure to start pigpiod. 
    Note that the default GPIO polling interval of 5ms is too slow.
    Set it with a faster value.
    Example: sudo pigpiod -s 1 -b 1000.
    """

    def __init__(self, SCL, SDA, handle_data):
        """
        Instantiate with the gpios for the I2C clock and data lines. 
        The given callback is called after an I2C message is received.
        """
        
        self.pi = pigpio.pi()
        self.SCL = SCL
        self.SDA = SDA
        self.handle_data = handle_data

        self.reset()
        self.oldSCL = 1
        self.oldSDA = 1

        self.pi.set_mode(SCL, pigpio.INPUT)
        self.pi.set_mode(SDA, pigpio.INPUT)

        self.pi.set_pull_up_down(SCL, pigpio.PUD_UP)
        self.pi.set_pull_up_down(SDA, pigpio.PUD_UP)

        self.subscribe()

    def subscribe(self):
        #self.t = threading.Thread(target=read_forever, args=(self.pi, self.SCL, self.SDA, self))
        #self.t.start()

        self.handle_scl_changed = self.pi.callback(self.SCL, pigpio.EITHER_EDGE, self.handle_edge)
        self.handle_sda_changed = self.pi.callback(self.SDA, pigpio.EITHER_EDGE, self.handle_edge)

    def unsubscribe(self):
        self.handle_scl_changed.cancel()
        self.handle_sda_changed.cancel()

        #global do_read_forever
        #do_read_forever = False
        #self.t.join()

    def reset(self):
        self.in_data = False
        self.byte = 0
        self.bit = 0
        self.data = []

    def stop(self):
        self.unsubscribe()
        self.pi.stop()

    def handle_edge(self, gpio, level, tick = 0):
        """
        Check which line has altered state (ignoring watchdogs) and
        call the parser with the new state.
        """
        
        SCL = self.oldSCL
        SDA = self.oldSDA

        if gpio == self.SCL:
            SCL = level

        if gpio == self.SDA:
            SDA = level

        self.interpret_signals(SCL, SDA)

    def get_clock_states(self, SCL, SDA):

        if SCL == self.oldSCL:
            scl_state = ClockState.STEADY
        else:
            self.oldSCL = SCL
            scl_state = ClockState.RISING if SCL == 1 else ClockState.FALLING

        if SDA == self.oldSDA:
            sda_state = ClockState.STEADY
        else:
            self.oldSDA = SDA
            sda_state = ClockState.RISING if SDA == 1 else ClockState.FALLING

        return scl_state, sda_state

    def interpret_signals(self, SCL, SDA):
        """
        Accumulate all the data between START and STOP conditions
        and call hanlle_callback when STOP is detected.
        """

        scl_state, sda_state = self.get_clock_states(SCL, SDA)

        if scl_state == ClockState.RISING:
            if not self.in_data:
                return

            if self.bit < 8:
                self.byte = (self.byte << 1) | SDA
                self.bit += 1
            else:
                self.data.append(self.byte)
                self.bit = 0
                self.byte = 0

        elif scl_state == ClockState.STEADY:

            if sda_state == ClockState.RISING:
                if SCL == 1: 
                    self.handle_data(self.data)
                    self.reset()

            if sda_state == ClockState.FALLING:
                if SCL == 1:
                    self.in_data = True
                    self.byte = 0
                    self.bit = 0

class RateLimiter:
    def __init__(self):
        self.last_callback_time = datetime.now()

    def rate_limit(self, seconds = 1):
        now = datetime.now()
        if now-self.last_callback_time < timedelta(seconds=1):
            return True
        self.last_callback_time = now
        return False

class TestRunner:

    def __init__(self):
        self.sniffer = Sniffer(9, 10, self.handle_data2)
        self.rate_limiter = RateLimiter()

    def handle_data2(self, data):
        chars = ''.join(chr(byte) for byte in data)
        timestamp = chars[2:7]
        incline = chars[9:13]
        speed = chars[15:19]
        distance = chars[20:25]

        print(f"\'{timestamp}\' {incline}\' \'{speed}\' \'{distance}\'")

    def handle_data(self, data):
        if self.rate_limiter.rate_limit():
            return

        print(data)

    def stop(self):
        self.sniffer.stop()

    def run_forever(self):
        signal.pause()

if __name__ == "__main__":

    runner = TestRunner()
    runner.run_forever()
    runner.stop()