/*
    Builds the map
*/

using Godot;

public partial class MapBuilder : Node3D
{
    private static int size = 9;
    private static char[,] map;

    // Scenes
    PackedScene boss = GD.Load<PackedScene>("res://scenes/dungeonRooms/boss.tscn");
    PackedScene bridge = GD.Load<PackedScene>("res://scenes/dungeonRooms/bridge.tscn");
    PackedScene elite = GD.Load<PackedScene>("res://scenes/dungeonRooms/elite.tscn");
    PackedScene heal = GD.Load<PackedScene>("res://scenes/dungeonRooms/heal.tscn");
    PackedScene normal = GD.Load<PackedScene>("res://scenes/dungeonRooms/normal.tscn");
    PackedScene portal = GD.Load<PackedScene>("res://scenes/dungeonRooms/portal.tscn");
    PackedScene shop = GD.Load<PackedScene>("res://scenes/dungeonRooms/shop.tscn");
    PackedScene treasure = GD.Load<PackedScene>("res://scenes/dungeonRooms/treasure.tscn");
    
    public override void _Ready()
    {
        map = MapSystem.GenerateMap(1);
    }


}