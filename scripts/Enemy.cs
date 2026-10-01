using Godot;

/// <summary>
/// Enemy AI — chases the player by moving directly toward them.
/// No NavigationRegion3D or baked NavMesh required.
///
/// Only prerequisite:
///   Tag the player node with the group "player" in the Godot editor
///   (select Player → Node tab → Groups → add "player").
/// </summary>
public partial class Enemy : CharacterBody3D, IGravity
{
    // ── Inspector-tunable fields ──────────────────────────────────────────────

    /// <summary>Movement speed in world units per second.</summary>
    [Export] public float MoveSpeed = 4.0f;

    /// <summary>Distance at which the enemy stops moving (melee range).</summary>
    [Export] public float StopDistance = 1.2f;

    /// <summary>Distance at which the enemy starts chasing the player.</summary>
    [Export] public float DetectionRange = 20.0f;

    // IGravity ─────────────────────────────────────────────────────────────────
    [Export] public float GravityScale { get; set; } = 1.0f;

    // ── Private state ─────────────────────────────────────────────────────────
    private Node3D _visuals;
    private CharacterBody3D _player;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    public override void _Ready()
    {
        _visuals = GetNode<Node3D>("Visuals");
        _player  = FindPlayer();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Always apply gravity first
        GravityComponent.ApplyGravity(this, delta);

        // Re-find the player if reference became invalid (e.g. respawn)
        if (!IsInstanceValid(_player))
            _player = FindPlayer();

        if (_player != null)
            ChasePlayer((float)delta);

        MoveAndSlide();
    }

    // ── Chase logic ───────────────────────────────────────────────────────────

    private void ChasePlayer(float delta)
    {
        float dist = GlobalPosition.DistanceTo(_player.GlobalPosition);

        // Too far away — freeze
        if (dist > DetectionRange)
        {
            BrakeHorizontal();
            return;
        }

        // Close enough — stop and face the player
        if (dist <= StopDistance)
        {
            BrakeHorizontal();
            FacePlayer();
            return;
        }

        // Compute flat direction toward the player (Y handled by gravity)
        Vector3 toPlayer = _player.GlobalPosition - GlobalPosition;
        toPlayer.Y = 0f;
        toPlayer   = toPlayer.Normalized();

        // Apply velocity
        Vector3 velocity = Velocity;
        velocity.X = toPlayer.X * MoveSpeed;
        velocity.Z = toPlayer.Z * MoveSpeed;
        Velocity   = velocity;

        FacePlayer();
    }

    /// <summary>Smoothly brakes horizontal movement to zero.</summary>
    private void BrakeHorizontal()
    {
        Vector3 vel = Velocity;
        vel.X  = Mathf.MoveToward(vel.X, 0f, MoveSpeed);
        vel.Z  = Mathf.MoveToward(vel.Z, 0f, MoveSpeed);
        Velocity = vel;
    }

    /// <summary>Rotates the Visuals node to face the player (flat, on XZ plane).</summary>
    private void FacePlayer()
    {
        if (!IsInstanceValid(_visuals) || !IsInstanceValid(_player))
            return;

        Vector3 lookTarget = new Vector3(
            _player.GlobalPosition.X,
            GlobalPosition.Y,           // keep same Y so we don't tilt
            _player.GlobalPosition.Z
        );

        if (lookTarget.DistanceTo(GlobalPosition) > 0.01f)
        {
            _visuals.LookAt(lookTarget, Vector3.Up);
            _visuals.RotateY(Mathf.Pi); // flip so the eye faces forward
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Returns the first CharacterBody3D in the "player" group.</summary>
    private CharacterBody3D FindPlayer()
    {
        var nodes = GetTree().GetNodesInGroup("player");
        foreach (var n in nodes)
        {
            if (n is CharacterBody3D cb)
                return cb;
        }
        return null;
    }
}

