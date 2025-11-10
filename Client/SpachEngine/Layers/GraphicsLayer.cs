using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpachEngine.Layers.MeowcleTK;
using SpachEngine.Objects;

namespace SpachEngine.Layers;

public class GraphicsLayer : MeowcleWindow
{
    protected List<GameObject> gameObjects = new List<GameObject>();

    public Camera? camera;
    
    private int shaderHandle;
    private int ulModel;
    private int ulVieww;
    private int ulProjj;
    private int ulColor;
    private int ulLightAmb;
    private int ulLightDirDir;
    private int ulLightDirColor;

    public Vector3 lightAmbient = Vector3.Zero;
    public Vector3 lightDirectional = Vector3.One;
    public Vector3 lightDirectionalDirection = Vector3.Zero;
    
    protected void Load(Shader _shader)
    {
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace); 
        GL.CullFace(TriangleFace.Back);

        shaderHandle = _shader.handle;
        GL.UseProgram(shaderHandle);
        ulColor = GL.GetUniformLocation(shaderHandle, "color");
        ulModel = GL.GetUniformLocation(shaderHandle, "model");
        ulVieww = GL.GetUniformLocation(shaderHandle, "vieww");
        ulProjj = GL.GetUniformLocation(shaderHandle, "projj");
        
        ulLightAmb = GL.GetUniformLocation(shaderHandle, "lightAmb");
        ulLightDirDir = GL.GetUniformLocation(shaderHandle, "lightDirDir");
        ulLightDirColor = GL.GetUniformLocation(shaderHandle, "lightDirColor");
        
        GL.Uniform3(ulLightAmb, lightAmbient);
        GL.Uniform3(ulLightDirColor, lightDirectional);
        GL.Uniform3(ulLightDirDir, lightDirectionalDirection);
    }
    
    protected override void OnRenderFrame(double dt)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        if (camera != null)
        {
            Matrix4 vieww = camera.GetViewMatrix();
            Matrix4 projj = camera.GetProjectionMatrix(meowcleAspectRatio);
            GL.UniformMatrix4(ulVieww, true, ref vieww);
            GL.UniformMatrix4(ulProjj, true, ref projj);
        }

        for (int i = 0; i < gameObjects.Count; i++) RenderObject(gameObjects[i]);
    }
    
    private void RenderObject(GameObject obj)
    {
        Mesh mesh = obj.mesh;
        Texture tex = obj.texture;
        Vector3 pos = obj.position;
        Vector3 rot = obj.rotation;
        Vector3 color = obj.color;
        Matrix4 model = Matrix4.Identity;
        model *= Matrix4.CreateRotationZ(rot.Z);
        model *= Matrix4.CreateRotationY(rot.Y);
        model *= Matrix4.CreateRotationX(rot.X);
        model *= Matrix4.CreateTranslation(pos);
        GL.UniformMatrix4(ulModel, true, ref model);
        GL.Uniform3(ulColor, color);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, tex.handle);
        GL.BindVertexArray(mesh.vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.vertexLength);
    }
    
    protected override void OnUnload()
    {
        GL.DeleteProgram(shaderHandle);
    }
}