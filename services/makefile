run:
	$(MAKE) -C i2c
	sudo nice -n 0 ./i2c/bin/i2c >> i2c.log 2>&1 &
	python3 -u main.py >> main.log 2>&1 &
	tail -f main.log

stop:
	sudo pkill -f main.py
	sudo pkill i2c

run-test:
	$(MAKE) -C i2c
	sudo ./i2c/bin/i2c > /dev/null &
	python3 -u main.py real test

run-fake:
	sudo python3 -u main.py fake

run-fake-test:
	sudo python3 -u main.py fake test
