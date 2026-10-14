using Godot;
using System.Collections.Generic;
using WeaponData;
using GameGlobals;

public partial class Range : Weapon
{
    [Export] public override string Id { get; set; }

    protected override PlayerControl Player { get; set; }

    public override void _Ready()
    {
        Init(WeaponType.Range);
    }

    protected override void OnPlayerAttack()
    {
        AttackRange();
    }
}