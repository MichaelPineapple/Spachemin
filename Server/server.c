#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <pthread.h>
#include <unistd.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <sys/types.h>

#include "framequeue.c"

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

void syncPlayers(int players[], int playerCount, int frameDelay)
{
    for (int i = 0; i < playerCount; i++)
    {
        char login[3];
        login[0] = (char)playerCount;
        login[1] = (char)i;
        login[2] = (char)frameDelay;
        send(players[i], login, 3, 0);
    }
    
    bool sync[playerCount];
    clearBools(sync, playerCount, false);
    while (!checkBools(sync, playerCount))
    {
        for (int i = 0; i < playerCount; i++)
        {
            int player = players[i];
            char buffer[1];
            recv(player, buffer, 1, 0);
            if (buffer[0] == 69) sync[i] = true;
        }
    }
}

void awaitPlayers(int sock, int players[], int playerCount)
{
    for (int i = 0; i < playerCount; i++)
    {
        listen(sock, 1);
        players[i] = accept(sock, NULL, NULL);
        printf("Player %d Connected\n", i);
        fflush(stdout);
    }
}

void recieveTrans(FrameQueue* q, int players[], int playerCount)
{
    for (int i = 0; i < playerCount; i++)
    {
        int bufferLen = MAX_DATA + 4;
        char buffer[bufferLen];
        int r = recv(players[i], buffer, bufferLen, MSG_DONTWAIT);
        if (r > 0)
        {
            int frameIndex = bytesToInt((unsigned char*)buffer);
            printf("From Player %d (%d)> ", i, frameIndex);
            printData(buffer, bufferLen);
            put(q, frameIndex, i, buffer, 4);
        }
    }
}

bool sendTrans(FrameQueue* q, int players[], int playerCount)
{
    FrameData frame = get(q);
    for (int i = 0; i < playerCount; i++)
    {
        for (int j = 0; j < playerCount; j++)
        {
            int r = send(players[j], frame.data[i], MAX_DATA, 0);
            if (r <= 0) return false;
            printf("To Player %d (%d)> ", j, q->current - 1);
            printData(frame.data[i], MAX_DATA);
        }
    }
    return true;
}

void coreLoop(int players[], int playerCount, int frameDelay)
{
    FrameQueue q;
    initializeFrameQueue(&q, playerCount);
    addDelay(&q, frameDelay);
    
    int t = 0;
    while (true)
    {
        recieveTrans(&q, players, playerCount);
        
        if (hasNext(&q))
        {
            if (!sendTrans(&q, players, playerCount)) break;
        }
        
        usleep(10000);
        //printf("%d\n", t);
        fflush(stdout);
        t++;
    }
}

int main(int argc, char const* argv[])
{
    printf("*** Spachemin Server ***\n");
    fflush(stdout);
        
    int playerCount = atoi(argv[1]);
    int frameDelay = atoi(argv[2]);
    int players[playerCount];
    
    int sock = startServer();
    awaitPlayers(sock, players, playerCount);
    printf("All Players Connected\n");
    fflush(stdout);
    
    syncPlayers(players, playerCount, frameDelay);
    printf("Player Sync\n");
    fflush(stdout);

    coreLoop(players, playerCount, frameDelay);
    
    return 0;
}
