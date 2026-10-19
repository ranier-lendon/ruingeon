using Godot;
using System;

public partial class Player : CharacterBody3D
{
    PlayerProfile profile;

    public override void _Ready()
    {
        profile = new PlayerProfile("Adventurer", 1, 0, 0, 0);

        SaveManager.Register(profile);
        SaveManager.Load();
    }
}
