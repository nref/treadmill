run:
	$(MAKE) -C i2c
	sudo nice -n 0 ./i2c/bin/i2c >> i2c.log 2>&1 &
	python3 -u main.py >> main.log 2>&1 &
	tail -n 0 -f main.log

stop:
	sudo pkill -f main.py
	sudo pkill i2c

test:
	$(MAKE) -C i2c
	sudo ./i2c/bin/i2c > /dev/null &
	python3 -u main.py real test

fake:
	sudo python3 -u main.py fake

fake-test:
	sudo python3 -u main.py fake test
