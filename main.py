import signal 
import sys

from gpiozero import Device
from gpiozero.pins.mock import MockFactory
from time import sleep

from treadmillServer import TreadmillServer
from fakeMetricsBoard import FakeMetricsBoard
from metricsBoard import MetricsBoard
from precor956i import Precor956i

def close(tm, metrics):
    tm.end_workout() 
    tm.close()
    metrics.close()

def test_main(tm, metrics):
    
    def handle_sigint(sig, frame):
        close(tm, metrics)
        sys.exit(0)

    signal.signal(signal.SIGINT, handle_sigint)

    # Error conditions
    tm.go_to_speed(0.55)
    tm.go_to_incline(0.1)

    tm.start_workout()

    for speed in [2, 3]:
         tm.go_to_speed(speed)
         sleep(10)

    tm.go_to_speed(1.0) # Test that incline takes priority over speed
    for incline in [5.0, 0.0]:
       tm.go_to_incline(incline)
       sleep(10)

def main(fake = False, test = False):

    if fake:
        Device.pin_factory = MockFactory()

    tm = Precor956i()
    metrics = FakeMetricsBoard(tm) if fake else MetricsBoard(tm)

    if test:
        test_main(tm, metrics)
    else:
        TreadmillServer(tm, metrics)

    close(tm, metrics)

if __name__ == '__main__':
    fake = len(sys.argv) > 1 and sys.argv[1] == "fake"
    test = len(sys.argv) > 2 and sys.argv[2] == "test"

    main(fake, test)