using SpacheNet;
using SpachEngine;

namespace Spachemin;

public class Spachemin : SpachWindow
{
    private const float SPEED_PLAYER = 0.02f;
    
    private readonly SpacheNetClient Net;
    
    public Spachemin(SpacheNetClient net)
    {
        Net = net;
        Size = (700, 700);
        Title = "Spachemin";
    }

    protected override void OnLoadGraphics()
    {
        string pathApp = Path.GetDirectoryName(Environment.ProcessPath) + "/Assets/";
        string pathShaders = pathApp + "Shaders/";
        string pathMeshes = pathApp + "Meshes/";
        string pathTextures = pathApp + "Textures/";
        
        Shader shader = new Shader(pathShaders + "Default/default.vert", pathShaders + "Default/default.frag");
        Mesh playerMesh = new Mesh(pathMeshes + "cube.mesh", shader);
        Texture playerTex = new Texture(pathTextures + "awesomeface.png");
        
        SetDefaultShader(shader);
        SetPlayerMesh(playerMesh, playerTex);
        
        base.OnLoadGraphics();
    }

    protected override void OnUpdateFrame(double dt)
    {
        Input inputLocal = new Input(KeyboardState);
        Input[] inputRemote = Step(inputLocal, Net);
        for (int i = 0; i < inputRemote.Length; i++) ProcessInput(i, inputRemote[i]);
        
        PlayerCamera.Position.X = players[Net.PlayerId].Position.X;
        PlayerCamera.Position.Y = players[Net.PlayerId].Position.Y;
    }
    
    private void ProcessInput(int id, Input input)
    {
        if (input.Quit) Close();
        if (input.F) players[id].Position.Y += SPEED_PLAYER;
        if (input.B) players[id].Position.Y -= SPEED_PLAYER;
        if (input.L) players[id].Position.X -= SPEED_PLAYER;
        if (input.R) players[id].Position.X += SPEED_PLAYER;
    }
        
    private static Input[] Step(Input input, SpacheNetClient net)
    {
        byte[][] matrix = net.Step(input.ToBytes());
        Input[] output = new Input[matrix.Length];
        for (int i = 0; i < matrix.Length; i++) output[i] = new Input(matrix[i]);
        return output;
    }
}

