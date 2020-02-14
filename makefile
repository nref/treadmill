i2c: i2c.c notify.h
	gcc -o i2c i2c.c -Wall -Wpedantic -lpigpio -lrt
	
i2c-cs: i2c.cs
	mcs i2c.cs

run:
	sudo python3 -u main.py

run-test:
	sudo python3 -u main.py real test

run-fake:
	sudo python3 -u main.py fake

run-fake-test:
	sudo python3 -u main.py fake test
