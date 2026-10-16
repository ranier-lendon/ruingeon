using Godot;
using System;
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
        Player.PlayerAttack += TryAttackMelee;
        Player.PlayerRangeAttack += TryAttackRange;
    }

    private void TryAttackMelee()
    {
        if (_attackTimer > 0f) return;
        _attackTimer = 1f / Player.AttackSpeed;

        OnPlayerAttack();
    }

    private void TryAttackRange(float chargeTimer)
    {
        if (_attackTimer > 0f) return;
        _attackTimer = 1f / Player.AttackSpeed;

        OnPlayerAttack(chargeTimer);
    }

    protected virtual void OnPlayerAttack() {}
    protected virtual void OnPlayerAttack(float chargeTimer) {}
    
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

    protected virtual void AttackRange(float chargeTimer)
    {
        Projectile arrow = ProjectileManager.Instance.GetProjectile();

        float chargePercent = Mathf.Clamp(chargeTimer / ((RangeInfo)Info).MaxChargeTime, 0f, 1f);
        float MaxDistance = Mathf.Lerp(1f, ((RangeInfo)Info).MaxProjectileDistance, chargePercent);
        float spread = Mathf.Lerp(((RangeInfo)Info).MaxSpread, ((RangeInfo)Info).MinSpread, chargePercent);
        float spreadOffset = Mathf.DegToRad((float)GD.RandRange(-spread, spread));
        Vector3 direction = Player.Rotation + new Vector3(0f, 0f, spreadOffset);

        arrow.Setup(
            Player.GlobalPosition + new Vector3(0f, 2f, 0f), 
            direction,  
            Player.Damage, 
            Player.CritChance, 
            Player.CritDamage, 
            Player.ArmorPenetration,
            MaxDistance);
    }
}