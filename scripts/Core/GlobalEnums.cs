namespace GameGlobals
{
    public enum Direction
    {
        up,
        down,
        left,
        right,
        none
    }

    public enum DoorState
    {
        Open,
        Close
    }

    public enum WaveType
    {
        Normal,
        Elite,
        Boss
    }

    public enum WeaponType
    {
        Melee,
        Range,
        None
    }

    public enum Buff
    {
        WeaponDamage,           // Increased weapon damage
        HealthRegen,            // Health regeneration
        MoveSpeed,              // Increased movement speed
        DashDistance,           // Increased dash distance
        DashCooldown,           // Reduced dash cooldown
        WeaponRange,            // Increased weapon range
        CooldownReduction,      // Reduced attack cooldown
        MaxHealth,              // Increased maximum health
        CriticalHitChance,      // Increased critical hit chance
        CriticalHitDamage,          // Increased area of effect
        Knockback,   // Increased status effect duration
    }
}