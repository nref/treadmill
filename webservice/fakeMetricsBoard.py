from datetime import datetime, timedelta
from time import sleep

from metricsBoardBase import MetricsBoardBase
from treadmillState import TreadmillState

def strfdelta(tdelta, fmt):
    d = {"days": tdelta.days}
    d["H"], rem = divmod(tdelta.seconds, 3600)
    d["M"], d["S"] = divmod(rem, 60)
    return fmt.format(**d)

class FakeMetricsBoard(MetricsBoardBase):

    def __init__(self, treadmill):
        self.treadmill = treadmill

        super().__init__(treadmill)

    def read_forever(self):
        """
            Fake a metrics board which just outputs the expected speed and incline
            and counts the elapsed time and integrates distance from speed over time.
        """
        end_time = datetime.now() - timedelta(minutes = 60)
        simulated_distance = 0

        while self.run:
            if self.treadmill.state != TreadmillState.Started:
                simulated_distance = 0
                sleep(1)
                continue

            simulated_timestamp = strfdelta(end_time - datetime.now(), "{M:02d}:{S:02d}")
            simulated_distance += self.speed_feedback/3600 # 3600s in an hour
            simulated_speed_feedback = self.treadmill.speed_expected
            simulated_incline_feedback = self.treadmill.incline_expected

            self.handle_data(f"  {simulated_timestamp}  {simulated_incline_feedback:0.1f}   {simulated_speed_feedback:0.1f}   {simulated_distance:0.2f}")
            sleep(0.1)
