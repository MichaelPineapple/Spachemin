using OpenTK.Graphics.OpenGL4;

namespace SpachEngine;

internal class SpachEngineUtils
{
    internal static int CreateVAO(float[] mesh, int shader)
    {
        const int typeSize = sizeof(float); 
        int vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);
        int VBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, mesh.Length * typeSize, mesh, BufferUsageHint.DynamicDraw);
        int aVert = GL.GetAttribLocation(shader, "vert");
        GL.VertexAttribPointer(aVert, 2, VertexAttribPointerType.Float, false, 2 * typeSize, 0);
        GL.EnableVertexAttribArray(aVert);
        return vao;
    }

    internal static float[] CreateSquareMesh(float size)
    {
        return new []
        {
            -size,  size,
             size, -size,
            -size, -size,
        
            -size,  size,
             size,  size,
             size, -size,
        };
    }
}