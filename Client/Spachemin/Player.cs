using OpenTK.Mathematics;
using SpachEngine;

namespace Spachemin;

public class Player : PhysicsObject
{
    private const float SPEED = 0.001f;
    
    private Vector3 front = Vector3.UnitZ;
    private Vector3 up = Vector3.UnitY;
    private Vector3 right = Vector3.UnitX;
    
    private float pitch;
    private float yaw = -MathHelper.PiOver2;
    
    private Vector2 prevMousePos = Vector2.Zero;
    private float lookSensitivity = 0.2f;
    
    public Player(Mesh _mesh, Texture _tex) : base(_mesh, _tex) { }

    public Vector3 GetFrontVector()
    {
        return front;
    }

    public Vector3 GetUpVector()
    {
        return up;
    }
    
    public float GetPitch()
    {
        return MathHelper.RadiansToDegrees(pitch);
    }

    public void SetPitch(float val)
    {
        float angle = MathHelper.Clamp(val, -89f, 89f);
        pitch = MathHelper.DegreesToRadians(angle);
        UpdateVectors();
    }
        
    public float GetYaw()
    {
        return MathHelper.RadiansToDegrees(yaw);
    }

    public void SetYaw(float val)
    {
        yaw = MathHelper.DegreesToRadians(val);
        UpdateVectors();
    }
    
    private void UpdateVectors()
    {
        front.X = MathF.Cos(pitch) * MathF.Cos(yaw);
        front.Y = MathF.Sin(pitch);
        front.Z = MathF.Cos(pitch) * MathF.Sin(yaw);
        front = Vector3.Normalize(front);
        right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
        up = Vector3.Normalize(Vector3.Cross(right, front));
    }

    public void ProcessInput(Input input)
    {
        float deltaX = input.MouseX - prevMousePos.X;
        float deltaY = input.MouseY - prevMousePos.Y;
        prevMousePos = new Vector2(input.MouseX, input.MouseY);
        SetYaw(GetYaw() + (deltaX * lookSensitivity));
        SetPitch(GetPitch() - (deltaY * lookSensitivity));
        rotation.Y = -yaw;
        
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