using Godot;
using System.Collections.Generic;
using GameGlobals;
using WeaponData;

public abstract partial class Weapon : Node
{
    // Constants
    private const float penetrationScaling = 100f;
    private const float armorScaling = 100f;

    private float _attackTimer = 0f;

    public abstract string Id { get; set; }
    protected WeaponInfo Info;

    protected virtual List<IDamageable> Targets { get; set; }
    protected abstract PlayerControl Player { get; set; }

    public override void _Process(double delta)
    {
        if (_attackTimer > 0)
            _attackTimer -= (float)delta;
    }

    public virtual void Init(WeaponType type)
    {
        Info = WeaponDatabase.WeaponList[Id];

        if (type != Info.Type)
        {
            GD.PushError($"Weapon type {type} does not match weapon type {Info.Type}");
            return;
        }

        Player = GetNode<PlayerControl>("../../..");
        Player.PlayerAttack += TryAttack;
    }

    private void TryAttack()
    {
        if (_attackTimer > 0f) return;
        _attackTimer = 1f / Player.AttackSpeed;
        OnPlayerAttack();
    }

    protected abstract void OnPlayerAttack();
    
    protected virtual void AttackMelee()
    {
        // Player Stats
        float damage = Player.Damage;
        float critChance = Player.CritChance;
        float critDamage = Player.CritDamage;
        float armorPenetration = Player.ArmorPenetration;

        foreach (IDamageable target in Targets)
        {
            float finalDamage = DamageCalculator.CalculateDamage(
                damage, armorPenetration, critChance, critDamage, target);

            target.TakeDamage(finalDamage);
        }
    }

    protected virtual void AttackRange()
    {
        Projectile arrow = ProjectileManager.Instance.GetProjectile();
        

        arrow.Setup(
            Player.GlobalPosition, 
            direction,  
            Player.Damage, 
            Player.CritChance, 
            Player.CritDamage, 
            Player.ArmorPenetration,
            ProjectileSpeed,
            MaxDistance);
    }
}