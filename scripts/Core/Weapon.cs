using Godot;

public abstract class Weapon 
{
    public abstract void OnAttack(string Id, IStats stats, float charge = 0f, List<Enemy> enemies = new())
    {
        var Type = WeaponData[Id].Type;
        switch (Type)
        {
            case WeaponType.Range:
                AttackRange(Id, stats, charge);
                break;
            case WeaponType.Melee:
                AttackMelee(Id, stats, enemies);
                break;
            case WeaponType.None:
                GD.PushError($"Weapon is not assigned to Id: {Id}");
                break;
            default:
                GD.PushError($"Invalid weapon type: {Type}");
                break;
        }
    }

    protected virtual void AttackMelee(IStats stats, List<IDamageable> damageable)
    {
        // Constants
        const float penetrationScaling = 100f;
        const float armorScaling = 100f;

        // Player Stats
        float damage = stats.Damage;
        float critChance = stats.CritChance;
        float critDamage = stats.CritDamage;
        float armorPenetration = stats.ArmorPenetration;

        // Armor Penetration reduces Enemy Armor
        float armorMitigation = armorPenetration / (armorPenetration + penetrationScaling);

        foreach (IDamageable enemy in damageable)
        {
            // Enemy Stats
            float armor = enemy is IStats enemyStats ? enemyStats.Defense : 0f;
            
            // Apply Armor Penetration to Enemy Armor
            armor *= (1 - armorMitigation);
            
            // Apply Damage Mitigation to Damage
            float finalDamage = damage * (1 - armor / (armor + armorScaling)); 

            // Check Crit
            if (GD.Randi() < critChance)
                finalDamage *= critDamage; 

            // Apply Damage
            enemy.TakeDamage(finalDamage);
        }
    }

    protected virtual void AttackRange(string Id, IStats stats, float charge)
    {

    }
}