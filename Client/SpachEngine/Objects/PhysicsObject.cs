using OpenTK.Mathematics;

namespace SpachEngine;

public class PhysicsObject : GameObject
{
    public Vector3 velocity = Vector3.Zero;

    public PhysicsObject(Mesh _mesh, Texture _tex) : base(_mesh, _tex) { }
    
    public void ApplyForce(Vector3 force)
    {
        velocity += force;
    }

    public void Update()
    {
        position += velocity;
    }
}