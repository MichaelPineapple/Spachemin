using OpenTK.Mathematics;

namespace SpachEngine.Objects;

public class Player : PhysicsObject
{
    private Vector3 front = Vector3.UnitZ;
    private Vector3 up = Vector3.UnitY;
    private Vector3 right = Vector3.UnitX;
    
    private float pitch;
    private float yaw = -MathHelper.PiOver2;
    
    public Player(Vector3 _pos, Mesh _mesh, Texture _tex) : base(_pos, _mesh, _tex) { }
    
    private void UpdateVectors()
    {
        front.X = MathF.Cos(pitch) * MathF.Cos(yaw);
        front.Y = MathF.Sin(pitch);
        front.Z = MathF.Cos(pitch) * MathF.Sin(yaw);
        front = Vector3.Normalize(front);
        right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
        up = Vector3.Normalize(Vector3.Cross(right, front));
    }
    
    public void SetPitch(float val)
    {
        const float clamp = MathHelper.PiOver2 - 0.001f;
        float angle = MathHelper.Clamp(val, -clamp, clamp);
        pitch = angle;
        UpdateVectors();
    }
    
    public void SetYaw(float val)
    {
        yaw = val;
        UpdateVectors();
    }
    
    public Vector3 GetFrontVector()
    {
        return front;
    }
    
    public Vector3 GetRightVector()
    {
        return right;
    }

    public Vector3 GetUpVector()
    {
        return up;
    }
    
    public float GetPitch()
    {
        return pitch;
    }
    
    public float GetYaw()
    {
        return yaw;
    }
}