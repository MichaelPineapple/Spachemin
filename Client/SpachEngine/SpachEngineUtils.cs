using OpenTK.Graphics.OpenGL4;

namespace SpachEngine;

internal class SpachEngineUtils
{
    internal static int CreateShader()
    {
        const string vertexShaderSource =
            "#version 330 core \n" +
            "in vec2 vert;" +
            "uniform mat4 model;" +
            "uniform mat4 projj;" +
            "void main(){" +
            "gl_Position = vec4(vert, 0.0, 1.0) * model * projj;" +
            "}";

        const string fragmentShaderSource =
            "#version 330 core \n" +
            "out vec4 FragColor;" +
            "uniform vec3 color;" +
            "void main(){" +
            "FragColor = vec4(color, 1.0f);" +
            "}";
        
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        
        GL.CompileShader(vertexShader);
        GL.CompileShader(fragmentShader);
        
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int succ1);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int succ2);
        
        if (succ1 == 0) Console.WriteLine(GL.GetShaderInfoLog(vertexShader));
        if (succ2 == 0) Console.WriteLine(GL.GetShaderInfoLog(fragmentShader));
        
        int program = GL.CreateProgram();

        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        
        GL.LinkProgram(program);
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int succ3);
        
        if (succ3 == 0) Console.WriteLine(GL.GetProgramInfoLog(program));
        
        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);
        
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);

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