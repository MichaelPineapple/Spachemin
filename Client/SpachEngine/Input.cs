using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpachEngine;

public struct Input
{
    public readonly bool Quit;
    public readonly bool F, B, L, R, U, D;

    public Input()
    {
        Quit = false;
        F = B = L = R = U = D = false;
    }
    
    public Input(KeyboardState keyboard)
    {
        Quit = keyboard.IsKeyDown(Keys.Escape);
        F = keyboard.IsKeyDown(Keys.W);
        L = keyboard.IsKeyDown(Keys.A);
        B = keyboard.IsKeyDown(Keys.S);
        R = keyboard.IsKeyDown(Keys.D);
        U = keyboard.IsKeyDown(Keys.Space);
        D = keyboard.IsKeyDown(Keys.LeftControl);
    }
    
    public Input(byte[] data)
    {
        bool[] bools = ByteToBools(data[0]);
        Quit = bools[0];
        F = bools[1];
        B = bools[2];
        L = bools[3];
        R = bools[4];
        U = bools[5];
        D = bools[6];
    }

    public byte[] ToBytes()
    {
        bool[] bools = new [] { Quit, F, B, L, R, U, D };
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