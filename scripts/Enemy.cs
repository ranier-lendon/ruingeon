using Godot;

// Enemy AI — chases the player directly. Requires player node in group "player".
public partial class Enemy : CharacterBody3D, IGravity
{
    [Export] public float MoveSpeed = 4.0f;
    [Export] public float StopDistance = 1.2f;
    [Export] public float DetectionRange = 20.0f;

    [Export] public float GravityScale { get; set; } = 1.0f;

    private Node3D _visuals;
    private CharacterBody3D _player;

    public override void _Ready()
    {
        _visuals = GetNode<Node3D>("Visuals");
        _player  = FindPlayer();
    }

    public override void _PhysicsProcess(double delta)
    {
        GravityComponent.ApplyGravity(this, delta);

        if (!IsInstanceValid(_player))
            _player = FindPlayer();

        if (_player != null)
            ChasePlayer((float)delta);

        MoveAndSlide();
    }

    private void ChasePlayer(float delta)
    {
        float dist = GlobalPosition.DistanceTo(_player.GlobalPosition);

        if (dist > DetectionRange)
        {
            BrakeHorizontal();
            return;
        }

        if (dist <= StopDistance)
        {
            BrakeHorizontal();
            FacePlayer();
            return;
        }

        Vector3 toPlayer = _player.GlobalPosition - GlobalPosition;
        toPlayer.Y = 0f;
        toPlayer   = toPlayer.Normalized();

        Vector3 velocity = Velocity;
        velocity.X = toPlayer.X * MoveSpeed;
        velocity.Z = toPlayer.Z * MoveSpeed;
        Velocity   = velocity;

        FacePlayer();
    }

    private void BrakeHorizontal()
    {
        Vector3 vel = Velocity;
        vel.X  = 0f;
        vel.Z  = 0f;
        Velocity = vel;
    }

    private void FacePlayer()
    {
        if (!IsInstanceValid(_visuals) || !IsInstanceValid(_player))
            return;

        Vector3 lookTarget = new Vector3(
            _player.GlobalPosition.X,
            GlobalPosition.Y,
            _player.GlobalPosition.Z
        );

        if (lookTarget.DistanceTo(GlobalPosition) > 0.01f)
        {
            _visuals.LookAt(lookTarget, Vector3.Up);
            _visuals.RotateY(Mathf.Pi); // flip so the eye faces forward
        }
    }

    private CharacterBody3D FindPlayer()
    {
        var nodes = GetTree().GetNodesInGroup("player");
        if (nodes[0] is CharacterBody3D cb)
            return cb;
        return null;
    }
}
