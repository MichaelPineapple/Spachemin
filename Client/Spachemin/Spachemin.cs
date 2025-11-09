using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpacheNet;
using SpachEngine;

namespace Spachemin;

public class Spachemin : SpachWindow
{
    private readonly SpacheNetClient Net;
    private bool Paused;
    
    public Spachemin(SpacheNetClient net)
    { 
        Net = net;
        PlayerID = Net.PlayerId;
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
        Mesh meshPlayer = new Mesh(pathMeshes + "cube.mesh", shader);
        Mesh meshGround = new Mesh(pathMeshes + "ground.mesh", shader);
        Texture texPlayer = new Texture(pathTextures + "grid.png");
        Texture texGround = new Texture(pathTextures + "grid.png");
        
        SetDefaultShader(shader);
        SetPlayerMesh(meshPlayer, texPlayer);
        SetGroundMesh(meshGround, texGround);
        
        base.OnLoadGraphics();
    }

    protected override void OnUpdateFrame(double dt)
    {
        if (KeyboardState.IsKeyPressed(Keys.P)) TogglePause();
        Input inputLocal = new Input(KeyboardState, MouseState);
        Input[] inputRemote = Step(inputLocal, Net);
        for (int i = 0; i < inputRemote.Length; i++)
        {
            if (Players[i].OnUpdate(inputRemote[i])) Close();
        }
    }

    private void TogglePause()
    {
        if (Paused) Unpause();
        else Pause();
    }
    
    private void Pause()
    {
        this.CursorState = CursorState.Normal;
        Paused = true;
    }

    private void Unpause()
    {
        this.CursorState = CursorState.Grabbed;
        Paused = false;
    }

    private static Input[] Step(Input input, SpacheNetClient net)
    {
        byte[][] matrix = net.Step(input.ToBytes());
        Input[] output = new Input[matrix.Length];
        for (int i = 0; i < matrix.Length; i++) output[i] = new Input(matrix[i]);
        return output;
    }
}

