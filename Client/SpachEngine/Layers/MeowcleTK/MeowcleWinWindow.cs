using System.Runtime.InteropServices;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpachEngine.Layers.MeowcleTK;

public unsafe class MeowcleWinWindow : NativeWindow
{
    #region Win32 Function for timing
    
    [DllImport("kernel32", SetLastError = true)] 
    private static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);
    
    [DllImport("kernel32")]
    private static extern IntPtr GetCurrentThread();
    
    [DllImport("winmm")]
    private static extern uint TimeBeginPeriod(uint uPeriod);
    
    [DllImport("winmm")]
    private static extern uint TimeEndPeriod(uint uPeriod);
    
    #endregion
    
    private const int TIME_PERIOD = 8;
    private bool disposed;

    public MeowcleWinWindow() : base(NativeWindowSettings.Default)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SetThreadAffinityMask(GetCurrentThread(), new IntPtr(1));
            TimeBeginPeriod(TIME_PERIOD);
        }
        Context?.MakeCurrent();
    }

    protected bool ShouldClose()
    {
        return GLFW.WindowShouldClose(WindowPtr);
    }
    
    public virtual void Run()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) TimeEndPeriod(TIME_PERIOD);
    }
    
    public override void Dispose()
    {
        base.Dispose();
        if (disposed) return;
        GC.SuppressFinalize(this);
        disposed = true;
    }
}