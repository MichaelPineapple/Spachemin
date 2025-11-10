using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SpachEngine.Objects;

public class Mesh
{
    internal int vao { get; private set; }
    internal int vertexLength;
    
    public Mesh(string path, Shader shader)
    {
        float[] mesh = ReadObjFile(path);
        vertexLength = mesh.Length;
        vao = CreateVAO(mesh, shader.handle);
    }
    
    // https://en.wikipedia.org/wiki/Wavefront_.obj_file
    private static float[] ReadObjFile(string path)
    {
        try
        {
            List<float> output = new List<float>();
            List<Vector3> verticies = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();
            
            void handleF(string str)
            {
                string[] split = str.Split('/');
                int[] a = new int[3];
                for (int i = 0; i < split.Length; i++) a[i] = int.Parse(split[i]) - 1;
                for (int i = 0; i < 3; i++) output.Add(verticies[a[0]][i]);
                for (int i = 0; i < 2; i++) output.Add(texCoords[a[1]][i]);
                for (int i = 0; i < 3; i++) output.Add(normals[a[2]][i]);
            }
            
            string[] data = File.ReadAllLines(path);
            for (int i = 0; i < data.Length; i++)
            {
                string[] split = data[i].Split(' ');
                switch (split[0])
                {
                    case "v":
                        float vx = float.Parse(split[1]);
                        float vy = float.Parse(split[2]);
                        float vz = float.Parse(split[3]);
                        verticies.Add(new Vector3(vx, vy, vz));
                        break;
                    case "vn":
                        float nx = float.Parse(split[1]);
                        float ny = float.Parse(split[2]);
                        float nz = float.Parse(split[3]);
                        normals.Add(new Vector3(nx, ny, nz));
                        break;
                    case "vt":
                        float tu = float.Parse(split[1]);
                        float tv = float.Parse(split[2]);
                        texCoords.Add(new Vector2(tu, tv));
                        break;
                    case "f":
                        handleF(split[1]);
                        handleF(split[2]);
                        handleF(split[3]);
                        break;
                }
            }
            return output.ToArray();
        }
        catch
        {
            Console.WriteLine("Failed to load .obj file: " + path);
            throw;
        }
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