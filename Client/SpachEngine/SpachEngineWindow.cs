using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Spachemin;
using SpachEngine.MeowcleTK;

namespace SpachEngine;

public class SpachEngineWindow : MeowcleWindow
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
    private int ulColor;
    private int ulModel;
    private int ulProjj;
    
    public SpachEngineWindow()
    {
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
        GL.CullFace(TriangleFace.Back);
        this.CursorState = CursorState.Normal;
        
        GL.UseProgram(shaderDefault);
        GL.BindVertexArray(vaoPlayer);
        
        ulColor = GL.GetUniformLocation(shaderDefault, "color");
        ulModel = GL.GetUniformLocation(shaderDefault, "model");
        ulProjj = GL.GetUniformLocation(shaderDefault, "projj");
    }

    protected override void OnRenderFrame(double dt)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Matrix4 proj = Matrix4.CreateOrthographicOffCenter(-MeowcleAspectRatio, MeowcleAspectRatio, -1.0f, 1.0f, 1.0f, -1.0f);
        GL.UniformMatrix4(ulProjj, true, ref proj);
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
        GL.UniformMatrix4(ulModel, true, ref model);
        GL.Uniform3(ulColor, players[i].Color);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
    
    protected override void OnUnloadGraphics()
    {
        GL.DeleteProgram(shaderDefault);
    }
}