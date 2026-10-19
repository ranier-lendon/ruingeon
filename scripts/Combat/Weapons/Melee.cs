using Godot;
using System.Collections.Generic;
using WeaponData;
using GameGlobals;


public partial class Melee : Weapon
{
    [Export] public override string Id { get; set; }

    protected override PlayerControl Player { get; set; }

    private Area3D _hurtBox;
    protected override List<IDamageable> Targets { get; set; } = new();

    public override void _Ready()
    {
        Init(WeaponType.Melee);

        _hurtBox = GetNode<Area3D>("HurtBox");
        _hurtBox.BodyEntered += OnHurtBoxBodyEntered;
        _hurtBox.BodyExited  += OnHurtBoxBodyExited;
    }

    protected override void OnPlayerAttack()
    {
        AttackMelee();
        
        var tween = CreateTween();
        tween.TweenProperty(this, "rotation_degrees:y", 150, 0.1f);
        tween.TweenProperty(this, "rotation_degrees:y", 0, 0.1f);
    }

    private void OnHurtBoxBodyEntered(Node3D body)
    {
        if (body is IDamageable damageable && !Targets.Contains(damageable))
        {
            Targets.Add(damageable);
        }
    }

    private void OnHurtBoxBodyExited(Node3D body)
    {
        if (body is IDamageable damageable && Targets.Contains(damageable))
        {
            Targets.Remove(damageable);
        }
    }
}