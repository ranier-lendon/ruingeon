using Godot;
using System;
using GameGlobals;

public partial class RoomTrigger : Area3D
{
    [Export] public Node3D Door;
    [Export] public int MaxWave;
    [Export] public SpawnManager SpawnManager;
    [Export] public int MobsPerWave = 3;

    private int _wave = 1;
    private int _aliveEnemies = 0;
    private bool _isActive = false;    // room has been entered
    private bool _isWaveActive = false; // a wave is currently running

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (_isActive) return; // already triggered, ignore re-entries
        _isActive = true;

        GD.Print($"{body.Name} entered the room!");
        DoorControl(DoorState.Close);
        StartWave();
    }

    private void DoorControl(DoorState state)
    {
        Vector3 targetScale = state == DoorState.Close ? 
            new Vector3(1f, 10f, 0.2f) : 
            new Vector3(1f, 1f, 0.2f);
        
        var tween = CreateTween();
        tween.TweenProperty(Door, "scale", targetScale, 1f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
    }

    private void StartWave()
    {
        if (_isWaveActive) return; // prevent spawning while a wave is running
        _isWaveActive = true;

        GD.Print($"Starting wave {_wave}/{MaxWave}...");
        var enemies = SpawnManager.SpawnMobs(MobsPerWave);
        _aliveEnemies = enemies.Count;

        foreach (var enemy in enemies)
        {
            enemy.Died += OnEnemyDied;
        }

        _wave++;
    }

    private void OnEnemyDied()
    {
        _aliveEnemies--;

        if (_aliveEnemies > 0) return;

        // Wave is now cleared
        _isWaveActive = false;

        if (_wave <= MaxWave)
        {
            StartWave();
        }
        else
        {
            GD.Print("All waves cleared! Opening door.");
            DoorControl(DoorState.Open);
        }
    }
}
