#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <pthread.h>
#include <unistd.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <sys/types.h>

#define DATA_SIZE 255
#define PORT 9001

int startServer()
{
    int sock = socket(AF_INET, SOCK_STREAM, 0);
    struct sockaddr_in addr;
    addr.sin_family = AF_INET;
    addr.sin_port = htons(PORT);
    addr.sin_addr.s_addr = INADDR_ANY;
    bind(sock, (struct sockaddr*)&addr, sizeof(addr));
    return sock;
}

int awaitTransmissions(int players[], int playerCount, char data[][DATA_SIZE])
{
    for (int i = 0; i < playerCount; i++)
    {
        int player = players[i];
        for (int j = 0; j < DATA_SIZE; j++) data[i][j] = '\0';
        int r = recv(player, data[i], sizeof(data[i]), 0);
        if (r <= 0) return -1;
        printf("From Player %d> %s\n", i, data[i]);
        fflush(stdout);
    }
    return 0;
}

int broadcastTransmissions(int players[], int playerCount, char data[][DATA_SIZE])
{
    for (int i = 0; i < playerCount; i++)
    {
        int player = players[i];
        for (int j = 0; j < playerCount; j++)
        {
            int r = send(player, data[j], DATA_SIZE, 0);
            if (r <= 0) return -1;
            printf("To Player %d> %s\n", i, data[j]);
            fflush(stdout);
        }
    }
    return 0;
}

void awaitPlayers(int sock, int players[], int playerCount)
{
    for (int i = 0; i < playerCount; i++)
    {
        listen(sock, 1);
        players[i] = accept(sock, NULL, NULL);
        char login[2];
        login[0] = (char)playerCount;
        login[1] = (char)i;
        send(players[i], login, 2, 0);
        printf("Player %d Connected\n", i);
        fflush(stdout);
    }
}

int main(int argc, char const* argv[])
{
    printf("*** Spachemin Server ***\n");
    fflush(stdout);
        
    int playerCount = atoi(argv[1]);
    int players[playerCount];
    char data[playerCount][DATA_SIZE];
    
    int sock = startServer();
    awaitPlayers(sock, players, playerCount);
    printf("All Players Connected\n");
    fflush(stdout);
    while (true)
    {
        if (awaitTransmissions(players, playerCount, data) < 0) return -1;
        if (broadcastTransmissions(players, playerCount, data) < 0) return -1;
    }
    
    return 0;
}
