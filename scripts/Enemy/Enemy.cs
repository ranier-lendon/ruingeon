using Godot;

// Enemy AI — chases the player directly. Requires player node in group "player".
public partial class Enemy : CharacterBody3D, IGravity, IDamageable
{
    [Signal] public delegate void DiedEventHandler();

    [Export] public float MoveSpeed = 4.0f;
    [Export] public float StopDistance = 1.2f;
    [Export] public float DetectionRange = 20.0f;

    [Export] public float GravityScale { get; set; } = 1.0f;

    [ExportGroup("Combat")]
    [Export] public float Damage { get; set; } = 10f;
    [Export] public float AttackRange { get; set; } = 1.2f;
    [Export] public float AttackCooldown { get; set; } = 1.0f;

    [ExportGroup("Animation")]
    [Export] public string WalkAnimation = "Walk";
    [Export] public string IdleAnimation = "Idle";
    [Export] public string AttackAnimation = "Sword_Attack";
    [Export] public float WalkAnimationSpeed = 1.0f;
    [Export] public float AttackAnimationSpeed = 1.0f;
    [Export] public float AnimationBlendTime = 0.2f;

    public float Health { get; private set; } = 100f;

    private Node3D _visuals;
    private CharacterBody3D _player;
    private AnimationPlayer _animationPlayer;
    private string _currentAnimation = "";
    private bool _isAttacking = false;
    private float _attackCooldownTimer = 0f;

    public override void _Ready()
    {
        _visuals = GetNode<Node3D>("Visuals");
        _player  = FindPlayer();
        _animationPlayer = GetNodeOrNull<AnimationPlayer>("Visuals/Puglin/AnimationPlayer");

        if (_animationPlayer != null)
        {
            _animationPlayer.AnimationFinished += OnAnimationFinished;
        }

        EnsureAnimationLoop(WalkAnimation, loop: true);
        EnsureAnimationLoop(IdleAnimation, loop: true);
        EnsureAnimationLoop(AttackAnimation, loop: false);

        PlayAnimation(IdleAnimation, loop: true);
    }

    public override void _PhysicsProcess(double delta)
    {
        float fDelta = (float)delta;
        GravityComponent.ApplyGravity(this, delta);

        if (_attackCooldownTimer > 0f)
            _attackCooldownTimer -= fDelta;

        if (_isAttacking && (_animationPlayer == null || !_animationPlayer.IsPlaying()))
        {
            _isAttacking = false;
        }

        if (!IsInstanceValid(_player))
            _player = FindPlayer();

        if (_player != null)
            ChasePlayer(fDelta);
        else
            BrakeHorizontal();

        MoveAndSlide();

        UpdateAnimation();
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        GD.Print($"Enemy health: {Health}");
        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        EmitSignal(SignalName.Died);
        QueueFree();
    }

    public void Attack()
    {
        if (_isAttacking) return;

        _isAttacking = true;
        _attackCooldownTimer = AttackCooldown;

        PlayAnimation(AttackAnimation, AttackAnimationSpeed, loop: false);

        if (IsInstanceValid(_player))
        {
            float dist = GlobalPosition.DistanceTo(_player.GlobalPosition);
            if (dist <= AttackRange + 0.3f && _player is IDamageable damageable)
            {
                damageable.TakeDamage(Damage);
            }
        }
    }

    private void ChasePlayer(float delta)
    {
        float dist = GlobalPosition.DistanceTo(_player.GlobalPosition);

        if (_isAttacking)
        {
            BrakeHorizontal();
            FacePlayer();
            return;
        }

        if (dist > DetectionRange)
        {
            BrakeHorizontal();
            return;
        }

        float attackDist = Mathf.Max(StopDistance, AttackRange);
        if (dist <= attackDist)
        {
            BrakeHorizontal();
            FacePlayer();

            if (_attackCooldownTimer <= 0f)
            {
                Attack();
            }
            return;
        }

        Vector3 toPlayer = _player.GlobalPosition - GlobalPosition;
        toPlayer.Y = 0f;
        toPlayer   = toPlayer.Normalized();

        Vector3 velocity = Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, toPlayer.X * MoveSpeed, MoveSpeed * 10f * delta);
        velocity.Z = Mathf.MoveToward(velocity.Z, toPlayer.Z * MoveSpeed, MoveSpeed * 10f * delta);
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

    private void UpdateAnimation()
    {
        if (_animationPlayer == null || _isAttacking) return;

        Vector2 horizontalVel = new Vector2(Velocity.X, Velocity.Z);
        if (horizontalVel.LengthSquared() > 0.05f)
        {
            PlayAnimation(WalkAnimation, WalkAnimationSpeed, loop: true);
        }
        else
        {
            PlayAnimation(IdleAnimation, 1.0f, loop: true);
        }
    }

    private void PlayAnimation(string animName, float customSpeed = 1.0f, bool loop = true)
    {
        if (_animationPlayer == null) return;

        string resolved = ResolveAnimation(animName);
        if (string.IsNullOrEmpty(resolved)) return;

        if (_currentAnimation == resolved && _animationPlayer.IsPlaying() && loop)
            return;

        EnsureAnimationLoop(resolved, loop);
        _currentAnimation = resolved;
        _animationPlayer.Play(resolved, customBlend: AnimationBlendTime, customSpeed: customSpeed);
    }

    private void EnsureAnimationLoop(string animName, bool loop)
    {
        string resolved = ResolveAnimation(animName);
        if (_animationPlayer != null && !string.IsNullOrEmpty(resolved) && _animationPlayer.HasAnimation(resolved))
        {
            var anim = _animationPlayer.GetAnimation(resolved);
            if (anim != null)
            {
                anim.LoopMode = loop ? Animation.LoopModeEnum.Linear : Animation.LoopModeEnum.None;
            }
        }
    }

    private void OnAnimationFinished(StringName animName)
    {
        string resolvedAttack = ResolveAnimation(AttackAnimation);
        if (!string.IsNullOrEmpty(resolvedAttack) && animName == resolvedAttack)
        {
            _isAttacking = false;
        }
    }

    private string ResolveAnimation(string animName)
    {
        if (_animationPlayer == null || string.IsNullOrEmpty(animName))
            return null;

        if (_animationPlayer.HasAnimation(animName))
            return animName;

        string cleanName = animName.Replace("_Loop", "").Replace("_loop", "");

        string[] candidates = new string[]
        {
            animName,
            cleanName,
            $"{cleanName}_Loop",
            $"UAL1_Standard/{animName}",
            $"UAL1_Standard/{cleanName}",
            $"UAL1_Standard/{cleanName}_Loop",
            $"UAL2_Standard/{animName}",
            $"UAL2_Standard/{cleanName}",
            $"UAL2_Standard/{cleanName}_Loop",
        };

        foreach (var c in candidates)
        {
            if (_animationPlayer.HasAnimation(c))
                return c;
        }

        // Case-insensitive fallback search
        var allAnims = _animationPlayer.GetAnimationList();
        foreach (var a in allAnims)
        {
            string s = a.ToString();
            if (string.Equals(s, animName, System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(s, cleanName, System.StringComparison.OrdinalIgnoreCase) ||
                s.EndsWith($"/{cleanName}", System.StringComparison.OrdinalIgnoreCase) ||
                s.EndsWith($"/{animName}", System.StringComparison.OrdinalIgnoreCase))
            {
                return s;
            }
        }

        return null;
    }

    private CharacterBody3D FindPlayer()
    {
        var nodes = GetTree().GetNodesInGroup("player");
        if (nodes.Count > 0 && nodes[0] is CharacterBody3D cb)
            return cb;
        return null;
    }
}
