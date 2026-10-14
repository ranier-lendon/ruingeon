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

    public static class WeaponDatabase
    {
        public static Dictionary<string, WeaponInfo> WeaponList = new Dictionary<string, WeaponInfo>
        {
            { 
                "Rusty Sword", new WeaponInfo
                {
                    Name = "Rusty Sword",
                    Damage = 1.0f,
                    AttackSpeed = 1.0f,
                    CritChance = 0.1f,
                    CritDamage = 2.0f,
                    ArmorPenetration = 0.0f,
                    Type = WeaponType.Melee
                }
            },
            { 
                "Rusty Bow", new WeaponInfo
                {
                    Name = "Rusty Bow",
                    Damage = 1.0f,
                    AttackSpeed = 1.0f,
                    CritChance = 0.1f,
                    CritDamage = 2.0f,
                    ArmorPenetration = 0.0f,
                    Type = WeaponType.Range
                }
            }
        };
    }
}