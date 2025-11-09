using OpenTK.Mathematics;

namespace SpachEngine;

public class Camera
{
    public Vector3 Position = Vector3.Zero;
    
    public Vector3 Front = Vector3.UnitZ;
    public Vector3 Up = Vector3.UnitY;
    public Vector3 Right = Vector3.UnitX;
    
    private float Pitch;
    private float Yaw = -MathHelper.PiOver2;
    private float FOV = MathHelper.PiOver2;

    public Vector3 GetFront()
    {
        return this.Front;
    }

    public Vector3 GetUp()
    {
        return this.Up;
    }

    public Vector3 GetRight()
    {
        return this.Right;
    }
    
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
    
    public float GetPitch()
    {
        return MathHelper.RadiansToDegrees(Pitch);
    }

    public void SetPitch(float val)
    {
        float angle = MathHelper.Clamp(val, -89f, 89f);
        Pitch = MathHelper.DegreesToRadians(angle);
        UpdateVectors();
    }
        
    public float GetYaw()
    {
        return MathHelper.RadiansToDegrees(Yaw);
    }

    public void SetYaw(float val)
    {
        Yaw = MathHelper.DegreesToRadians(val);
        UpdateVectors();
    }
    
    internal Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + Front, Up);
    }

    internal Matrix4 GetProjectionMatrix(float aspectRatio)
    {
        return Matrix4.CreatePerspectiveFieldOfView(FOV, aspectRatio, 0.01f, 100f);
    }
    
    private void UpdateVectors()
    {
        Front.X = MathF.Cos(Pitch) * MathF.Cos(Yaw);
        Front.Y = MathF.Sin(Pitch);
        Front.Z = MathF.Cos(Pitch) * MathF.Sin(Yaw);
        Front = Vector3.Normalize(Front);
        Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }
}