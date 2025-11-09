using OpenTK.Graphics.OpenGL4;

namespace SpachEngine;

public class Shader
{
    internal int Handle { get; private set; }
    
    public Shader(string pathVert, string pathFrag)
    {
        string srcShaderVert = File.ReadAllText(pathVert);
        string srcShaderFrag = File.ReadAllText(pathFrag);
        Handle = CreateShader(srcShaderVert, srcShaderFrag);
    }
    
    public static int CreateShader(string srcShaderVert, string srcShaderFrag)
    {
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
}