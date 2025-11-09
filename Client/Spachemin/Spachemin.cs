using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpacheNet;
using SpachEngine;

namespace Spachemin;

public class Spachemin : SpachWindow
{
    private static readonly Vector3[] COLORS = new[]
    {
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
    };
    
    private readonly SpacheNetClient net;
    
    protected Player[] players = new Player[4];
    
    private bool paused;
    
    public Spachemin(SpacheNetClient _net)
    {
        net = _net;
        Size = (700, 700);
        Title = "Spachemin";
        this.CursorState = CursorState.Grabbed;
    }

    protected override void OnLoadGraphics()
    {
        string pathApp = Path.GetDirectoryName(Environment.ProcessPath) + "/Assets/";
        string pathShaders = pathApp + "Shaders/";
        string pathMeshes = pathApp + "Meshes/";
        string pathTextures = pathApp + "Textures/";
        
        shader = new Shader(pathShaders + "Default/default.vert", pathShaders + "Default/default.frag");
        
        Mesh meshPlayer = new Mesh(pathMeshes + "cube.mesh", shader);
        Mesh meshGround = new Mesh(pathMeshes + "ground.mesh", shader);
        
        Texture texPlayer = new Texture(pathTextures + "grid.png");
        Texture texGround = new Texture(pathTextures + "grid.png");
        
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new Player(null, meshPlayer, texPlayer);
            players[i].color = COLORS[i];
            gameObjects.Add(players[i]);
        }

        camera = new Camera();
        players[0].Camera = camera;
        
        GameObject ground = new GameObject(meshGround, texGround);
        ground.position = new Vector3(0.0f, -1.0f, 0.0f);
        gameObjects.Add(ground);
        
        lightAmbient = new Vector3(0.25f, 0.25f, 0.25f);
        lightDirectional = new Vector3(0.5f, 0.5f, 0.5f);
        lightDirectionalDirection = new Vector3(0.5f, -1.0f, 0.0f);
        
        base.OnLoadGraphics();
    }
    
    protected override void OnUpdateFrame(double dt)
    {
        if (KeyboardState.IsKeyPressed(Keys.P)) TogglePause();
        Input inputLocal = new Input(KeyboardState, MouseState);
        Input[] inputRemote = Step(inputLocal);
        for (int i = 0; i < inputRemote.Length; i++)
        {
            Input input = inputRemote[i];
            if (input.Quit) Close();
            players[i].OnUpdate(input);
        }
    }
        
    private Input[] Step(Input input)
    {
        byte[][] matrix = net.Step(input.ToBytes());
        Input[] output = new Input[matrix.Length];
        for (int i = 0; i < matrix.Length; i++) output[i] = new Input(matrix[i]);
        return output;
    }
    
    private void TogglePause()
    {
        if (paused) Unpause();
        else Pause();
    }
    
    private void Pause()
    {
        this.CursorState = CursorState.Normal;
        paused = true;
    }

    private void Unpause()
    {
        this.CursorState = CursorState.Grabbed;
        paused = false;
    }
}

