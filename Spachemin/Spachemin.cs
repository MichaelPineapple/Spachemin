using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using MclTK;
using MclNet;
using SoupEngine;

namespace Spachemin;

public class Spachemin : SoupWindow
{
    private static readonly Vector3[] COLORS = new[]
    {
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
    };
    
    private Player[] Players;
    private Player LocalPlayer;
    private MclObject Skybox;
    
    private bool Paused;
    private Input PausedInput;
    private bool ThirdPerson = true;
    private float CameraDistance = 1.0f;
    
    public Spachemin(MclNetClient net) : base(net)
    {
        Title = "Spachemin Alpha";
        
        string pathApp = Path.GetDirectoryName(Environment.ProcessPath) + "/Assets/";
        string pathShaders = pathApp + "Shaders/";
        string pathMeshes = pathApp + "Meshes/";
        string pathTextures = pathApp + "Textures/";

        string pathDefaultVertShader = pathShaders + "default.vert";
        MclShader shaderDefault = new MclShader(pathDefaultVertShader, pathShaders + "default.frag");
        MclShader shaderSkybox = new MclShader(pathDefaultVertShader, pathShaders + "skybox.frag");
        
        MclMesh meshPlayer = new MclMesh(pathMeshes + "player.obj", shaderDefault);
        MclMesh meshPlanet = new MclMesh(pathMeshes + "Planets/planet10.obj", shaderDefault);
        MclMesh meshSkybox = new MclMesh(pathMeshes + "skybox.obj", shaderSkybox);
        
        MclTexture texGrid = new MclTexture(pathTextures + "grid.png"); 
        MclTexture texSkybox = new MclTexture(pathTextures + "skybox.png");
        
        Skybox = new MclObject(Vector3.Zero, meshSkybox, texSkybox); 
        MclObject planet0 = new MclObject(new Vector3(0.0f, 0.0f, 0.0f), meshPlanet, texGrid);
        MclObject planet1 = new MclObject(new Vector3(-70.0f, 100.0f, 98.0f), meshPlanet, texGrid);
        MclObject planet2 = new MclObject(new Vector3(200.0f, -30.0f, 120.0f), meshPlanet, texGrid);
        
        MclObjects.Add(Skybox);
        MclObjects.Add(planet0);
        MclObjects.Add(planet1);
        MclObjects.Add(planet2);
        
        Players = new Player[Net.PlayerCount];
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i] = new Player(new Vector3(-15.0f, 0.0f, -15.0f), meshPlayer, texGrid);
            Players[i].Color = COLORS[i];
            MclObjects.Add(Players[i]);
        }
        
        LightAmbient = new Vector3(0.25f, 0.25f, 0.25f);
        LightDirectional = new Vector3(0.5f, 0.5f, 0.5f);
        LightDirectionalDirection = new Vector3(0.5f, -1.0f, 0.0f);
        
        GravitySources.Add(new GravitySource(planet0.Position, 0.01f, 10.0f));
        GravitySources.Add(new GravitySource(planet1.Position, 0.01f, 10.0f));
        GravitySources.Add(new GravitySource(planet2.Position, 0.01f, 10.0f));
        
        LocalPlayer = Players[Net.PlayerId];
        Camera = new MclCamera();
    }
    
    protected override void OnUpdateFrame(double dt)
    {
        if (KeyboardState.IsKeyPressed(Keys.P)) TogglePause();
        if (KeyboardState.IsKeyPressed(Keys.C)) ToggleThirdPerson();

        const float cameraDistanceIncrement = 0.1f;
        if (KeyboardState.IsKeyPressed(Keys.Equal)) CameraDistance -= cameraDistanceIncrement;
        if (KeyboardState.IsKeyPressed(Keys.Minus)) CameraDistance += cameraDistanceIncrement;

        const float fovIncrement = 10.0f;
        if (KeyboardState.IsKeyPressed(Keys.LeftBracket)) Camera?.AddFov(-fovIncrement);
        if (KeyboardState.IsKeyPressed(Keys.RightBracket)) Camera?.AddFov(fovIncrement);
        
        Input inputLocal = new Input(KeyboardState, MouseState);
        if (Paused) inputLocal = PausedInput;
        Input[] inputRemote = Step(inputLocal);
        for (int i = 0; i < inputRemote.Length; i++)
        {
            Input input = inputRemote[i];
            if (input.Quit) Close();
            Players[i].ProcessInput(input);
        }
        
        base.OnUpdateFrame(dt);
        
        Vector3 cameraOffset = Vector3.Zero;
        Vector3 front = LocalPlayer.GetFrontVector();
        Vector3 up = LocalPlayer.GetUpVector();
        if (ThirdPerson) cameraOffset = front.Normalized() * -CameraDistance;
        Camera?.Update(LocalPlayer.Position + cameraOffset, front, up);

        Skybox.Position = LocalPlayer.Position;
    }
        
    private Input[] Step(Input input)
    {
        byte[][] matrix = Net.Step(input.ToBytes());
        Input[] output = new Input[matrix.Length];
        for (int i = 0; i < matrix.Length; i++) output[i] = new Input(matrix[i]);
        return output;
    }
    
    public void GoThirdPerson()
    {
        ThirdPerson = true;
    }

    public void GoFirstPerson()
    {
        ThirdPerson = false;
    }

    public void ToggleThirdPerson()
    {
        if (ThirdPerson) GoFirstPerson();
        else GoThirdPerson();
    }
    
    private void TogglePause()
    {
        if (Paused) Unpause();
        else Pause();
    }
    
    private void Pause()
    {
        CursorState = CursorState.Normal;
        PausedInput = new Input(null, MouseState);
        Paused = true;
    }

    private void Unpause()
    {
        CursorState = CursorState.Grabbed;
        Paused = false;
    }
    
    public static void Main(string[] args)
    {
        Console.WriteLine("*** Spachemin ***");
        string? ip = null;
        if (!(args.Length > 0 && args[0] == "local")) ip = Console.ReadLine();
        if (ip?.Trim().ToLower() == "l") ip = null;
        MclNetClient net = new MclNetClient();
        net.Connect(ip);
        Console.WriteLine("Connected");
        Spachemin spachemin = new Spachemin(net);
        spachemin.Run();
        net.Disconnect();  
    }
}