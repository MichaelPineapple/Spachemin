#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <pthread.h>
#include <unistd.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <sys/types.h>

void* clientThread(void* _client)
{
    int client = (int)(size_t) _client;
    printf("Client #%d Connected!\n", client);
    fflush(stdout);
    
    while (true)
    {
        char data[255] = "";
        int r = recv(client, data, sizeof(data), 0);
        if (r <= 0) break;
        printf("Client %d> %s\n", client, data);
        fflush(stdout);
        send(client, data, sizeof(data), 0);
    }
    
    printf("Client %d Disconnected!\n", client);
    fflush(stdout);
    return NULL;
}

int startServer()
{
    int sock = socket(AF_INET, SOCK_STREAM, 0);
    struct sockaddr_in addr;
    addr.sin_family = AF_INET;
    addr.sin_port = htons(9001);
    addr.sin_addr.s_addr = INADDR_ANY;
    bind(sock, (struct sockaddr*)&addr, sizeof(addr));
    return sock;
}

int awaitConnection(int sock)
{
    listen(sock, 1);
    return accept(sock, NULL, NULL);
}

pthread_t startThread(int client)
{
    pthread_t thread;
    pthread_create(&thread, NULL, clientThread, (void*)(size_t)client);
    return thread;
}

void joinThreads(pthread_t* threads)
{
    int n = sizeof(threads);
    for (int i = 0; i < n; i++) pthread_join(threads[i], NULL);
}

void connectionLoop(int sock)
{
    pthread_t threads[100];
    int i = 0;
    while (i < 100)
    {
        int client = awaitConnection(sock);
        threads[i] = startThread(client);
        i++;
    }
    
    joinThreads(threads);
}

int main(int argc, char const* argv[])
{
    printf("Spachemin Server!\n");
    fflush(stdout);
    
    int sock = startServer();
    connectionLoop(sock);
    
    return 0;
}
