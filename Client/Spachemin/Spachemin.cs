using OpenTK.Mathematics;
using SpacheNet;
using SpachEngine;

namespace Spachemin;

public class Spachemin : SpachEngineWindow
{
    private const float SPEED_PLAYER = 0.02f;

    private readonly SpacheNetClient? Net;
    
    public Spachemin(SpacheNetClient? net)
    {
        Net = net;
        Size = (700, 700);
        Title = "Spachemin";
        
        players = new Vector3[4];
        for (int i = 0; i < players.Length; i++) players[i] = Vector3.Zero;

        colors = new Vector3[4];
        colors[0] = new Vector3(1.0f, 1.0f, 1.0f);
        colors[1] = new Vector3(1.0f, 0.0f, 0.0f);
        colors[2] = new Vector3(0.0f, 0.0f, 1.0f);
        colors[3] = new Vector3(0.0f, 0.0f, 1.0f);
    }

    protected override void OnLoadGraphics()
    {
        string pathApp = Path.GetDirectoryName(Environment.ProcessPath) + "/Assets/";
        string pathShaders = pathApp + "Shaders/";
        string pathMeshes = pathApp + "Meshes/";
        
        Shader shader = new Shader(pathShaders + "Default/default.vert", pathShaders + "Default/default.frag");
        Mesh playerMesh = new Mesh(pathMeshes + "square.mesh", shader);
        
        SetDefaultShader(shader);
        SetPlayerMesh(playerMesh);
        
        base.OnLoadGraphics();
    }

    protected override void OnUpdateFrame(double dt)
    {
        Input inputLocal = new Input(KeyboardState);
        Input[] inputRemote = Step(inputLocal, Net);
        for (int i = 0; i < inputRemote.Length; i++) ProcessInput(i, inputRemote[i]);
    }
    
    private void ProcessInput(int id, Input input)
    {
        Vector3 position = players[id];
        if (input.Quit) Close();
        if (input.W) position.Y += SPEED_PLAYER;
        if (input.A) position.X -= SPEED_PLAYER;
        if (input.S) position.Y -= SPEED_PLAYER;
        if (input.D) position.X += SPEED_PLAYER;
        players[id] = position;
    }
        
    private static Input[] Step(Input input, SpacheNetClient? net)
    {
        if (net == null) return new [] { new Input(input.ToBytes()) };
        byte[][] matrix = net.Step(input.ToBytes());
        Input[] output = new Input[matrix.Length];
        for (int i = 0; i < matrix.Length; i++) output[i] = new Input(matrix[i]);
        return output;
    }
}

