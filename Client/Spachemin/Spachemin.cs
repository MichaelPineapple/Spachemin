using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpacheNet;
using SpachEngine;

namespace Spachemin;

public class Spachemin : SpachEngineWindow
{
    private readonly SpacheNetClient Net;
    
    public Spachemin(SpacheNetClient net)
    {
        Net = net;
        Size = (700, 700);
        Title = "Spachemin";
        players = new Vector3[Net.PlayerCount];
        for (int i = 0; i < players.Length; i++) players[i] = Vector3.Zero;
        colors[0] = new Vector3(1.0f, 0.0f, 0.0f);
        colors[1] = new Vector3(0.0f, 0.0f, 1.0f);
    }

    protected override void OnLoadGraphics()
    {
        string pathApp = Path.GetDirectoryName(Environment.ProcessPath) + "/Assets";
        string pathShaders = pathApp + "/Shaders";
        Shader shader = new Shader(pathShaders + "/Default/default.vert", pathShaders + "/Default/default.frag");
        SetDefaultShader(shader);
        
        base.OnLoadGraphics();
    }

    protected override void OnUpdateFrame(double dt)
    {
        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
        Input inputLocal = Utils.CaptureInput(KeyboardState);
        Input[] inputRemote = Utils.Step(inputLocal, Net);
        for (int i = 0; i < inputRemote.Length; i++) Utils.ProcessInput(i, inputRemote[i], ref players);
    }
}