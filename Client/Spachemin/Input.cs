using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Spachemin;

public struct Input
{
    public readonly bool W, A, S, D;

    public Input()
    {
        W = A = S = D = false;
    }
    
    public Input(KeyboardState keyboard)
    {
        W = keyboard.IsKeyDown(Keys.W);
        A = keyboard.IsKeyDown(Keys.A);
        S = keyboard.IsKeyDown(Keys.S);
        D = keyboard.IsKeyDown(Keys.D);
    }
    
    public Input(byte[] data)
    {
        bool[] bools = ByteToBools(data[0]);
        W = bools[0];
        A = bools[1];
        S = bools[2];
        D = bools[3];
    }

    public byte[] ToBytes()
    {
        bool[] bools = new [] { W, A, S, D };
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