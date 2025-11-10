using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Spachemin;

public struct Input
{
    public bool quit;
    public bool F, B, L, R, U, D;
    public float mouseX, mouseY;
    
    public Input()
    {
        quit = false;
        F = B = L = R = U = D = false;
        mouseX = mouseY = 0.0f;
    }
    
    public Input(KeyboardState? keyboard, MouseState? mouse)
    {
        if (keyboard != null)
        {
            quit = keyboard.IsKeyDown(Keys.Escape);
            F = keyboard.IsKeyDown(Keys.W);
            L = keyboard.IsKeyDown(Keys.A);
            B = keyboard.IsKeyDown(Keys.S);
            R = keyboard.IsKeyDown(Keys.D);
            U = keyboard.IsKeyDown(Keys.Space);
            D = keyboard.IsKeyDown(Keys.LeftControl);
        }
        if (mouse != null)
        {
            mouseX = mouse.X;
            mouseY = mouse.Y;
        }
    }
    
    public Input(byte[] data)
    {
        bool[] bools = ByteToBools(data[0]);
        
        quit = bools[0];
        F = bools[1];
        B = bools[2];
        L = bools[3];
        R = bools[4];
        U = bools[5];
        D = bools[6];
        
        byte[] xb = new byte[4];
        byte[] yb = new byte[4];
        
        int j = 1;
        for (int i = 0; i < 4; i++)
        {
            xb[i] = data[j];
            yb[i] = data[j + 4];
            j++;
        }
        
        mouseX = BitConverter.ToSingle(xb);
        mouseY = BitConverter.ToSingle(yb);
    }

    public byte[] ToBytes()
    {
        byte b = ByteFromBools(new [] { quit, F, B, L, R, U, D });
        
        byte[] xb = BitConverter.GetBytes(mouseX);
        byte[] yb = BitConverter.GetBytes(mouseY);
        
        byte[] output = new byte[9];
        output[0] = b;
        
        int j = 1;
        for (int i = 0; i < 4; i++)
        {
            output[j] = xb[i];
            output[j + 4] = yb[i];
            j++;
        }
        
        return output;
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