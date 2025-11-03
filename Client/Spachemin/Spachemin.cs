using System.Net.Sockets;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Spachemin;

public class Spachemin : GraphicsEngine
{
    private const float SPEED_PLAYER = 0.02f;
    
    private readonly int playerCount;
    private readonly Stream stream;

    
    private Spachemin(Stream stream, int _playerCount)
    {
        this.playerCount = _playerCount;
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
        
        Input[] inputRemote = Step(inputLocal);
        for (int i = 0; i < inputRemote.Length; i++) ProcessInput(i, inputRemote[i]);
    }
    
    private Input[] Step(Input input)
    {
        const int bufferLen = 3;
        byte[] myData = new byte[bufferLen];
        byte x = Utils.EncodeInput(input)[0];
        myData[0] = x;
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
        client.Connect(ip, 9001);
        Stream stream = client.GetStream();
        
        byte[] loginData = new byte[2];
        _ = stream.Read(loginData, 0, 2);
        int playerCount = loginData[0];
        stream.Write(new byte[] {69}, 0, 1);
        
        Console.WriteLine("Connected");
        Spachemin x = new Spachemin(stream, playerCount);
        x.Run();
        
        client.Close();
    }
}