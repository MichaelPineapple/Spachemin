using OpenTK.Mathematics;
using SpachEngine;

namespace Spachemin;

public class Player : GameObject
{
    private const float SPEED = 0.025f;

    public Camera? Camera;
    
    private Vector3 Front = Vector3.UnitZ;
    private Vector3 Up = Vector3.UnitY;
    private Vector3 Right = Vector3.UnitX;
    
    private float Pitch;
    private float Yaw = -MathHelper.PiOver2;
    
    private Vector2 PrevMousePos = Vector2.Zero;
    private float LookSensitivity = 0.2f;

    public Player(Camera? camera, Mesh mesh, Texture tex) : base(mesh, tex)
    {
        Camera = camera;
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
    
    private void UpdateVectors()
    {
        Front.X = MathF.Cos(Pitch) * MathF.Cos(Yaw);
        Front.Y = MathF.Sin(Pitch);
        Front.Z = MathF.Cos(Pitch) * MathF.Sin(Yaw);
        Front = Vector3.Normalize(Front);
        Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }
    
    public void OnUpdate(Input input)
    {
        ProcessInput(input);
        if (Camera != null)
        {
            Camera.Position = position;
            Camera.Front = Front;
            Camera.Up = Up;
        }
    }

    private void ProcessInput(Input input)
    {
        Vector3 forwardDelta = Front * SPEED;
        Vector3 rightDelta = Right * SPEED;
        Vector3 upDelta = Up * SPEED;
        if (input.F) position += forwardDelta;
        if (input.B) position -= forwardDelta;
        if (input.L) position -= rightDelta;
        if (input.R) position += rightDelta;
        if (input.U) position += upDelta;
        if (input.D) position -= upDelta;
        float deltaX = input.MouseX - PrevMousePos.X;
        float deltaY = input.MouseY - PrevMousePos.Y;
        PrevMousePos = new Vector2(input.MouseX, input.MouseY);
        SetYaw(GetYaw() + (deltaX * LookSensitivity));
        SetPitch(GetPitch() - (deltaY * LookSensitivity));
    }
}