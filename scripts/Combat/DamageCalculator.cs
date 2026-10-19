using Godot;

public static class DamageCalculator
{
    private const float _penetrationScaling = 100f;
    private const float _armorScaling = 100f;

    public static float CalculateDamage(
        float baseDamage,
        float armorPenetration,
        float critChance,
        float critDamage,
        IDamageable target
    )
    {
        float armor = target is IStats enemyStats ? enemyStats.Armor : 0f;

        float armorMitigation = armorPenetration / (armorPenetration + _penetrationScaling);
        armor *= (1 - armorMitigation);
        float damageMitigation = armor / (armor + _armorScaling);
        float damage = baseDamage * (1 - damageMitigation);

        if (GD.Randf() < critChance)
            damage *= critDamage;

        return damage;
    }
}