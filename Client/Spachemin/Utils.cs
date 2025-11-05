using OpenTK.Graphics.OpenGL4;

namespace Spachemin;

public class Utils
{
    public static int CreateShader()
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
    
    public static int CreateVAO(float[] mesh, int shader)
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

    public static float[] CreateSquareMesh(float size)
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
    
    public static byte[] EncodeInput(Input _data)
    {
        bool[] bools = new []
        {
            _data.W,
            _data.A,
            _data.S,
            _data.D,
        };
        return new[] { ByteFromBools(bools) };
    }

    public static Input DecodeInput(byte[] _data)
    {
        bool[] bools = ByteToBools(_data[0]);
        Input output = new Input();
        output.W = bools[0];
        output.A = bools[1];
        output.S = bools[2];
        output.D = bools[3];
        return output;
    }
    
    public static byte ByteFromBools(bool[] bools)
    {
        int len = bools.Length;
        if (len > 8) throw new Exception("Data cannot be longer than 8!");
        byte output = 0;
        byte x = 1;
        const byte two = 2;
        for (int i = 0; i < bools.Length; i++)
        {
            if (bools[i]) output |= x;
            x *= two;
        }
        return output;
    }
    
    public static bool[] ByteToBools(byte bytes)
    {
        bool[] output = new bool[8];
        byte x = 1;
        const byte two = 2;
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = (bytes & x) != 0;
            x *= two;
        }
        return output;
    }
}

public struct Input
{
    public bool W, A, S, D;
    public Input() { W = A = S = D = false; }
}