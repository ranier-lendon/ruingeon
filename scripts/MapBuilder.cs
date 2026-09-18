/*
    Builds the map
*/

using Godot;

public partial class MapBuilder : Node3D
{
    private static int size = 9;
    private static char[,] map;
    public override void _Ready()

    // Scenes
    PackedScene normal = GD.Load<PackedScene>("res://scenes/dungeon rooms/bridge.tscn");
    {
        map = MapSystem.GenerateMap(1);
    }


}