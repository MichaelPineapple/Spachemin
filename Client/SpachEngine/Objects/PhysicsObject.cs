using OpenTK.Mathematics;

namespace SpachEngine.Objects;

public class PhysicsObject : GameObject
{
    public Vector3 velocity = Vector3.Zero;

    public PhysicsObject(Vector3 _pos, Mesh _mesh, Texture _tex) : base(_pos, _mesh, _tex) { }
    
    public void ApplyForce(Vector3 force)
    {
        velocity += force;
    }

    public void Update()
    {
        position += velocity;
    }
}