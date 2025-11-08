using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpachEngine.MeowcleTK;

public class MeowcleWindow : MeowcleWinWindow
{
    private readonly double updatePeriod;
    protected float MeowcleAspectRatio { get; private set; }
    
    public MeowcleWindow(double fps = 60.0f)
    {
        updatePeriod = 1.0 / fps;
    }
    
    public override void Run()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        while (!ShouldClose())
        {
            double elapsed = stopwatch.Elapsed.TotalSeconds;
            if (elapsed > updatePeriod)
            {
                stopwatch.Restart();
                NewInputFrame();
                GLFW.PollEvents();
                OnUpdateFrame(elapsed);
                OnRenderFrame(elapsed);
                Context.SwapBuffers();
            }
        }

        OnUnload();
        base.Run();
    }
    
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
        MeowcleAspectRatio = Size.X / (float)Size.Y;
    }
    
    protected virtual void OnUnload() { }
    protected virtual void OnUpdateFrame(double dt) { }
    protected virtual void OnRenderFrame(double dt) { }
}