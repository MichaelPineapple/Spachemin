using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpachEngine;

public struct Input
{
    public readonly bool Quit;
    public readonly bool W, A, S, D;

    public Input()
    {
        Quit = false;
        W = A = S = D = false;
    }
    
    public Input(KeyboardState keyboard)
    {
        Quit = keyboard.IsKeyDown(Keys.Escape);
        W = keyboard.IsKeyDown(Keys.W);
        A = keyboard.IsKeyDown(Keys.A);
        S = keyboard.IsKeyDown(Keys.S);
        D = keyboard.IsKeyDown(Keys.D);
    }
    
    public Input(byte[] data)
    {
        bool[] bools = ByteToBools(data[0]);
        Quit = bools[0];
        W = bools[1];
        A = bools[2];
        S = bools[3];
        D = bools[4];
    }

    public byte[] ToBytes()
    {
        bool[] bools = new [] { Quit, W, A, S, D };
        return new[] { ByteFromBools(bools) };
    }
    
    private static byte ByteFromBools(bool[] bools)
    {
        int len = bools.Length;
        if (len > 8) throw new Exception("Data cannot be longer than 8!");
        byte output = 0;
        byte x = 1;
        const byte two = 2;
        for (int i = 0; i < bools.Length; i++)
        {
            if (bools[i]) output |= x;
            x *= two;
        }
        return output;
    }
    
    private static bool[] ByteToBools(byte bytes)
    {
        bool[] output = new bool[8];
        byte x = 1;
        const byte two = 2;
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = (bytes & x) != 0;
            x *= two;
        }
        return output;
    }
}