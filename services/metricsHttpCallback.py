import json
import requests

from treadmillMetric import TreadmillMetric

class MetricsHttpCallback:
    
    def __init__(self, url):
        self.url = url

    def handle_metric_changed(self, metric, value):
        data = { "metric": metric, "value": value } 
        requests.post(self.url, json.dumps(data, default=str))
