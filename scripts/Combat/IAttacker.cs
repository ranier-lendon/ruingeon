public interface IAttacker
{
    float Damage { get; }
    float AttackRange { get; }
    float AttackCooldown { get; }
    
    void Attack();
}