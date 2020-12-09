# Install dependencies
sudo apt-get install python3-distutils
sudo apt-get install python3-gpiozero
suto apt-get install git
git clone https://github.com/joan2937/pigpio
cd pigpio
git checkout v77
make
sudo make install
cd ..

# Install service
cp treadmill.service /etc/systemd/system/
systemctl daemon-reload
systemctl enable treadmill
service treadmill start