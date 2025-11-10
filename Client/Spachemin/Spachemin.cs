using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpacheNet;
using SpachEngine;
using SpachEngine.Objects;

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
    
    private readonly SpacheNetClient net;
    private Player[] players;
    private Player localPlayer;
    
    private bool paused;
    private Input pausedInput;
    private bool thirdPerson = true;
    private float cameraDistance = 1.0f;
    
    public Spachemin(SpacheNetClient _net)
    {
        net = _net;
        Size = (700, 700);
        Title = "Spachemin";
        CursorState = CursorState.Grabbed;
        
        string pathApp = Path.GetDirectoryName(Environment.ProcessPath) + "/Assets/";
        string pathShaders = pathApp + "Shaders/";
        string pathMeshes = pathApp + "Meshes/";
        string pathTextures = pathApp + "Textures/";
        
        shader = new Shader(pathShaders + "Default/default.vert", pathShaders + "Default/default.frag");
        
        Mesh meshPlayer = new Mesh(pathMeshes + "player.obj", shader);
        Mesh meshPlanet = new Mesh(pathMeshes + "sphere.obj", shader);
        
        Texture texPlayer = new Texture(pathTextures + "grid.png"); 
        Texture texPlanet = new Texture(pathTextures + "grid.png");

        players = new Player[net.playerCount];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new Player(meshPlayer, texPlayer);
            players[i].position = new Vector3(-2.0f, 0.0f, -2.0f);
            players[i].color = COLORS[i];
            gameObjects.Add(players[i]);
        }
        localPlayer = players[net.playerId];

        camera = new Camera();
        
        GameObject planet = new GameObject(meshPlanet, texPlanet);
        planet.position = new Vector3(0.0f, 0.0f, 0.0f);
        
        gameObjects.Add(planet);
        
        lightAmbient = new Vector3(0.25f, 0.25f, 0.25f);
        lightDirectional = new Vector3(0.5f, 0.5f, 0.5f);
        lightDirectionalDirection = new Vector3(0.5f, -1.0f, 0.0f);
        
        Load();
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
        if (paused) inputLocal = pausedInput;
        Input[] inputRemote = Step(inputLocal);
        for (int i = 0; i < inputRemote.Length; i++)
        {
            Input input = inputRemote[i];
            if (input.quit) Close();
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
            PhysicsObject physObj = (PhysicsObject)obj;
            Vector3 pos = physObj.position;
            Vector3 toPlanet = Vector3.Zero - pos;
            float gravStrength = 0.001f / (toPlanet.Length * toPlanet.Length);
            Vector3 gravDirection = (toPlanet.Length == 0) ? Vector3.Zero : toPlanet.Normalized();
            Vector3 gravity = gravDirection * gravStrength;
            
            const float planetRadius = 1.0f;
            if (toPlanet.Length > planetRadius) physObj.ApplyForce(gravity);
            else physObj.velocity /= 2.0f;
            
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
        CursorState = CursorState.Normal;
        pausedInput = new Input(null, MouseState);
        paused = true;
    }

    private void Unpause()
    {
        CursorState = CursorState.Grabbed;
        paused = false;
    }
}

