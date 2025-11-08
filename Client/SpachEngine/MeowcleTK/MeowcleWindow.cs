using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpachEngine.MeowcleTK;

public class MeowcleWindow : MeowcleWinWindow
{
    private readonly double UpdatePeriod;
    private readonly double RenderPeriod;
    
    private Thread RenderThread;
    private bool StopRender;
    
    protected float MeowcleAspectRatio { get; private set; }
    
    public MeowcleWindow(double updateFps = 60.0f, double renderFps = 60.0f)
    {
        UpdatePeriod = 1.0 / updateFps;
        RenderPeriod = 1.0 / renderFps;
        RenderThread = new Thread(RenderThreadFunction);
    }
    
    public override void Run()
    {
        RenderThread.Start();
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        while (!ShouldClose())
        {
            double elapsed = stopwatch.Elapsed.TotalSeconds;
            if (elapsed > RenderPeriod)
            {
                stopwatch.Restart();
                NewInputFrame();
                GLFW.PollEvents();
                OnUpdateFrame(elapsed);
            }
        }

        StopRender = true;
        RenderThread.Join();
        base.Run();
    }

    private void RenderThreadFunction()
    {
        InitializeGraphicsContext();
        OnLoadGraphics();
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        while (!StopRender)
        {
            double elapsed = stopwatch.Elapsed.TotalSeconds;
            if (elapsed > UpdatePeriod)
            {
                stopwatch.Restart();
                OnRenderFrame(elapsed);
                Context.SwapBuffers();
            }
        }
        
        OnUnloadGraphics();
    }
    
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
        MeowcleAspectRatio = Size.X / (float)Size.Y;
    }
    
    protected virtual void OnLoadGraphics() { }
    protected virtual void OnUnloadGraphics() { }
    protected virtual void OnRenderFrame(double dt) { }
    protected virtual void OnUpdateFrame(double dt) { }
}