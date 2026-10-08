using Godot;
using Godot.Collections;
using System.Collections.Generic;
using GameGlobals;

public partial class SpawnManager : Node3D
{
    [Export] public string CurrentDungeon;
    [Export] public WaveType WaveType;

    private List<Marker3D> _spawnPoints = new();
    private DungeonSpawnData _spawnData;
    private Array<PackedScene> _mobList;

    public override void _Ready()
    {
        // Load mobs for this dungeon
        string resourcePath = $"res://resources/{CurrentDungeon}MobList.tres";
        _spawnData = GD.Load<DungeonSpawnData>(resourcePath);

        if (_spawnData == null)
        {
            GD.PrintErr($"SpawnManager: Could not load resource at '{resourcePath}'. Check the path and CurrentDungeon value.");
            return;
        }

        _mobList = _spawnData.GetEnemies(WaveType);

        if (_mobList == null || _mobList.Count == 0)
        {
            GD.PrintErr($"SpawnManager: Mob list for WaveType '{WaveType}' is empty or null!");
        }

        foreach (var child in GetChildren())
        {
            // Get all spawn points
            if (child is Marker3D marker)
            {
                _spawnPoints.Add(marker);
            }
        }

        GD.Print($"SpawnManager ready: {_spawnPoints.Count} spawn points, {_mobList?.Count ?? 0} mob types loaded.");
    }

    public List<Enemy> SpawnMobs(int mobCount)
    {
        var spawned = new List<Enemy>();

        if (_spawnPoints.Count == 0)
        {
            GD.PushWarning("No spawn points found for this dungeon!");
            return spawned;
        }

        // Shuffle spawn points so enemies don't stack on the same position
        var shuffledPoints = new List<Marker3D>(_spawnPoints);
        Shuffle(shuffledPoints);
        int spawnIndex = 0;

        for (int i = 0; i < mobCount; i++)
        {
            // If we've used all spawn points, re-shuffle and cycle again
            if (spawnIndex >= shuffledPoints.Count)
            {
                Shuffle(shuffledPoints);
                spawnIndex = 0;
            }

            int randomMobIndex  = GD.RandRange(0, _mobList.Count - 1);
            Marker3D spawnPoint = shuffledPoints[spawnIndex];
            PackedScene mobScene = _mobList[randomMobIndex];
            spawnIndex++;

            Enemy mob = mobScene.Instantiate<Enemy>();
            GetParent().AddChild(mob);
            mob.GlobalPosition = spawnPoint.GlobalPosition;
            spawned.Add(mob);
        }

        return spawned;
    }

    // Fisher-Yates Shuffle
    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = GD.RandRange(0, i);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
