import threading

from datetime import datetime
from treadmillMetric import TreadmillMetric

ZERO = 1e-9

class MetricsBoardBase:

    def __init__(self, treadmill):
        self.run = True
        self.treadmill = treadmill
        self.treadmill.state_changed_callbacks.append(self.handle_state_changed)
        self.metrics_callbacks = {}
        
        self.timestamp = "00:00"
        self.distance = 0.0
        self.speed_feedback = 0.0
        self.incline_feedback = 0.0

        self.run_thread = threading.Thread(target=self.read_forever)
        self.run_thread.start()

    def read_forever(self):
        pass

    def add_metrics_callback(self, callback):
        if callback.url not in self.metrics_callbacks:
            print(f'Adding metrics callback {callback.url}')
            self.metrics_callbacks[callback.url] = callback

    def remove_metrics_callback(self, url):
        self.metrics_callbacks.pop(url, None)

    def notify_metric_changed(self, metric, value):
        self.treadmill.handle_metric_changed(metric, value)

        # Do not modify collection while iterating
        callbacks_to_remove = []

        for url, callback in self.metrics_callbacks.items(): 
            try:
                callback.handle_metric_changed(metric, value)
            except Exception as e:
                print(str(e))
                callbacks_to_remove.append(url)
        
        for url in callbacks_to_remove:
            print(f'Removing metrics callback: {url}')
            self.remove_metrics_callback(url)

    def close(self):
        self.run = False
        self.run_thread.join()

    def handle_state_changed(self, state):
        self.notify_metric_changed(TreadmillMetric.State, state)

    def handle_data(self, chars):
        """
            Parse the given string as Precor 956i metrics.
            Tested also for Precor 946i.
            Sample data: "  58:43  0.5   7.2   0.15"
        """
        
        self.treadmill.handle_data(chars) 

        #print(chars)
        timestamp = chars[2:7]
        incline = chars[9:13]
        speed = chars[15:19]
        distance = chars[20:25]

        try:
            new_speed_feedback = float(speed)

            if abs(self.speed_feedback - new_speed_feedback) > ZERO:
                self.speed_feedback = new_speed_feedback
                self.notify_metric_changed(TreadmillMetric.Speed, new_speed_feedback)

        except ValueError:
            pass
        
        try:
            new_incline_feedback = float(incline)

            if abs(self.incline_feedback - new_incline_feedback) > ZERO:
                self.incline_feedback = new_incline_feedback
                self.notify_metric_changed(TreadmillMetric.Incline, new_incline_feedback)

        except ValueError:
            pass

        try:
            # When changing speed this shows pace
            if ":" not in distance:

                new_distance = float(distance)

                if abs(self.distance - new_distance) > ZERO:
                    self.distance = new_distance
                    self.notify_metric_changed(TreadmillMetric.Distance, new_distance)

        except ValueError:
            pass

        try:
            new_timestamp = datetime.strptime(timestamp, "%H:%M")

            if self.timestamp != new_timestamp:
                self.timestamp = new_timestamp
                self.notify_metric_changed(TreadmillMetric.Timestamp, new_timestamp)

        except ValueError:
            try:
                new_timestamp = datetime.strptime(timestamp, "%M:%S")

                if self.timestamp != new_timestamp:
                    self.timestamp = new_timestamp
                    self.notify_metric_changed(TreadmillMetric.Timestamp, new_timestamp)

            except ValueError:
                pass
