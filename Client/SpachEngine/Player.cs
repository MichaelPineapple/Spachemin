using OpenTK.Mathematics;

namespace SpachEngine;

public class Player : Camera
{
    private const float SPEED = 0.025f;
    public Vector3 Color = Vector3.One;
    private Vector2 PrevMousePos = Vector2.Zero;
    private float LookSensitivity = 0.2f;
    private bool FirstMove = true;

    public bool OnUpdate(Input input)
    {
        return ProcessInput(input);
    }

    private bool ProcessInput(Input input)
    {
        if (input.Quit) return true;
        
        Vector3 forwardDelta = Front * SPEED;
        Vector3 rightDelta = Right * SPEED;
        Vector3 upDelta = Up * SPEED;
        if (input.F) Position += forwardDelta;
        if (input.B) Position -= forwardDelta;
        if (input.L) Position -= rightDelta;
        if (input.R) Position += rightDelta;
        if (input.U) Position += upDelta;
        if (input.D) Position -= upDelta;
        
        if (FirstMove) 
        {
            PrevMousePos = new Vector2(input.MouseX, input.MouseY);
            FirstMove = false;
        }
        else
        {
            float deltaX = input.MouseX - PrevMousePos.X;
            float deltaY = input.MouseY - PrevMousePos.Y;
            PrevMousePos = new Vector2(input.MouseX, input.MouseY);
            SetYaw(GetYaw() + (deltaX * LookSensitivity));
            SetPitch(GetPitch() - (deltaY * LookSensitivity));
        }

        return false;
    }
}