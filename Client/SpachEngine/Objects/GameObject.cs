using OpenTK.Mathematics;

namespace SpachEngine.Objects;

public class GameObject
{
    public Mesh mesh;
    public Texture texture;
    
    public Vector3 position;
    public Vector3 rotation = Vector3.Zero;
    public Vector3 color = Vector3.One;
    
    public GameObject(Vector3 _pos, Mesh _mesh, Texture _tex)
    {
        position = _pos;
        mesh = _mesh;
        texture = _tex;
    }
}