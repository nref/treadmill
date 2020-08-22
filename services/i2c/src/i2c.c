#include <ctype.h>
#include <signal.h>
#include <stdio.h>
#include <time.h>
#include "pigpio.h"
#include "notify.h"

#define SCL_FALLING 0
#define SCL_RISING  1
#define SCL_STEADY  2

#define SDA_FALLING 0
#define SDA_RISING  4
#define SDA_STEADY  8

// Each message from the Precor treadmill metrics board should have this many chars
#define MSG_LEN 26

char buffer[MSG_LEN];
int received_length = 0;

static volatile sig_atomic_t keep_running = 1;
int scl_pin = 9;
int sda_pin = 10;
int scl = 1;
int sda = 1;

void parse_i2c(int SCL, int SDA)
{
    static int in_data=0, byte=0, bit=0;
    static int oldSCL=1, oldSDA=1;
 
    int xSCL, xSDA;

    if (SCL != oldSCL)
    {
        oldSCL = SCL;
        if (SCL) xSCL = SCL_RISING;
        else     xSCL = SCL_FALLING;
    }
    else         xSCL = SCL_STEADY;
 
    if (SDA != oldSDA)
    {
        oldSDA = SDA;
        if (SDA) xSDA = SDA_RISING;
        else     xSDA = SDA_FALLING;
    }
    else         xSDA = SDA_STEADY;
 
    switch (xSCL+xSDA)
    {
        case SCL_RISING + SDA_RISING:
        case SCL_RISING + SDA_FALLING:
        case SCL_RISING + SDA_STEADY:
            if (in_data)
            {
                if (bit++ < 8)
                {
                    byte <<= 1;
                    byte |= SDA;
                }
                else
                {
                    // Replace any unprintable character with a space
                    if (isprint(byte))
                    {
                        sprintf(&buffer[received_length++], "%c", (char)byte);
                    }
                    else 
                    {
                        sprintf(&buffer[received_length++], " ");
                    }

                    bit = 0;
                    byte = 0;
                }
            }
            break;
  
        case SCL_FALLING + SDA_RISING:
            break;
  
        case SCL_FALLING + SDA_FALLING:
            break;
  
        case SCL_FALLING + SDA_STEADY:
            break;
  
        case SCL_STEADY + SDA_RISING:
            if (SCL)
            {
                // printf("message end. len=%d, bit=%d\n",received_length,bit);
                in_data = 0;
                byte = 0;
                bit = 0;

                // Should have received the whole message now: not more, not less
                if (received_length != MSG_LEN)
                {
                    received_length = 0;
                    break;
                }
                
                sprintf(&buffer[received_length++], "\n"); // message end
                notify(buffer);
            }
            break;
  
        case SCL_STEADY + SDA_FALLING:
            if (SCL)
            {
                in_data = 1;
                byte = 0;
                bit = 0;
                // message start
            }
            break;
  
        case SCL_STEADY + SDA_STEADY:
            break;
 
    }

    // Get ready to receive next message
    if (received_length > MSG_LEN)
        received_length = 0;
}

void handle_interrupt(int _) 
{
    (void)_;
    keep_running = 0;
}

void handle_scl_changed(int gpio, int level, uint32_t tick)
{
    // printf("handle_scl_changed: %d\n", level);
    if (gpio != scl_pin)
        return;

    scl = level;
    parse_i2c(scl, sda);
}

void handle_sda_changed(int gpio, int level, uint32_t tick)
{
    // printf("handle_sda_changed: %d\n", level);
    if (gpio != sda_pin)
        return;

    sda = level;
    parse_i2c(scl, sda);
}

void sleep_for(long seconds, long nanoseconds)
{
    struct timespec ts;
    ts.tv_sec = seconds;
    ts.tv_nsec = nanoseconds;
    clock_nanosleep(CLOCK_MONOTONIC, 0, &ts, NULL);
}

int setupGpio()
{
    gpioCfgBufferSize(120);
    gpioCfgClock(1, 0, -1);

    if (gpioInitialise() < 0)
    {
        fprintf(stderr, "gpioInitialise() failed. Kill any other instances of this program and ensure pigpiod is not running.\n");
        return -1;
    }

    gpioSetSignalFunc(SIGINT, handle_interrupt);
    gpioSetAlertFunc(scl_pin, handle_scl_changed);
    gpioSetAlertFunc(sda_pin, handle_sda_changed);

    gpioSetMode(scl_pin, PI_INPUT);
    gpioSetMode(sda_pin, PI_INPUT);

    gpioSetPullUpDown(scl_pin, PI_PUD_UP);
    gpioSetPullUpDown(sda_pin, PI_PUD_UP);

    return 0;
}

int main(int argc, char* argv[])
{
    printf("scl: %d\n", scl_pin);
    printf("sda: %d\n", sda_pin);

    // Don't buffer stdout
    setbuf(stdout, NULL);

    if (setupGpio() < 0)
        return -1;

    while (keep_running)
    {
        sleep_for(1, 0);
    }

    gpioTerminate();
}
