using OpenTK.Mathematics;

namespace Spachemin;

public class GravitySource
{
    public Vector3 position;
    public float mass;
    
    public GravitySource(Vector3 _pos, float _mass)
    {
        position = _pos;
        mass = _mass;
    }
}