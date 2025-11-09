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
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
    };

    private Vector3 gravity = new Vector3(0.0f, -0.0001f, 0.0f);
    private float groundLevel = -1.0f;
    
    private readonly SpacheNetClient net;
    private Player[] players;
    private Player localPlayer;
    
    private bool paused;
    private bool thirdPerson = true;
    private float cameraDistance = 1.0f;
    
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
        
        Mesh meshPlayer = new Mesh(pathMeshes + "player.obj", shader);
        Mesh meshGround = new Mesh(pathMeshes + "ground.obj", shader);
        
        Texture texPlayer = new Texture(pathTextures + "grid.png"); 
        Texture texGround = new Texture(pathTextures + "grid.png");

        players = new Player[net.PlayerCount];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new Player(meshPlayer, texPlayer);
            players[i].color = COLORS[i];
            gameObjects.Add(players[i]);
        }
        localPlayer = players[net.PlayerId];

        camera = new Camera();
        
        GameObject ground = new GameObject(meshGround, texGround);
        ground.position = new Vector3(0.0f, groundLevel, 0.0f);
        gameObjects.Add(ground);
        
        lightAmbient = new Vector3(0.25f, 0.25f, 0.25f);
        lightDirectional = new Vector3(0.5f, 0.5f, 0.5f);
        lightDirectionalDirection = new Vector3(0.5f, -1.0f, 0.0f);
        
        base.OnLoadGraphics();
    }
    
    protected override void OnUpdateFrame(double dt)
    {
        if (KeyboardState.IsKeyPressed(Keys.P)) TogglePause();
        if (KeyboardState.IsKeyPressed(Keys.C)) ToggleThirdPerson();

        const float cameraDistanceIncrement = 0.1f;
        if (KeyboardState.IsKeyPressed(Keys.Equal)) cameraDistance -= cameraDistanceIncrement;
        if (KeyboardState.IsKeyPressed(Keys.Minus)) cameraDistance += cameraDistanceIncrement;

        const float fovIncrement = 10.0f;
        if (KeyboardState.IsKeyPressed(Keys.LeftBracket)) camera?.AddFov(-fovIncrement);
        if (KeyboardState.IsKeyPressed(Keys.RightBracket)) camera?.AddFov(fovIncrement);
        
        Input inputLocal = new Input(KeyboardState, MouseState);
        Input[] inputRemote = Step(inputLocal);
        for (int i = 0; i < inputRemote.Length; i++)
        {
            Input input = inputRemote[i];
            if (input.Quit) Close();
            players[i].ProcessInput(input);
        }
        
        for (int i = 0; i < gameObjects.Count; i++) UpdateObject(gameObjects[i]);
        
        Vector3 cameraOffset = Vector3.Zero;
        Vector3 front = localPlayer.GetFrontVector();
        Vector3 up = localPlayer.GetUpVector();
        if (thirdPerson) cameraOffset = front.Normalized() * -cameraDistance;
        camera?.Update(localPlayer.position + cameraOffset, front, up);
    }

    private void UpdateObject(GameObject obj)
    {
        if (obj is PhysicsObject)
        {
            const float feetOffset = 0.1f;
            PhysicsObject physObj = (PhysicsObject)obj;
            float feet = physObj.position.Y - feetOffset;
            Vector3 vel = physObj.velocity;
            if (feet > groundLevel) physObj.ApplyForce(gravity);
            else if (feet < groundLevel)
            {
                if (vel.Y < 0) physObj.velocity.Y = 0;
                physObj.position.Y = groundLevel + feetOffset;
            }
            else
            {
                const float cof = 0.25f;
                Vector3 frictionForce = new Vector3(-vel.X * cof, 0.0f, -vel.Z * cof);
                physObj.ApplyForce(frictionForce);
            }
            physObj.Update();
        }
    }
        
    private Input[] Step(Input input)
    {
        byte[][] matrix = net.Step(input.ToBytes());
        Input[] output = new Input[matrix.Length];
        for (int i = 0; i < matrix.Length; i++) output[i] = new Input(matrix[i]);
        return output;
    }
    
    public void GoThirdPerson()
    {
        thirdPerson = true;
    }

    public void GoFirstPerson()
    {
        thirdPerson = false;
    }

    public void ToggleThirdPerson()
    {
        if (thirdPerson) GoFirstPerson();
        else GoThirdPerson();
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

