#include <arpa/inet.h>
#include <errno.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <string.h>

const char* broadcastAddr = "255.255.255.255";
const int broadcastPort = 7887;
struct sockaddr_in addr;
int sock = 0;

void setup_socket()
{
    if (sock > 0)
        return;

    sock = socket(PF_INET, SOCK_DGRAM, 0);

    if (sock < 0)
    {
        fprintf(stderr, "socket failed: %s\n", strerror(errno));
        return;
    }

    int broadcastPermission = 1;

    if  (setsockopt(sock, SOL_SOCKET, SO_BROADCAST, (void*) &broadcastPermission, sizeof(broadcastPermission)) < 0)
    {
        fprintf(stderr, "setsockopt failed: %s\n", strerror(errno));
        return;
    }

    memset(&addr, 0, sizeof(addr));
    addr.sin_family = AF_INET;
    addr.sin_addr.s_addr = inet_addr(broadcastAddr);
    addr.sin_port = htons(broadcastPort);
}

void broadcast(char* msg)
{
    setup_socket();
    if (sock <= 0)
    {
        fprintf(stderr, "Cannot send \"%s\": No socket\n", msg);
        return;
    }

    if (sendto(sock, msg, strlen(msg), 0, (struct sockaddr *) &addr, sizeof(addr)) < 0)
        fprintf(stderr, "sendto failed for \"%s\": %s\n", msg, strerror(errno));
}

void notify(char* msg)
{
    printf(msg);
    broadcast(msg);
    return;
}