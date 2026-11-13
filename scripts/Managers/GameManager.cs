using Godot;
using System;

public partial class GameManager : Node
{
    public PlayerProfile PlayerProfile { get; private set; }
    public PlayerRuntime PlayerRuntime { get; private set; }
    public DungeonRunState DungeonRun { get; private set; }

    public override void _Ready()
    {
        InitializePlayer();

        PlayerRuntime.Died += OnPlayerDied;
    }

    private void InitializePlayer()
    {
        PlayerProfile = new PlayerProfile(
            "Adventurer", 1, 0, 0, 0
        );

        PlayerRuntime = new PlayerRuntime(100, new string[]{"Rusty Sword", null}, null);
    }

    private void OnPlayerDied()
    {
        GD.Print("Game Manager: Player Died");

    }
}