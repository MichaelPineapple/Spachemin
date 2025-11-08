using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpacheNet;

namespace Spachemin;

public class Spachemin : GraphicsEngine
{
    private const float SPEED_PLAYER = 0.02f;
    
    private readonly SpacheNetClient net;
    
    private Spachemin(SpacheNetClient _net)
    {
        this.net = _net;
        Size = (700, 700);
        Title = "Spachemin";
        UpdateFrequency = 60.0;
        
        players = new Vector3[net.PlayerCount];
        for (int i = 0; i < players.Length; i++) players[i] = Vector3.Zero;
        colors[0] = new Vector3(1.0f, 0.0f, 0.0f);
        colors[1] = new Vector3(0.0f, 0.0f, 1.0f);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
        
        Input inputLocal = new Input();
        inputLocal.W = KeyboardState.IsKeyDown(Keys.W);
        inputLocal.A = KeyboardState.IsKeyDown(Keys.A);
        inputLocal.S = KeyboardState.IsKeyDown(Keys.S);
        inputLocal.D = KeyboardState.IsKeyDown(Keys.D);
        
        Input[] inputRemote = Step(inputLocal);
        for (int i = 0; i < inputRemote.Length; i++) ProcessInput(i, inputRemote[i]);
    }
    
    private Input[] Step(Input input)
    {
        byte[][] matrix = net.Step(Utils.EncodeInput(input));
        Input[] output = new Input[matrix.Length];
        for (int i = 0; i < matrix.Length; i++) output[i] = Utils.DecodeInput(matrix[i]);
        return output;
    }
    
    private void ProcessInput(int id, Input input)
    {
        Vector3 position = players[id];
        if (input.W) position.Y += SPEED_PLAYER;
        if (input.A) position.X -= SPEED_PLAYER;
        if (input.S) position.Y -= SPEED_PLAYER;
        if (input.D) position.X += SPEED_PLAYER;
        players[id] = position;
    }

    public static void Main()
    {
        Console.WriteLine("Hello, Spachemin!");
        SpacheNetClient net = new SpacheNetClient();
        net.Connect(Console.ReadLine());
        Console.WriteLine("Connected");
        Spachemin x = new Spachemin(net);
        x.Run();
        net.Disconnect();        
    }
}