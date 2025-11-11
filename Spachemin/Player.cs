using MclTK;
using OpenTK.Mathematics;
using SoupEngine;

namespace Spachemin;

public class Player : SoupPlayer
{
    private const float SPEED = 0.001f;
    private float LookSensitivity = 0.002f;
    private Vector2 PrevMouse = Vector2.Zero;
    
    public Player(Vector3 pos, MclMesh mesh, MclTexture tex) : base(pos, mesh, tex) { }
    
    public void ProcessInput(Input input)
    {
        float deltaX = input.MouseX - PrevMouse.X;
        float deltaY = input.MouseY - PrevMouse.Y;
        PrevMouse = new Vector2(input.MouseX, input.MouseY);
        SetYaw(GetYaw() + (deltaX * LookSensitivity));
        SetPitch(GetPitch() - (deltaY * LookSensitivity));
        Rotation.Y = -GetYaw();

        Vector3 front = GetFrontVector();
        Vector3 right = GetRightVector();
        Vector3 unitFront = new Vector3(front.X, 0.0f, front.Z).Normalized();
        Vector3 unitRight = right.Normalized();
        Vector3 unitUp = Vector3.UnitY;
        Vector3 force = Vector3.Zero;
        if (input.F) force += unitFront;
        if (input.B) force -= unitFront;
        if (input.L) force -= unitRight;
        if (input.R) force += unitRight;
        if (input.U) force += unitUp;
        if (input.D) force -= unitUp;
        if (force.Length != 0) force = force.Normalized() * SPEED;
        ApplyForce(force);
    }
}