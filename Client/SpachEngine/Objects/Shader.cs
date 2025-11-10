using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SpachEngine.Objects;

public class Shader
{
    internal int handle { get; private set; }
    
    private int ulModel;
    private int ulVieww;
    private int ulProjj;
    private int ulColor;
    private int ulLightAmb;
    private int ulLightDirDir;
    private int ulLightDirColor;
    
    public Shader(string pathVert, string pathFrag)
    {
        string srcShaderVert = File.ReadAllText(pathVert);
        string srcShaderFrag = File.ReadAllText(pathFrag);
        handle = CreateShader(srcShaderVert, srcShaderFrag);
        
        ulColor = GL.GetUniformLocation(handle, "color");
        ulModel = GL.GetUniformLocation(handle, "model");
        ulVieww = GL.GetUniformLocation(handle, "vieww");
        ulProjj = GL.GetUniformLocation(handle, "projj");
        
        ulLightAmb = GL.GetUniformLocation(handle, "lightAmb");
        ulLightDirDir = GL.GetUniformLocation(handle, "lightDirDir");
        ulLightDirColor = GL.GetUniformLocation(handle, "lightDirColor");
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

    private void SetMatrix4(int ul, Matrix4 matrix)
    {
        GL.UniformMatrix4(ul, true, ref matrix);
    }

    internal void SetModel(Matrix4 model)
    {
        SetMatrix4(ulModel, model);
    }
    
    internal void SetView(Matrix4 view)
    {
        SetMatrix4(ulVieww, view);
    }
    
    internal void SetProjection(Matrix4 projection)
    {
        SetMatrix4(ulProjj, projection);
    }
    
    internal void SetColor(Vector3 color)
    {
        GL.Uniform3(ulColor, color);
    }
    
    internal void SetLightAmbient(Vector3 color)
    {
        GL.Uniform3(ulLightAmb, color);
    }
    
    internal void SetLightDirectional(Vector3 color, Vector3 direction)
    {
        GL.Uniform3(ulLightDirColor, color);
        GL.Uniform3(ulLightDirDir, direction);
    }

    internal void SetTexture(Texture tex)
    {
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, tex.handle);
    }

    internal void Use()
    {
        GL.UseProgram(handle);
    }
}