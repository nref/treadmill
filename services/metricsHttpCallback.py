import requests

from treadmillMetric import TreadmillMetric

class MetricsHttpCallback:
    
    def __init__(self, url):
        self.url = url

    def handle_metric_changed(self, metric, value):
        requests.post(self.url, { metric, value })