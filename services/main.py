import signal 
import sys

from gpiozero import Device
from gpiozero.pins.mock import MockFactory
from time import sleep

from treadmillServer import TreadmillServer
from fakeMetricsBoard import FakeMetricsBoard
from metricsBoard import MetricsBoard
from precor956i import Precor956i
from gpiozero import LED

def close(tm, metrics):
    tm.end_workout() 
    tm.close()
    metrics.close()

def test_main(tm, metrics):

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

def test_incline():
    
    incl_up = LED(23)
    incl_down = LED(24)

    while (True):
        print("incl up")
        incl_up.on()
        sleep(0.2)
        incl_up.off()
        sleep(5)

        print("incl down")
        incl_down.on()
        sleep(0.2)
        incl_down.off()
        sleep(5)

def test_speed():
    
    start = LED(18)
    stop = LED(17)
    speed_up = LED(27)
    speed_down = LED(22)

    sleep(2)
    print("start")
    start.on()
    sleep(1)
    start.off()
    sleep(5)

    i = 0
    while (i < 2):
        i+=1

        print("speed up")
        speed_up.on()
        sleep(2)
        speed_up.off()
        sleep(5)

        print("speed down")
        speed_down.on()
        sleep(2)
        speed_down.off()
        sleep(5)

    print("stop")
    stop.on()
    sleep(1)
    stop.off()

def test_stop():
    stop = LED(17)

    while (True):
        print("stop")
        stop.on()
        sleep(0.2)
        stop.off()
        sleep(2)

def main(fake = False, test = False):
    # test_incline()
    # test_speed()
    # test_stop()
    # return
    
    if fake:
        Device.pin_factory = MockFactory()

    tm = Precor956i()
    metrics = FakeMetricsBoard(tm) if fake else MetricsBoard(tm)
    
    def handle_sigint(sig, frame):
        close(tm, metrics)
        sys.exit(0)

    signal.signal(signal.SIGINT, handle_sigint)

    if test:
        test_main(tm, metrics)
    else:
        TreadmillServer(tm, metrics)

    close(tm, metrics)

if __name__ == '__main__':
    fake = len(sys.argv) > 1 and sys.argv[1] == "fake"
    test = len(sys.argv) > 2 and sys.argv[2] == "test"

    main(fake, test)