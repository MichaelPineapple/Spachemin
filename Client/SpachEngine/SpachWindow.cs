using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Spachemin;
using SpachEngine.MeowcleTK;

namespace SpachEngine;

public class SpachWindow : MeowcleWindow
{
    private static readonly Vector3[] COLORS = new[]
    {
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
    };
    
    protected Player[] players = new Player[4];
    
    private int shaderDefault;
    private int vaoPlayer;
    private int ulModel;
    private int ulVieww;
    private int ulProjj;
    private int ulColor;

    protected Camera PlayerCamera;
    
    public SpachWindow()
    {
        PlayerCamera = new Camera(Vector3.Zero);
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new Player();
            players[i].Color = COLORS[i];
        }
    }
    
    protected override void OnLoadGraphics()
    {
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace); 
        this.CursorState = CursorState.Normal;
        
        GL.UseProgram(shaderDefault);
        GL.BindVertexArray(vaoPlayer);
        
        ulColor = GL.GetUniformLocation(shaderDefault, "color");
        ulModel = GL.GetUniformLocation(shaderDefault, "model");
        ulVieww = GL.GetUniformLocation(shaderDefault, "vieww");
        ulProjj = GL.GetUniformLocation(shaderDefault, "projj");
    }

    protected override void OnRenderFrame(double dt)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Matrix4 projj = PlayerCamera.GetProjectionMatrix(MeowcleAspectRatio);
        GL.UniformMatrix4(ulProjj, true, ref projj);
        for (int i = 0; i < players.Length; i++) RenderPlayer(i);
    }

    protected void SetDefaultShader(Shader shader)
    {
        shaderDefault = shader.Handle;
    }

    protected void SetPlayerMesh(Mesh mesh)
    {
        vaoPlayer = mesh.VAO;
    }

    private void RenderPlayer(int i)
    {
        Matrix4 model = Matrix4.CreateTranslation(players[i].Position);
        Matrix4 vieww = Matrix4.Identity;
        GL.UniformMatrix4(ulModel, true, ref model);
        GL.UniformMatrix4(ulVieww, true, ref vieww);
        GL.Uniform3(ulColor, players[i].Color);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
    
    protected override void OnUnloadGraphics()
    {
        GL.DeleteProgram(shaderDefault);
    }
}