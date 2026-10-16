using Godot;
using System;
using System.Collections.Generic;
using WeaponData;

public partial class PlayerControl : CharacterBody3D, IGravity, IDamageable, IStats
{
    [Signal] public delegate void DiedEventHandler();
    [Signal] public delegate void PlayerAttackEventHandler();

    // IGravity implementation
    [Export] public float GravityScale { get; set; } = 1.0f;

    // Stats
    [Export] public float Health { get; private set; } = 100f;
    [Export] public float MaxHealth { get; private set; } = 100f;
    [Export] public float Damage { get; private set; } = 10f;
    [Export] public float CritChance { get; private set; } = 1f;
    [Export] public float CritDamage { get; private set; } = 2f;
    [Export] public float ArmorPenetration { get; private set; } = 0f;
    [Export] public float AttackSpeed { get; private set; } = 1f;
    [Export] public float Armor { get; private set; } = 0f;
    [Export] public float MoveSpeed { get; private set; } = 10f;
    [Export] public float Luck { get; private set; } = 0f;

    [Export] public float RotateSensitivity = 0.005f;

    [Export] public float DashDistance = 8.0f;
    [Export] public float DashDuration = 0.15f;
    [Export] public float DashCooldown = 1.0f;

    private Node3D _visuals;
    private Camera3D _camera;
    private bool _isRotating = false;

    // Dash state
    private bool _isDashing = false;
    private float _dashTimer = 0f;
    private float _dashCooldownTimer = 0f;
    private Vector3 _dashDirection = Vector3.Zero;

    private Weapon _weapon;

    public override void _Ready()
    {
        _visuals = GetNode<Node3D>("Visuals");
        _camera = GetViewport().GetCamera3D();
        _weapon = GetNode<Weapon>("Visuals/Weapon/Melee");

        Damage = WeaponDatabase.WeaponList[_weapon.Id].Damage;
        CritChance = WeaponDatabase.WeaponList[_weapon.Id].CritChance;
        CritDamage = WeaponDatabase.WeaponList[_weapon.Id].CritDamage;
        ArmorPenetration = WeaponDatabase.WeaponList[_weapon.Id].ArmorPenetration;
        AttackSpeed = WeaponDatabase.WeaponList[_weapon.Id].AttackSpeed;
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

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("attack"))
        {
            EmitSignal(SignalName.PlayerAttack);
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

        // Apply gravity via shared helper
        GravityComponent.ApplyGravity(this, delta);

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
                velocity.X = direction.X * MoveSpeed;
                velocity.Z = direction.Z * MoveSpeed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, MoveSpeed);
                velocity.Z = Mathf.MoveToward(Velocity.Z, 0, MoveSpeed);
            }
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        EmitSignal(SignalName.Died);
        QueueFree();
    }
}
