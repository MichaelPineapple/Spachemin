using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpacheNet;
using SpachEngine;

namespace Spachemin;

public class Spachemin : SpachEngineWindow
{
    private readonly SpacheNetClient net;
    
    public Spachemin(SpacheNetClient _net)
    {
        this.net = _net;
        Size = (700, 700);
        Title = "Spachemin";
        UpdateFrequency = 60.0;
        
        players = new Vector3[net.PlayerCount];
        for (int i = 0; i < players.Length; i++) players[i] = Vector3.Zero;
        colors[0] = new Vector3(1.0f, 0.0f, 0.0f);
        colors[1] = new Vector3(0.0f, 0.0f, 1.0f);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();

        Input inputLocal = Utils.CaptureInput(KeyboardState);
        Input[] inputRemote = Utils.Step(inputLocal, net);
        for (int i = 0; i < inputRemote.Length; i++) Utils.ProcessInput(i, inputRemote[i], ref players);
    }
}