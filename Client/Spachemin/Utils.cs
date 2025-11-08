using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpacheNet;

namespace Spachemin;

public class Utils
{
    public static byte[] EncodeInput(Input _data)
    {
        bool[] bools = new []
        {
            _data.W,
            _data.A,
            _data.S,
            _data.D,
        };
        return new[] { ByteFromBools(bools) };
    }

    public static Input DecodeInput(byte[] _data)
    {
        bool[] bools = ByteToBools(_data[0]);
        Input output = new Input();
        output.W = bools[0];
        output.A = bools[1];
        output.S = bools[2];
        output.D = bools[3];
        return output;
    }
    
    public static byte ByteFromBools(bool[] bools)
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
    
    public static bool[] ByteToBools(byte bytes)
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
    
    public static Input CaptureInput(KeyboardState keyboard)
    {
        Input inputLocal = new Input();
        inputLocal.W = keyboard.IsKeyDown(Keys.W);
        inputLocal.A = keyboard.IsKeyDown(Keys.A);
        inputLocal.S = keyboard.IsKeyDown(Keys.S);
        inputLocal.D = keyboard.IsKeyDown(Keys.D);
        return inputLocal;
    }
    
    public static void ProcessInput(int id, Input input, ref Vector3[] players)
    {
        Vector3 position = players[id];
        if (input.W) position.Y += Globals.SPEED_PLAYER;
        if (input.A) position.X -= Globals.SPEED_PLAYER;
        if (input.S) position.Y -= Globals.SPEED_PLAYER;
        if (input.D) position.X += Globals.SPEED_PLAYER;
        players[id] = position;
    }
        
    public static Input[] Step(Input input, SpacheNetClient net)
    {
        byte[][] matrix = net.Step(Utils.EncodeInput(input));
        Input[] output = new Input[matrix.Length];
        for (int i = 0; i < matrix.Length; i++) output[i] = Utils.DecodeInput(matrix[i]);
        return output;
    }
}

public struct Input
{
    public bool W, A, S, D;
    public Input() { W = A = S = D = false; }
}