using OpenTK.Mathematics;

namespace SpachEngine;

public class GameObject
{
    public Mesh mesh;
    public Texture texture;
    
    public Vector3 position = Vector3.Zero;
    public Vector3 rotation = Vector3.Zero;
    public Vector3 color = Vector3.One;
    
    public GameObject(Mesh _mesh, Texture _tex)
    {
        mesh = _mesh;
        texture = _tex;
    }
}