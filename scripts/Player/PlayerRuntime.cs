using Godot;
using System;
using GameGlobals;
using System.Collections.Generic;

public partial class PlayerRuntime : RefCounted 
{
    [Signal] public delegate void DiedEventHandler();

    public float CurrentHealth { get; set; }
    public string[] Weapons { get; set; } = new string[2];
    public string EquippedWeapon { get; set; }
    public List<Buff> ActiveBuffs { get; set; }

    public PlayerRuntime(
        float health, 
        string[] weapons,
        List<Buff> buffs
        )
    {
        CurrentHealth = health;
        Weapons = weapons;
        EquippedWeapon = Weapons[0];
        ActiveBuffs = buffs ?? new List<Buff>();
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            EmitSignal(SignalName.Died);
        }
    }

    public void SwapWeapon()
    {
        if (EquippedWeapon == Weapons[0])
            EquippedWeapon = Weapons[1];
        else
            EquippedWeapon = Weapons[0];
    }

    public void AddBuff(Buff buff)
    {
        ActiveBuffs.Add(buff);
    }
}