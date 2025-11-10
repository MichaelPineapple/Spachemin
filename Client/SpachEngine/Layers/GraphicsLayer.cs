using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpachEngine.Layers.MeowcleTK;
using SpachEngine.Objects;

namespace SpachEngine.Layers;

public class GraphicsLayer : MeowcleWindow
{
    protected List<GameObject> gameObjects = new List<GameObject>();

    public Camera? camera;
    private int currentShader;

    public Vector3 lightAmbient = Vector3.Zero;
    public Vector3 lightDirectional = Vector3.One;
    public Vector3 lightDirectionalDirection = Vector3.Zero;

    public GraphicsLayer()
    {
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace); 
        GL.CullFace(TriangleFace.Back);
    }
    
    protected override void OnRenderFrame(double dt)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        for (int i = 0; i < gameObjects.Count; i++) RenderObject(gameObjects[i]);
    }
    
    private void RenderObject(GameObject obj)
    {
        Mesh mesh = obj.mesh;
        Shader shader = mesh.shader;
        
        SwitchShaderIfNecessary(shader);
        
        Texture tex = obj.texture;
        Vector3 pos = obj.position;
        Vector3 rot = obj.rotation;
        Vector3 color = obj.color;
        
        Matrix4 model = Matrix4.Identity;
        model *= Matrix4.CreateRotationZ(rot.Z);
        model *= Matrix4.CreateRotationY(rot.Y);
        model *= Matrix4.CreateRotationX(rot.X);
        model *= Matrix4.CreateTranslation(pos);

        shader.SetModel(model);
        shader.SetColor(color);
        shader.SetTexture(tex);
        
        GL.BindVertexArray(mesh.vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.vertexLength);
    }
    
    private void SwitchShaderIfNecessary(Shader shader)
    {
        if (shader.handle != currentShader)
        {
            shader.Use();

            if (camera != null)
            {
                shader.SetView(camera.GetViewMatrix());
                shader.SetProjection(camera.GetProjectionMatrix(meowcleAspectRatio));
            }
            
            shader.SetLightAmbient(lightAmbient);
            shader.SetLightDirectional(lightDirectional, lightDirectionalDirection);

            currentShader = shader.handle;
        }
    }
}