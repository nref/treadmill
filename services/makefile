run:
	$(MAKE) -C i2c
	sudo ./i2c/bin/i2c > /dev/null &
	python3 -u main.py

run-test:
	$(MAKE) -C i2c
	sudo ./i2c/bin/i2c > /dev/null &
	python3 -u main.py real test

run-fake:
	sudo python3 -u main.py fake

run-fake-test:
	sudo python3 -u main.py fake test
