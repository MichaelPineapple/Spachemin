#define MAX_FRAMES 100
#define MAX_DATA 3
#define MAX_PLAYERS 3

typedef struct
{
    char data[MAX_PLAYERS][MAX_DATA];
    bool valid[MAX_PLAYERS];
} FrameData;

typedef struct
{
    FrameData array[MAX_FRAMES];
    int current;
    int playerCount;
} FrameQueue;

void clearFrameData(FrameData* frame, char val)
{
    for (int i = 0; i < MAX_PLAYERS; i++)
    {
        clearData(frame->data[i], MAX_DATA, val);
        frame->valid[i] = false;
    }
}

void clearFrameQueue(FrameQueue* q, char val)
{
    for (int i = 0; i < MAX_FRAMES; i++) clearFrameData(&q->array[i], val);
}

void initializeFrameQueue(FrameQueue* q, int playerCount)
{
    q->current = 0;
    q->playerCount = playerCount;
    clearFrameQueue(q, 0);
}

void put(FrameQueue* q, int frameIndex, int playerId, char data[], int offset)
{
    int i = frameIndex % MAX_FRAMES;
    FrameData* frame = &q->array[i];
    if (!frame->valid[playerId])
    {
        copyData(data, frame->data[playerId], MAX_DATA, offset);
        frame->valid[playerId] = true;
    }
    else 
    {
        printf("ERROR - Frame %d & PlayerId %d Is Already Set!\n", i, playerId);
        fflush(stdout);
    }
}

bool hasNext(FrameQueue* q)
{
    FrameData frame = q->array[q->current];
    return checkBools(frame.valid, q->playerCount);
}

FrameData get(FrameQueue* q)
{
    FrameData frame = q->array[q->current];
    if (!checkBools(frame.valid, q->playerCount))
    {
        printf("ERROR - Invalid Frame %d!\n", q->current);
        fflush(stdout);
    }
    clearBools(q->array[q->current].valid, MAX_PLAYERS, false);
    q->current++;
    if (q->current >= MAX_FRAMES) q->current = 0;
    return frame;
}

void printFrame(FrameData* frame)
{
    for (int i = 0; i < MAX_PLAYERS; i++) 
    {
        printf("%d:", i);
        printData(frame->data[i], MAX_DATA);
    }
}

void addDelay(FrameQueue* q, int delay)
{
    for (int i = 0; i < delay; i++) clearBools(q->array[i].valid, MAX_PLAYERS, true);
}
