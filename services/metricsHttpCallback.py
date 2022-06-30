import json
import requests

from treadmillMetric import TreadmillMetric

class MetricsHttpCallback:
    
    def __init__(self, url):
        self.url = url

    def handle_metric_changed(self, state, metric, value):
        data = { "state": state, "metric": metric, "value": value } 
        requests.post(self.url, json.dumps(data, default=str))
