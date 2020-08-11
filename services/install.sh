cp treadmill.service /etc/systemd/system/
systemctl daemon-reload
systemctl enable treadmill
service treadmill start