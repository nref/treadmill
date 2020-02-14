#include <arpa/inet.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <string.h>

const char* broadcastAddr = "192.168.1.255";
const int broadcastPort = 7887;

void notify(char* msg)
{
    printf(msg);
    return;
    
    struct sockaddr_in addr;
    int sock;
    sock = socket(PF_INET, SOCK_DGRAM, IPPROTO_UDP);

    if (sock < 0)
    {
        fprintf(stderr, "socket failed for %s", msg);
        return;
    }

    int broadcastPermission = 1;
    int ok = setsockopt(sock, SOL_SOCKET, SO_BROADCAST, (void*) &broadcastPermission, sizeof(broadcastPermission));

    if  (ok < 0)
    {
        fprintf(stderr, "setsockopt failed for %s", msg);
        return;
    }

    memset(&addr, 0, sizeof(addr));
    addr.sin_family = AF_INET;
    addr.sin_addr.s_addr = inet_addr(broadcastAddr);
    addr.sin_port = htons(broadcastPort);

    int len = strlen(msg);
    
    ok = sendto(sock, msg, len, 0, (struct sockaddr *) &addr, sizeof(addr));

    if (ok < 0)
    {
        fprintf(stderr, "sendto failed for %s", msg);
    }
}