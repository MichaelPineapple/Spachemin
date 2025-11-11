using MclTK;
using OpenTK.Mathematics;

namespace SoupEngine;

public class SoupObject : MclObject
{
    public Vector3 Velocity = Vector3.Zero;

    public SoupObject(Vector3 pos, MclMesh mesh, MclTexture tex) : base(pos, mesh, tex) { }
    
    public void ApplyForce(Vector3 force)
    {
        Velocity += force;
    }

    public void Update()
    {
        Position += Velocity;
    }
}