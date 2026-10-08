using Godot;
using Godot.Collections;
using GameGlobals;

[GlobalClass]
public partial class DungeonSpawnData : Resource
{
    [Export] public Array<PackedScene> Normal;
    [Export] public Array<PackedScene> Elite;
    [Export] public Array<PackedScene> Boss;

    public Array<PackedScene> GetEnemies(WaveType waveType)
    {
        switch (waveType)
        {
            case WaveType.Normal: return Normal;
            case WaveType.Elite: return Elite;
            case WaveType.Boss: return Boss;
            default:
                GD.PrintErr($"Error: No wave type '{waveType}'!");
                return null;
        }
    }
}