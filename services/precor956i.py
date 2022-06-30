import threading
import datetime

from gpiozero import LED
from time import sleep

from metricsBoardBase import ZERO
from treadmillServer import TreadmillServer
from treadmillState import TreadmillState
from treadmillMetric import TreadmillMetric

class Precor956i:

    def __init__(self):

        self.run = True
        self.state = TreadmillState.Ready
        self.state_changed_callbacks = []
        self.timestamp = "00:00"

        self.lastUpdate = datetime.datetime.now()
        self.heartbeat = datetime.timedelta(seconds=5)

        self.distance = 0.0 # mi or km
        self.speed_setpoint = 0.0 # mi/h or km/h
        self.speed_feedback = 0.0 # mi/h or km/h

        self.incline_setpoint = 0.0 # percent grade, 0.0-15.0
        self.incline_feedback = 0.0 # percent grade, 0.0-15.0

        self.speed_min = 0.5
        self.speed_max = 12.0

        self.incline_min = 0.0
        self.incline_max = 15.0

        self.setup_pins()
        self.start_threads()
        
    def setup_pins(self):
        pins = {
            "Reset": 17,
            "Start": 18,
            "Speed Up": 27,
            "Speed Down": 22,
            "Incline Up": 23,
            "Incline Down": 24,
        }

        self.reset = LED(pins["Reset"])
        self.start = LED(pins["Start"])
        self.speed_up = LED(pins["Speed Up"])
        self.speed_down = LED(pins["Speed Down"])
        self.incline_up = LED(pins["Incline Up"])
        self.incline_down = LED(pins["Incline Down"])

    def start_threads(self):
        self.run = True
        self.keep_speed_thread = threading.Thread(target=self.keep_speed)
        self.keep_incline_thread = threading.Thread(target=self.keep_incline)
        
        self.keep_speed_thread.start()
        self.keep_incline_thread.start()

    def set_state(self, state):
        if self.state == state:
            return
        self.state = state
        self.notify_state_changed(state)

    def notify_state_changed(self, state):
        for callback in self.state_changed_callbacks:
            callback(state)

    def handle_data(self, chars):
        self.lastUpdate = datetime.datetime.now()

    def handle_metric_changed(self, metric, value):

        if metric == TreadmillMetric.Timestamp:
            self.timestamp = value
        elif metric == TreadmillMetric.Incline:
            self.incline_feedback = value
            print(f"incline feedback changed to {value}")
        elif metric == TreadmillMetric.Speed:
            self.speed_feedback = value
            print(f"speed feedback changed to {value}")

        elif metric == TreadmillMetric.Distance:
            self.distance = value

    # Fast: Leave the pin on. This causes speed or incline to change rapidly.
    def pulse(self, led, duration_s = 0.1, then_wait_s = 0.1, fast: bool = False):
        print(f'pulse(pin={led.pin}, duration_s={duration_s}, then_wait_s={then_wait_s}, fast={fast})')
        led.on()

        if fast:
            return

        if duration_s > 0:
            sleep(duration_s)

        led.off()

        if then_wait_s > 0:
            sleep(then_wait_s)

    def diff_speed(self):
        return self.speed_feedback - self.speed_setpoint

    def diff_incline(self):
        return self.incline_feedback - self.incline_setpoint

    def rate_limit_speed(self):
        diff = self.diff_speed()

        if self.state not in [TreadmillState.Started]:
            sleep(0.5)
        elif abs(diff) < ZERO:
            sleep(0.5)
        if abs(diff) < 0.2:
            sleep(0.5)
        elif abs(diff) < 0.5:
            sleep(0.2)
        else:
            sleep(0.1)

        diff = self.diff_speed()
        return diff
        
    def rate_limit_incline(self):
        diff = self.diff_incline()
        
        if self.state not in [TreadmillState.Started]:
            sleep(0.5)
        elif abs(diff) < ZERO:
            sleep(0.5)
        elif abs(diff) < 1.0:
            sleep(0.5)
        elif abs(diff) < 2.0:
            sleep(0.2)
        else:
            sleep(0.1)

        diff = self.diff_incline()
        return diff

    def control_loop(self, rate_limit_func, interrupt_func, increment_func, decrement_func):
        
        while self.run:
            diff = rate_limit_func()

            if (interrupt_func is not None and interrupt_func()) \
                or not self.state_ok() \
                or self.setpoint_reached(diff):
                continue

            if diff < -1:
                increment_func(fast=True)
            elif diff < 0:
                increment_func()
            elif diff > 1: 
                decrement_func(fast=True)
            elif diff > 0:
                decrement_func()

    # Do not change speed or incline outside of the Started state
    def state_ok(self):
        self.validate_state()
        return self.state in [TreadmillState.Started]

    # Stop workout if we haven't heard from the treadmill in a while
    def validate_state(self):
        
        heartbeat = datetime.datetime.now() - self.lastUpdate

        if (self.state in [TreadmillState.Started] and heartbeat > self.heartbeat):
            print(f"No contact from treadmill in {self.heartbeat.total_seconds()}s. Ending workout.")
            self.end_workout()

    # Do not change speed or incline when the setpoint has been reached
    def setpoint_reached(self, diff):
        return abs(diff) < ZERO

    def keep_speed(self):

        # Do not change speed while incline is changing
        def defer_to_incline():
            return abs(self.diff_incline()) > ZERO

        self.control_loop(self.rate_limit_speed, defer_to_incline, self.increment_speed, self.decrement_speed)

    def keep_incline(self):

        self.control_loop(self.rate_limit_incline, None, self.increment_incline, self.decrement_incline)

    def go_to_speed(self, speed):
        if speed < self.speed_min or speed > self.speed_max:
            raise ValueError(f"Cannot set speed setpoint to {speed} because it is less than {self.speed_min} or greater than {self.speed_max}")

        if round(speed % 0.1, 5) not in [0.1, 0.0]:
            raise ValueError(f"Cannot set speed setpoint to {speed} because it is not a multiple of 0.1")

        self.speed_setpoint = speed
        return f"Setting speed setpoint to {speed}"

    def go_to_incline(self, incline):

        if incline < self.incline_min or incline > self.incline_max:
            raise ValueError(f"Cannot set incline setpoint to {incline} because it is less than {self.incline_min} or greater than {self.incline_max}")

        if round(incline % 0.5, 5) != 0.0:
            raise ValueError(f"Cannot set incline setpoint to {incline} because it is not a multiple of 0.5")

        self.incline_setpoint = incline
        return f"Setting incline setpoint to {incline}"

    def increment_speed(self, fast: bool = False):
        if self.state in [TreadmillState.Starting, TreadmillState.Started]:
            self.pulse(self.speed_up, then_wait_s=0.1, fast=fast)

    def decrement_speed(self, fast: bool = False):
        if self.state == TreadmillState.Started:
            self.pulse(self.speed_down, then_wait_s=0.1, fast=fast)

    def increment_incline(self, fast: bool = False):
        if self.state == TreadmillState.Started:
            self.pulse(self.incline_up, then_wait_s=0.25, fast=fast)

    def decrement_incline(self, fast: bool = False):
        if self.state == TreadmillState.Started:
            self.pulse(self.incline_down, then_wait_s=0.25, fast=fast)

    def start_workout(self):
        if self.state != TreadmillState.Ready:
            return

        self.set_state(TreadmillState.Starting)
        self.lastUpdate = datetime.datetime.now()
        self.pulse(self.start, then_wait_s=1) # Ready -> Starting
        self.increment_speed() # Bypasses 3, 2, 1 countdown
        self.speed_setpoint = 1.0
        self.set_state(TreadmillState.Started)

    def pause(self):
        if self.state == TreadmillState.Started:
            self.set_state(TreadmillState.Paused)
            self.release_pins()
            self.pulse(self.reset, then_wait_s=1) # Started -> Paused

    def resume(self):
        if self.state == TreadmillState.Paused:
            self.set_state(TreadmillState.Started)
            self.pulse(self.start, then_wait_s=1) # Paused -> Started

    def end_from_pause(self):
        if self.state == TreadmillState.Paused:
            self.set_state(TreadmillState.Summary)
            self.pulse(self.reset, then_wait_s=1) # Paused -> Summary

    def close_summary(self):
        if self.state == TreadmillState.Summary:
            self.set_state(TreadmillState.Ready)
            self.pulse(self.reset, then_wait_s=1) # Summary -> Ready

    def end_workout(self):
        
        self.pause()
        self.end_from_pause()
        self.close_summary()

        if self.state != TreadmillState.Ready:
            self.pulse(self.reset, then_wait_s=1)

        self.speed_setpoint = 0.0
        self.incline_setpoint = 0.0

    def release_pins(self):
        self.speed_up.off()
        self.speed_down.off()
        self.incline_up.off()
        self.incline_down.off()

    def close(self):
        self.run = False
        self.keep_speed_thread.join()
        self.keep_incline_thread.join()
