using System.Net.Sockets;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Spachemin;

public class Spachemin : GraphicsEngine
{
    private const float SPEED_PLAYER = 0.02f;
    
    private readonly int playerCount;
    private readonly int frameDelay;
    private readonly Stream stream;

    private int frame = 0;
    
    private Spachemin(Stream stream, int _playerCount, int _frameDelay)
    {
        this.playerCount = _playerCount;
        this.frameDelay = _frameDelay;
        this.stream = stream;
        
        Size = (700, 700);
        Title = "Spachemin";
        UpdateFrequency = 60.0;
        players = new Vector3[playerCount];
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
        
        Input[] inputRemote = Step(frame + frameDelay, inputLocal);
        for (int i = 0; i < inputRemote.Length; i++) ProcessInput(i, inputRemote[i]);
        frame++;
    }
    
    private Input[] Step(int _frame, Input input)
    {
        const int bufferLen = 3;
        byte[] myData = new byte[bufferLen + 4];
        byte x = Utils.EncodeInput(input)[0];
        byte[] frameBytes = Utils.BytesFromInt(_frame);
        myData[0] = frameBytes[0];
        myData[1] = frameBytes[1];
        myData[2] = frameBytes[2];
        myData[3] = frameBytes[3];
        myData[4] = x;
        stream.Write(myData, 0, myData.Length);
        Input[] output = new Input[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            byte[] buffer = new byte[bufferLen];
            _ = stream.Read(buffer, 0, buffer.Length);
            output[i] = Utils.DecodeInput(buffer);
        }
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
        string? ip = Console.ReadLine(); // 44.245.211.131
        if (ip == null) ip = "";
        if (ip.Length == 0) ip = "127.0.0.1";
        Console.WriteLine("Connecting to `" + ip + "`...");
        
        TcpClient client = new TcpClient();
        client.NoDelay = true;
        client.Connect(ip, 9001);
        Stream stream = client.GetStream();
        
        byte[] loginData = new byte[3];
        _ = stream.Read(loginData, 0, 3);
        int playerCount = loginData[0];
        int frameDelay = loginData[2];
        stream.Write(new byte[] {69}, 0, 1);
        
        Console.WriteLine("Connected");
        Spachemin x = new Spachemin(stream, playerCount, frameDelay);
        x.Run();
        
        client.Close();
    }
}