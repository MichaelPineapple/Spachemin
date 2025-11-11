using OpenTK.Mathematics;

namespace SoupEngine.Objects;

public class GravitySource
{
    public Vector3 Position;
    public float Mass;
    
    public GravitySource(Vector3 pos, float mass)
    {
        Position = pos;
        Mass = mass;
    }
}