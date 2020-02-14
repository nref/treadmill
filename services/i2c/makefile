i2c: src/i2c.c include/notify.h
	mkdir -p bin
	gcc -o bin/i2c src/i2c.c -Iinclude -Wall -Wpedantic -lpigpio -lrt -lgcc_s
	
i2c-cs: src/i2c.cs
	mcs src/i2c.cs
