curl -d 'test' 192.168.1.152:8000/bad
curl 192.168.1.152:8000/bad

curl -d '7.5' 192.168.1.152:8000/speed/setpoint
curl -d '2.5'192.168.1.152:8000/incline/setpoint

curl 192.168.1.152:8000/state
curl 192.168.1.152:8000/speed/setpoint
curl 192.168.1.152:8000/incline/setpoint
curl 192.168.1.152:8000/speed/feedback
curl 192.168.1.152:8000/incline/feedback

curl -d 'anything' 192.168.1.152:8000/start
curl -d 'anything' 192.168.1.152:8000/pause
curl -d 'anything' 192.168.1.152:8000/resume
curl -d 'anything' 192.168.1.152:8000/end
