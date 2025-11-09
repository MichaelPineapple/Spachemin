using OpenTK.Mathematics;

namespace Spachemin;

public struct Player
{
    public Vector3 Position;
    public Vector3 Color;

    public Player()
    {
        Position = Vector3.Zero;
        Color = Vector3.One;
    }
}