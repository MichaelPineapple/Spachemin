using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpachEngine.MeowcleTK;

namespace SpachEngine;

public class SpachEngineWindow : MeowcleWindow
{
    private int shaderDefault;
    private int playerVAO;
    
    private int ulColor;
    private int ulModel;
    private int ulProjj;
    
    protected Vector3[] players = new Vector3[3];
    protected readonly Vector3[] colors = new Vector3[10];
    
    protected override void OnLoadGraphics()
    {
        string pathApp = Path.GetDirectoryName(Environment.ProcessPath) + "/Assets";
        
        string pathShaders = pathApp + "/Shaders";
        shaderDefault = SpachEngineUtils.LoadShader(pathShaders + "/Default/default.vert", pathShaders + "/Default/default.frag");
        GL.UseProgram(shaderDefault);

        float[] playerMesh = SpachEngineUtils.CreateSquareMesh(0.1f);
        playerVAO = SpachEngineUtils.CreateVAO(playerMesh, shaderDefault);
        GL.BindVertexArray(playerVAO);
        
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