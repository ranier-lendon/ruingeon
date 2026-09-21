using Godot;
using System;

public partial class PlayerControl : CharacterBody3D
{
    public const float Speed = 10.0f;
    public const float JumpVelocity = 4.5f;

    private Node3D _visuals;
    private Camera3D _camera;

    public override void _Ready()
    {
        _visuals = GetNode<Node3D>("Visuals");
        _camera = GetViewport().GetCamera3D();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        Vector2 inputDir = Input.GetVector("rightward", "leftward", "backward", "forward");
        
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();

        // Rotate Visuals to face the mouse cursor on the XZ plane
        if (_visuals != null && _camera != null)
        {
            Vector2 mousePos = GetViewport().GetMousePosition();
            Vector3 rayOrigin = _camera.ProjectRayOrigin(mousePos);
            Vector3 rayDir = _camera.ProjectRayNormal(mousePos);

            // Intersect ray with the horizontal plane at the player's Y position
            float planeY = GlobalPosition.Y;
            if (Mathf.Abs(rayDir.Y) > 0.001f)
            {
                float t = (planeY - rayOrigin.Y) / rayDir.Y;
                Vector3 worldPoint = rayOrigin + rayDir * t;

                Vector3 toMouse = worldPoint - GlobalPosition;
                if (toMouse.LengthSquared() > 0.001f)
                {
                    float angle = Mathf.Atan2(toMouse.X, toMouse.Z);
                    _visuals.Rotation = new Vector3(_visuals.Rotation.X, angle, _visuals.Rotation.Z);
                }
            }
        }
    }
}
