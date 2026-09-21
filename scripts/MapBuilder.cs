/*
    Plan:
    - Iterate through the 2d map from MapSystem
    - If room is not ' ' (empty), then spawn a room
    - '|' and '-' are bridge.
    - if got '-' rotate the bridge 90deg
    - Actual world position formula: (RoomSize * (x-mapSize/2), 0, RoomSize * (y-mapSize/2))
    - Each room is 20x1x20
    - Each room has walls
    - Walls have 3 variations (Wall, OpenDoorWall, CloseDoorWall)

*/

using Godot;

public partial class MapBuilder : Node3D
{
    private static int mapSize = 9;
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