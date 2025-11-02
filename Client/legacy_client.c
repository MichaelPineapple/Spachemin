#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <stdbool.h>
#include <unistd.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <sys/types.h>

int main(int argc, char const* argv[])
{
    // 44.245.211.131
    // 127.0.0.1
    
    printf("Connecting to %s ...\n", argv[1]);

    int sock = socket(AF_INET, SOCK_STREAM, 0);
    struct sockaddr_in addr;

    addr.sin_family = AF_INET;
    addr.sin_port = htons(9001);
    addr.sin_addr.s_addr = inet_addr(argv[1]);

    int connectStatus = connect(sock, (struct sockaddr*)&addr, sizeof(addr));

    if (connectStatus == -1) printf("Error!\n");
    else 
    {
        printf("Connected!\n");
        
        while (true)
        {
            char data[255];
            scanf("%s", data);
            clock_t t = clock();
            send(sock, data, sizeof(data), 0);
            recv(sock, data, sizeof(data), 0);
            t = clock() - t;
            double latency = ((double)t) / CLOCKS_PER_SEC;
            printf("Server> %s (%f)\n", data, latency);
        }
    }
    
    return 0;
}
