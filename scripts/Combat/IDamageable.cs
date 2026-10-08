using Godot;

public interface IDamageable
{
    float Health { get; }
    void TakeDamage(float amount);
    void Die();
}