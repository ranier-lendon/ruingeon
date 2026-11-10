using Godot;
using System;
using GameGlobals;
using System.Collections.Generic;

public class PlayerRuntime 
{
    public float CurrentHealth { get; set; }
    public string EquippedWeapon { get; set; }
    public List<Buff> ActiveBuffs { get; set; } = new List<Buff>();
    public string CurrentDungeon { get; set; }
    public int CurrentFloor { get; set; }
}