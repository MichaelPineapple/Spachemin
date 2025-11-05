int bytesToInt(unsigned char bytes[])
{
    int output = 0;
    output += bytes[3];
    output += bytes[2] << 8;
    output += bytes[1] << 16;
    output += bytes[0] << 24;
    return output;
}

bool checkBools(bool array[], int len)
{
    for (int i = 0; i < len; i++)
    {
        if (!array[i]) return false;
    }
    return true;
}

void clearBools(bool array[], int len, bool val)
{
    for (int i = 0; i < len; i++) array[i] = val;
}

void clearData(char data[], int len, char val)
{
    for (int i = 0; i < len; i++) data[i] = val;
}

void copyData(char src[], char dst[], int len, int srcOffset)
{
    for (int i = 0; i < len; i++) dst[i] = src[i + srcOffset];
}

void printData(char data[], int len)
{
    for (int i = 0; i < len; i++) printf("%d ", data[i]);
    printf("\n");
}
