using OpenTK.Mathematics;

namespace SpachEngine;

public class Camera
{
    //private const float SPEED = 1.0f;
    
    public  Vector3 Position;
    
    private Vector3 Front = -Vector3.UnitZ;
    private Vector3 Up = Vector3.UnitY;
    private Vector3 Right = Vector3.UnitX;
    //private Vector2 PrevMousePos = Vector2.Zero;
    
    private float Pitch;
    private float Yaw = -MathHelper.PiOver2;
    private float FOV = MathHelper.PiOver2;
    //private float Sensitivity = 0.2f;
    
    //private bool FirstMove = true;
    
    public Camera(Vector3 position)
    {
        Position = position;
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

    public void addFov(float val)
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
    
    // public void OnUpdate(Input input)
    // {
    //     Vector3 forwardDelta = Front * SPEED;
    //     Vector3 rightDelta = Right * SPEED;
    //     Vector3 upDelta = Up * SPEED;
    //     if (input.F) Position += forwardDelta;
    //     if (input.B) Position -= forwardDelta;
    //     if (input.L) Position -= rightDelta;
    //     if (input.R) Position += rightDelta;
    //     if (input.U) Position += upDelta;
    //     if (input.D) Position -= upDelta;
    //     
    //     if (FirstMove) 
    //     {
    //         PrevMousePos = new Vector2(input.MouseX, input.MouseY);
    //         FirstMove = false;
    //     }
    //     else
    //     {
    //         float deltaX = input.MouseX - PrevMousePos.X;
    //         float deltaY = input.MouseY - PrevMousePos.Y;
    //         PrevMousePos = new Vector2(input.MouseX, input.MouseY);
    //         SetYaw(GetYaw() + (deltaX * Sensitivity));
    //         SetPitch(GetPitch() - (deltaY * Sensitivity));
    //     }
    // }
}