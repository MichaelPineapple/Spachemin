using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpachEngine.MeowcleTK;

namespace SpachEngine;

public class SpachEngineWindow : MeowcleWindow
{
    private int shaderDefault;
    private int vaoPlayer;
    private int ulColor;
    private int ulModel;
    private int ulProjj;
    
    protected Vector3[] players;
    protected Vector3[] colors;
    
    protected override void OnLoadGraphics()
    {
        GL.UseProgram(shaderDefault);
        GL.BindVertexArray(vaoPlayer);
        
        ulColor = GL.GetUniformLocation(shaderDefault, "color");
        ulModel = GL.GetUniformLocation(shaderDefault, "model");
        ulProjj = GL.GetUniformLocation(shaderDefault, "projj");
    }

    protected override void OnRenderFrame(double dt)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
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
        Vector3 color = colors[i];
        Vector3 pos = players[i];
        Matrix4 model = Matrix4.CreateTranslation(pos);
        GL.UniformMatrix4(ulModel, true, ref model);
        GL.Uniform3(ulColor, color);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
    
    protected override void OnUnloadGraphics()
    {
        GL.DeleteProgram(shaderDefault);
    }
}