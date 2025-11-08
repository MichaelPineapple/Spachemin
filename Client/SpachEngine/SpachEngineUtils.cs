using OpenTK.Graphics.OpenGL4;

namespace SpachEngine;

internal class SpachEngineUtils
{
    public static int LoadShader(string pathShaderVert, string pathShaderFrag)
    {
        string srcShaderVert = File.ReadAllText(pathShaderVert);
        string srcShaderFrag = File.ReadAllText(pathShaderFrag);
        
        int shaderVert = GL.CreateShader(ShaderType.VertexShader);
        int shaderFrag = GL.CreateShader(ShaderType.FragmentShader);
        
        GL.ShaderSource(shaderVert, srcShaderVert);
        GL.ShaderSource(shaderFrag, srcShaderFrag);
        
        GL.CompileShader(shaderVert);
        GL.CompileShader(shaderFrag);
        
        GL.GetShader(shaderVert, ShaderParameter.CompileStatus, out int succ1);
        GL.GetShader(shaderFrag, ShaderParameter.CompileStatus, out int succ2);
        
        if (succ1 == 0) Console.WriteLine(GL.GetShaderInfoLog(shaderVert));
        if (succ2 == 0) Console.WriteLine(GL.GetShaderInfoLog(shaderFrag));
        
        int program = GL.CreateProgram();

        GL.AttachShader(program, shaderVert);
        GL.AttachShader(program, shaderFrag);
        
        GL.LinkProgram(program);
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int succ3);
        
        if (succ3 == 0) Console.WriteLine(GL.GetProgramInfoLog(program));
        
        GL.DetachShader(program, shaderVert);
        GL.DetachShader(program, shaderFrag);
        
        GL.DeleteShader(shaderFrag);
        GL.DeleteShader(shaderVert);

        return program;
    }
    
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