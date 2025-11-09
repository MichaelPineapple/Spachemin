using OpenTK.Graphics.OpenGL4;

namespace SpachEngine;

public class Mesh
{
    internal int VAO { get; private set; }
    
    public Mesh(string path, Shader shader)
    {
        float[] mesh = ReadMeshData(path);
        VAO = CreateVAO(mesh, shader.Handle);
    }

    private static float[] ReadMeshData(string path)
    {
        string data = File.ReadAllText(path);
        data = data.Replace(" ", "").Replace("\t", "").Replace("\n", "");
        string[] split = data.Split(',');
        float[] output = new float[split.Length];
        for (int i = 0; i < split.Length; i++) output[i] = float.Parse(split[i]);
        return output;
    }
    
    private static int CreateVAO(float[] mesh, int shader)
    {
        const int typeSize = sizeof(float);
        const int vertexSize = 8 * typeSize;
        
        int aVert = GL.GetAttribLocation(shader, "aVert");
        int aTex  = GL.GetAttribLocation(shader, "aTex");
        int aNorm = GL.GetAttribLocation(shader, "aNormal");
        
        int VAO = GL.GenVertexArray();
        GL.BindVertexArray(VAO);
        int VBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, mesh.Length * sizeof(float), mesh, BufferUsageHint.DynamicDraw);
        
        GL.VertexAttribPointer(aVert, 3, VertexAttribPointerType.Float, false, vertexSize, 0);
        GL.VertexAttribPointer(aTex,  2, VertexAttribPointerType.Float, false, vertexSize, 3 * typeSize);
        GL.VertexAttribPointer(aNorm, 3, VertexAttribPointerType.Float, false, vertexSize, 5 * typeSize);
        
        GL.EnableVertexAttribArray(aVert);
        GL.EnableVertexAttribArray(aTex);
        GL.EnableVertexAttribArray(aNorm);
    
        return VAO;
    }
}