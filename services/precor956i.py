import threading

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
        self.timestamp = "00:00"
        self.distance = 0.0 # mi or km
        self.speed_setpoint = 0.0 # mi/h or km/h
        self.speed_feedback = 0.0 # mi/h or km/h

        self.incline_setpoint = 0.0 # percent grade, 0.0-15.0
        self.incline_feedback = 0.0 # percent grade, 0.0-15.0

        self.speed_expected = 0.0 # The speed that would be expected from the count of pulses on the speed pins
        self.incline_expected = 0.0 # The incline that would be expected from the count of pulses on the incline pins

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

    def pulse(self, led, duration_s = 0.1, then_wait_s = 0.1):
        print(f'pulse({led.pin}, {duration_s}, {then_wait_s})')
        led.on()

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

            if diff < 0:
                increment_func()
            elif diff > 0:
                decrement_func()

    def validate_state(self):
        
        if (self.state in [TreadmillState.Started] and self.speed_feedback < ZERO):
            print(f"speed is zero. Assuming paused.")
            self.state = TreadmillState.Paused

        if (self.state not in [TreadmillState.Started] and self.speed_feedback > ZERO):
            print(f"speed is nonzero. Assuming started.")
            self.state = TreadmillState.Started

    # Do not change speed or incline outside of the Started state
    def state_ok(self):
        self.validate_state()
        return self.state in [TreadmillState.Started]

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

    def increment_speed(self):
        if self.state in [TreadmillState.Starting, TreadmillState.Started]:
            self.pulse(self.speed_up, then_wait_s=0)
            self.speed_expected += 0.1

    def decrement_speed(self):
        if self.state == TreadmillState.Started:
            self.pulse(self.speed_down, then_wait_s=0)
            self.speed_expected -= 0.1

    def increment_incline(self):
        if self.state == TreadmillState.Started:
            self.pulse(self.incline_up, then_wait_s=0)
            self.incline_expected += 0.5

    def decrement_incline(self):
        if self.state == TreadmillState.Started:
            self.pulse(self.incline_down, then_wait_s=0)
            self.incline_expected -= 0.5

    def start_workout(self):
        if self.state != TreadmillState.Ready:
            return

        self.state = TreadmillState.Starting
        self.pulse(self.start) # Ready -> Starting
        self.increment_speed() # Bypasses 3, 2, 1 countdown
        self.speed_setpoint = 1.0
        self.speed_expected = 1.0
        self.state = TreadmillState.Started

    def pause(self):
        if self.state == TreadmillState.Started:
            self.pulse(self.reset, then_wait_s=0.5) # Started -> Paused
            self.state = TreadmillState.Paused

    def resume(self):
        if self.state == TreadmillState.Paused:
            self.pulse(self.start, then_wait_s=0.5) # Paused -> Started
            self.state = TreadmillState.Started

    def end_from_pause(self):
        if self.state == TreadmillState.Paused:
            self.pulse(self.reset, then_wait_s=0.5) # Paused -> Summary
            self.state = TreadmillState.Summary

    def close_summary(self):
        if self.state == TreadmillState.Summary:
            self.pulse(self.reset, then_wait_s=0.5) # Summary -> Ready
        self.state = TreadmillState.Ready

    def end_workout(self):
        
        self.pause()
        self.end_from_pause()
        self.close_summary()

        if self.state != TreadmillState.Ready:
            self.pulse(self.reset, then_wait_s=0.5)

        self.speed_setpoint = 0.0
        self.incline_setpoint = 0.0
        self.speed_expected = 0.0
        self.incline_expected = 0.0

    def close(self):
        self.run = False
        self.keep_speed_thread.join()
        self.keep_incline_thread.join()