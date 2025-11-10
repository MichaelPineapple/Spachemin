using OpenTK.Mathematics;
using SpachEngine.Objects;

namespace Spachemin;

public class SpacheminPlayer : Player
{
    private const float SPEED = 0.001f;
    private float lookSensitivity = 0.002f;
    private Vector2 prevMousePos = Vector2.Zero;
    
    public SpacheminPlayer(Vector3 _pos, Mesh _mesh, Texture _tex) : base(_pos, _mesh, _tex) { }
    
    public void ProcessInput(Input input)
    {
        float deltaX = input.mouseX - prevMousePos.X;
        float deltaY = input.mouseY - prevMousePos.Y;
        prevMousePos = new Vector2(input.mouseX, input.mouseY);
        SetYaw(GetYaw() + (deltaX * lookSensitivity));
        SetPitch(GetPitch() - (deltaY * lookSensitivity));
        rotation.Y = -GetYaw();

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