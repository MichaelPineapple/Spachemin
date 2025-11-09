using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
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
    
    protected Player[] Players = new Player[4];
    private Mesh MeshPlayer;

    private Vector3 LightAmbient = new Vector3(0.5f, 0.5f, 0.5f);
    
    private int shaderDefault;
    
    private int texPlayer;
    
    private int ulModel;
    private int ulVieww;
    private int ulProjj;
    private int ulColor;
    private int ulLightAmb;

    protected int PlayerID;
    
    public SpachWindow()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i] = new Player();
            Players[i].Color = COLORS[i];
        }
    }
    
    protected override void OnLoadGraphics()
    {
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace); 
        GL.CullFace(TriangleFace.Back);
        this.CursorState = CursorState.Grabbed;
        
        GL.UseProgram(shaderDefault);
        
        ulColor = GL.GetUniformLocation(shaderDefault, "color");
        ulModel = GL.GetUniformLocation(shaderDefault, "model");
        ulVieww = GL.GetUniformLocation(shaderDefault, "vieww");
        ulProjj = GL.GetUniformLocation(shaderDefault, "projj");
        
        ulLightAmb = GL.GetUniformLocation(shaderDefault, "lightAmb");
        GL.Uniform3(ulLightAmb, LightAmbient);
    }

    protected override void OnRenderFrame(double dt)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Player me = Players[PlayerID];
        Matrix4 vieww = me.GetViewMatrix();
        Matrix4 projj = me.GetProjectionMatrix(MeowcleAspectRatio);
        GL.UniformMatrix4(ulVieww, true, ref vieww);
        GL.UniformMatrix4(ulProjj, true, ref projj);
        
        for (int i = 0; i < Players.Length; i++) RenderPlayer(Players[i]);
    }

    protected void SetDefaultShader(Shader shader)
    {
        shaderDefault = shader.Handle;
    }

    protected void SetPlayerMesh(Mesh mesh, Texture tex)
    {
        MeshPlayer = mesh;
        texPlayer = tex.Handle;
    }

    private void RenderPlayer(Player p)
    {
        Matrix4 model = Matrix4.CreateTranslation(p.Position);
        GL.UniformMatrix4(ulModel, true, ref model);
        GL.Uniform3(ulColor, p.Color);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, texPlayer);
        GL.BindVertexArray(MeshPlayer.VAO);
        GL.DrawArrays(PrimitiveType.Triangles, 0, MeshPlayer.VertexLength);
    }
    
    protected override void OnUnloadGraphics()
    {
        GL.DeleteProgram(shaderDefault);
    }
}