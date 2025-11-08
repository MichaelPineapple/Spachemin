using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace SpachEngine;

public class SpachEngineWindow : GameWindow
{
    private int shaderDefault;
    private int playerVAO;
    
    private int ulColor;
    private int ulModel;
    private int ulProjj;
    
    private float aspectRatio;

    protected Vector3[] players;
    protected readonly Vector3[] colors = new Vector3[10];
    
    public SpachEngineWindow() : base(GameWindowSettings.Default, NativeWindowSettings.Default) { }
    
    protected override void OnLoad()
    {
        base.OnLoad();
        
        shaderDefault = SpachEngineUtils.CreateShader();
        GL.UseProgram(shaderDefault);

        float[] playerMesh = SpachEngineUtils.CreateSquareMesh(0.1f);
        playerVAO = SpachEngineUtils.CreateVAO(playerMesh, shaderDefault);
        GL.BindVertexArray(playerVAO);
        
        ulColor = GL.GetUniformLocation(shaderDefault, "color");
        ulModel = GL.GetUniformLocation(shaderDefault, "model");
        ulProjj = GL.GetUniformLocation(shaderDefault, "projj");
    }
    
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        Matrix4 proj = Matrix4.CreateOrthographicOffCenter(-aspectRatio, aspectRatio, -1.0f, 1.0f, 1.0f, -1.0f);
        GL.UniformMatrix4(ulProjj, true, ref proj);
        
        for (int i = 0; i < players.Length; i++) RenderPlayer(i);
        SwapBuffers();
    }

    private void RenderPlayer(int i)
    {
        Vector3 color = colors[i];
        Vector3 pos = players[i];
        Matrix4 model = Matrix4.CreateTranslation(pos);
        GL.UniformMatrix4(ulModel, true, ref model);
        GL.Uniform3(ulColor, color);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
        
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        aspectRatio = Size.X / (float)Size.Y;
    }
    
    protected override void OnUnload()
    {
        GL.DeleteProgram(shaderDefault);
    }
}