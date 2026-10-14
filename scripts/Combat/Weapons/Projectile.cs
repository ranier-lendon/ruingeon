using Godot;
using System;

public partial class Projectile : Area3D
{
    private float _speed = 20f;
    private float _maxDistance = 100f;
    private float _distanceTraveled = 0f;
    
    private Vector3 _velocity;
    private float _damage;
    private float _critChance;
    private float _critDamage;
    private float _armorPenetration;
    private float _armorScaling = 100f;
    private float _penetrationScaling = 100f;

    private ProjectileManager _projectileManager;

    public void Setup(
        Vector3 position, 
        Vector3 direction, 
        float damage, 
        float critChance, 
        float critDamage, 
        float armorPenetration,
        float speed, 
        float maxDistance
    )
    {
        GlobalPosition = position;
        _velocity = direction.Normalized();
        _damage = damage;
        _critChance = critChance;
        _critDamage = critDamage;
        _armorPenetration = armorPenetration;
        _speed = speed;
        _maxDistance = maxDistance;
        _distanceTraveled = 0f;

        LookAt(GlobalPosition + _velocity);
        Visible = true;

        BodyEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        float moveDistance = _speed * (float)delta;
        GlobalPosition += _velocity * moveDistance;
        _distanceTraveled += moveDistance;

        if (_distanceTraveled >= _maxDistance)
        {
            ReturnToPool();
        }
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is IDamageable damageable)
        {
            float finalDamage = DamageCalculator.CalculateDamage(
                _damage, _armorPenetration, _critChance, _critDamage, damageable);
            damageable.TakeDamage(finalDamage);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        Visible = false;
        ProjectileManager.Instance.ReturnProjectile(this);
    }
}
