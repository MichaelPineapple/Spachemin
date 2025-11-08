using System.Runtime.InteropServices;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpachEngine.MeowcleTK;

public unsafe class MeowcleWinWindow : NativeWindow
{
    #region Win32 Function for timing
    
    [DllImport("kernel32", SetLastError = true)] 
    private static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);
    
    [DllImport("kernel32")]
    private static extern IntPtr GetCurrentThread();
    
    [DllImport("winmm")]
    private static extern uint timeBeginPeriod(uint uPeriod);
    
    [DllImport("winmm")]
    private static extern uint timeEndPeriod(uint uPeriod);
    
    #endregion
    
    private const int TimePeriod = 8;
    private bool disposed;
    
    public MeowcleWinWindow() : base(NativeWindowSettings.Default) { }

    public void InitializeGraphicsContext()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SetThreadAffinityMask(GetCurrentThread(), new IntPtr(1));
            timeBeginPeriod(TimePeriod);
        }
        Context?.MakeCurrent();
    }

    protected bool ShouldClose()
    {
        return GLFW.WindowShouldClose(WindowPtr);
    }
    
    public virtual void Run()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) timeEndPeriod(TimePeriod);
    }
    
    public override void Dispose()
    {
        base.Dispose();
        if (disposed) return;
        GC.SuppressFinalize(this);
        disposed = true;
    }
}