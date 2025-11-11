using MclTK;
using OpenTK.Mathematics;

namespace SoupEngine;

public class SoupPlayer : SoupObject
{
    private Vector3 Front = Vector3.UnitZ;
    private Vector3 Up = Vector3.UnitY;
    private Vector3 Right = Vector3.UnitX;
    
    private float Pitch;
    private float Yaw = -MathHelper.PiOver2;
    
    public SoupPlayer(Vector3 pos, MclMesh mesh, MclTexture tex) : base(pos, mesh, tex) { }
    
    public void SetPitch(float val)
    {
        const float clamp = MathHelper.PiOver2 - 0.001f;
        float angle = MathHelper.Clamp(val, -clamp, clamp);
        Pitch = angle;
        UpdateVectors();
    }
    
    public void SetYaw(float val)
    {
        Yaw = val;
        UpdateVectors();
    }
    
    public Vector3 GetFrontVector()
    {
        return Front;
    }
    
    public Vector3 GetRightVector()
    {
        return Right;
    }

    public Vector3 GetUpVector()
    {
        return Up;
    }
    
    public float GetPitch()
    {
        return Pitch;
    }
    
    public float GetYaw()
    {
        return Yaw;
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