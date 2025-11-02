#define DATA_MAX 3
#define PLAYER_MAX 5
#define QUEUE_MAX 100

typedef struct
{
    char data[PLAYER_MAX][DATA_MAX];
} TickData;

typedef struct
{
    TickData array[QUEUE_MAX];
    int head;
    int tail;
} TickQueue;

void setTickData(TickData* td, char c)
{
    for (int i = 0; i < PLAYER_MAX; i++)
    {
        for (int j = 0; j < DATA_MAX; j++) td->data[i][j] = c;
    }
}

void constructTickQueue(TickQueue* q)
{
    q->head = -1;
    q->tail = 0;
    for (int i = 0; i < QUEUE_MAX; i++) setTickData(&q->array[i], '0');
}

void enqueue(TickQueue* q, TickData value)
{
    if ((q->tail - q->head) > (QUEUE_MAX - 1)) printf("NO MORE SPACE!");
    if (q->tail >= QUEUE_MAX) q->tail = 0;
    q->array[q->tail] = value;
    q->tail++;
}

TickData dequeue(TickQueue* q)
{
    q->head++;
    if (q->head >= QUEUE_MAX) q->head = 0;
    if (q->head == q->tail) printf("Queue Empty! (head=%d, tail=%d)\n", q->head, q->tail);
    return q->array[q->head];
}
