using OpenTK.Mathematics;

namespace SpachEngine;

public class Camera
{
    public Vector3 Position;
    public float Zoom = 1.0f;
    
    public Camera(Vector3 position)
    {
        Position = position;
    }
    
    internal Matrix4 GetProjectionMatrix(float aspectRatio)
    {
        Vector3 pos = Position /= Zoom;
        float left =  pos.X - aspectRatio;
        float right = aspectRatio + pos.X;
        float up = pos.Y - 1.0f;
        float down = 1.0f + pos.Y;
        return Matrix4.CreateOrthographicOffCenter(left * Zoom, right * Zoom, up * Zoom, down * Zoom, 1.0f, -1.0f);
    }
}