using Godot;
using System;

public partial class PlayerControl : CharacterBody3D
{
    public const float Speed = 10.0f;
    public const float JumpVelocity = 4.5f;

    [Export] public float RotateSensitivity = 0.005f;

    // --- Dash settings (tweak these in the Inspector) ---
    [Export] public float DashDistance = 8.0f;  // World units travelled per dash
    [Export] public float DashDuration = 0.15f; // Seconds the dash lasts
    [Export] public float DashCooldown = 1.0f;  // Seconds before you can dash again

    private Node3D _visuals;
    private Camera3D _camera;
    private bool _isRotating = false;

    // Dash state
    private bool _isDashing = false;
    private float _dashTimer = 0f;
    private float _dashCooldownTimer = 0f;
    private Vector3 _dashDirection = Vector3.Zero;

    public override void _Ready()
    {
        _visuals = GetNode<Node3D>("Visuals");
        _camera = GetViewport().GetCamera3D();
    }

    public override void _Input(InputEvent @event)
    {
        // Track rotateCam press/release
        if (@event.IsActionPressed("rotateCam"))
        {
            _isRotating = true;
        }
        else if (@event.IsActionReleased("rotateCam"))
        {
            _isRotating = false;
        }

        // Rotate player Y while right-click dragging
        if (_isRotating && @event is InputEventMouseMotion motion)
        {
            RotateY(-motion.Relative.X * RotateSensitivity);
        }

        // Start dash on "dash" action (mapped to Shift)
        if (@event.IsActionPressed("dash") && !_isDashing && _dashCooldownTimer <= 0f)
        {
            // Use the direction the Visuals node is facing (toward the mouse)
            // Visuals' +Z points toward mouse due to the RotateY(Pi) flip in _Process
            _dashDirection = _visuals.GlobalTransform.Basis.Z;
            _dashDirection.Y = 0f;
            _dashDirection = _dashDirection.Normalized();

            _isDashing = true;
            _dashTimer = DashDuration;
            _dashCooldownTimer = DashCooldown;
        }
    }

    public override void _Process(double delta)
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector3 rayOrigin = _camera.ProjectRayOrigin(mousePos);
        Vector3 rayDir = _camera.ProjectRayNormal(mousePos);

        float planeY = GlobalPosition.Y;
        if (!Mathf.IsZeroApprox(rayDir.Y))
        {
            float t = (planeY - rayOrigin.Y) / rayDir.Y;
            if (t > 0f)
            {
                Vector3 worldMousePos = rayOrigin + rayDir * t;

                Vector3 lookTarget = new Vector3(worldMousePos.X, GlobalPosition.Y, worldMousePos.Z);
                if (lookTarget.DistanceTo(GlobalPosition) > 0.01f)
                {
                    _visuals.LookAt(lookTarget, Vector3.Up);
                    _visuals.RotateY(Mathf.Pi);
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float fDelta = (float)delta;
        Vector3 velocity = Velocity;

        // Tick down cooldown regardless of dashing
        if (_dashCooldownTimer > 0f)
            _dashCooldownTimer -= fDelta;

        if (_isDashing)
        {
            _dashTimer -= fDelta;
            float dashSpeed = DashDistance / DashDuration;
            velocity.X = _dashDirection.X * dashSpeed;
            velocity.Z = _dashDirection.Z * dashSpeed;

            if (_dashTimer <= 0f)
            {
                _isDashing = false;
                _dashTimer = 0f;
            }
        }
        else
        {
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
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
