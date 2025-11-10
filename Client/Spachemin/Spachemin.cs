using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpacheNet;
using SpachEngine;
using SpachEngine.Objects;

namespace Spachemin;

public class Spachemin : SpachEngineWindow
{
    private static readonly Vector3[] COLORS = new[]
    {
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
    };
    
    private readonly SpacheNetClient net;
    private SpacheminPlayer[] players;
    private SpacheminPlayer localPlayer;
    private GameObject skybox;
    
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
        
        Shader shader = new Shader(pathShaders + "Default/default.vert", pathShaders + "Default/default.frag");
        Shader shaderSkybox = new Shader(pathShaders + "Default/default.vert", pathShaders + "Skybox/skybox.frag");
        
        Mesh meshPlayer = new Mesh(pathMeshes + "player.obj", shader);
        Mesh meshPlanet = new Mesh(pathMeshes + "sphere.obj", shader);
        Mesh meshSkybox = new Mesh(pathMeshes + "skybox.obj", shaderSkybox);
        
        Texture texPlayer = new Texture(pathTextures + "grid.png"); 
        Texture texPlanet = new Texture(pathTextures + "grid.png");
        Texture texSkybox = new Texture(pathTextures + "stars.png");
        
        skybox = new GameObject(Vector3.Zero, meshSkybox, texSkybox);
        GameObject planet0 = new GameObject(new Vector3(0.0f, 0.0f, 0.0f), meshPlanet, texPlanet);
        GameObject planet1 = new GameObject(new Vector3(-7.0f, 10.0f, 5.0f), meshPlanet, texPlanet);
        GameObject planet2 = new GameObject(new Vector3(20.0f, -3.0f, 12.0f), meshPlanet, texPlanet);
        
        gameObjects.Add(skybox);
        gameObjects.Add(planet0);
        gameObjects.Add(planet1);
        gameObjects.Add(planet2);
        
        players = new SpacheminPlayer[net.playerCount];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new SpacheminPlayer(new Vector3(-2.0f, 0.0f, -2.0f), meshPlayer, texPlayer);
            players[i].color = COLORS[i];
            gameObjects.Add(players[i]);
        }
        
        lightAmbient = new Vector3(0.25f, 0.25f, 0.25f);
        lightDirectional = new Vector3(0.5f, 0.5f, 0.5f);
        lightDirectionalDirection = new Vector3(0.5f, -1.0f, 0.0f);
        
        gravitySources.Add(new GravitySource(planet0.position, 0.001f));
        gravitySources.Add(new GravitySource(planet1.position, 0.001f));
        gravitySources.Add(new GravitySource(planet2.position, 0.001f));
        
        localPlayer = players[net.playerId];
        camera = new Camera();
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
        
        base.OnUpdateFrame(dt);
        
        Vector3 cameraOffset = Vector3.Zero;
        Vector3 front = localPlayer.GetFrontVector();
        Vector3 up = localPlayer.GetUpVector();
        if (thirdPerson) cameraOffset = front.Normalized() * -cameraDistance;
        camera?.Update(localPlayer.position + cameraOffset, front, up);

        skybox.position = localPlayer.position;
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

