using System.Collections.Generic;
using GameGlobals;

namespace WeaponData
{
    public class WeaponInfo
    {
        public string Name { get; set; }
        public float Damage { get; set; }
        public float AttackSpeed { get; set; }
        public float CritChance { get; set; }
        public float CritDamage { get; set; }
        public float ArmorPenetration { get; set; }
        public WeaponType Type { get; set; }
    }

    public class MeleeInfo : WeaponInfo
    {
        public MeleeInfo()
        {
            Type = WeaponType.Melee;
        }
    }

    public class RangeInfo : WeaponInfo
    {
        public float MaxDamage { get; set; }
        public float MaxChargeTime { get; set; }
        public float MaxProjectileDistance { get; set; }
        public float MinSpread { get; set; }
        public float MaxSpread { get; set; }
        public RangeInfo()
        {
            Type = WeaponType.Range;
        }
    }

    public static class WeaponDatabase
    {
        public static Dictionary<string, WeaponInfo> WeaponList = new Dictionary<string, WeaponInfo>
        {
            { 
                "Rusty Sword", new MeleeInfo
                {
                    Name = "Rusty Sword",
                    Damage = 20.0f,
                    AttackSpeed = 1f,
                    CritChance = 0f,
                    CritDamage = 1f,
                    ArmorPenetration = 0.0f,
                }
            },
            { 
                "Rusty Bow", new RangeInfo
                {
                    Name = "Rusty Bow",
                    Damage = 1.0f,
                    MaxDamage = 10.0f,
                    AttackSpeed = 100f,
                    CritChance = 0.1f,
                    CritDamage = 2.0f,
                    ArmorPenetration = 0.0f,
                    MaxChargeTime = 1.0f,
                    MaxProjectileDistance = 120.0f,
                    MinSpread = 1.0f,
                    MaxSpread = 15.0f
                }
            }
        };
    }
}