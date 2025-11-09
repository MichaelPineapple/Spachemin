using OpenTK.Mathematics;

namespace SpachEngine;

public class Camera
{
    public Vector3 Position = Vector3.Zero;
    public Vector3 Front = Vector3.UnitZ;
    public Vector3 Up = Vector3.UnitY;
    
    private float FOV = MathHelper.PiOver2;
    
    public float GetFov()
    {
        return MathHelper.RadiansToDegrees(FOV);
    }
        
    public void SetFov(float val)
    {
        float angle = MathHelper.Clamp(val, 0.0001f, 180.0f);
        FOV = MathHelper.DegreesToRadians(angle);
    }

    public void AddFov(float val)
    {
        SetFov(GetFov() + val);
    }
    
    internal Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + Front, Up);
    }

    internal Matrix4 GetProjectionMatrix(float aspectRatio)
    {
        return Matrix4.CreatePerspectiveFieldOfView(FOV, aspectRatio, 0.01f, 100f);
    }
}