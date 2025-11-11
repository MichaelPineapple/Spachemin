using OpenTK.Mathematics;

namespace SoupEngine;

public class GravitySource
{
    public Vector3 Position;
    public float Mass;
    public float Radius;
    
    public GravitySource(Vector3 pos, float mass, float radius)
    {
        Position = pos;
        Mass = mass;
        Radius = radius;
    }
}