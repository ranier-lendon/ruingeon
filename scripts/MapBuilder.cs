/*
    Plan:
    - Iterate through the 2d map from MapSystem
    - If room is not ' ' (empty), then spawn a room
    - '|' and '-' are bridge.
    - if got '|' rotate the bridge 90deg
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
    PackedScene start = GD.Load<PackedScene>("res://scenes/dungeonRooms/start.tscn");
    PackedScene treasure = GD.Load<PackedScene>("res://scenes/dungeonRooms/treasure.tscn");
    
    public override void _Ready()
    {
        map = MapSystem.GenerateMap(1);
        BuildMap();
    }

    public void BuildMap()
    {
        for (int y=0; y<mapSize; y++)
        {
            for (int x=0; x<mapSize; x++)
            {
                char room = map[x,y];
                if (room == ' ')
                {
                    continue;
                }
                Vector3 position = new Vector3(x * 20 - (mapSize/2) * 20, 0, y * 20 - (mapSize/2) * 20);
                switch(room)
                {
                    case 's':
                        AddRoom(start, position);
                        break;
                    case 'x':
                        AddRoom(normal, position);
                        break;
                    case 'p':
                        AddRoom(portal, position);
                        break;
                    case 'z':
                        AddRoom(shop, position);
                        break;
                    case 'h':
                        AddRoom(heal, position);
                        break;
                    case 'b':
                        AddRoom(boss, position);
                        break;
                    case 't':
                        AddRoom(treasure, position);
                        break;
                    case 'e':
                        AddRoom(elite, position);
                        break;
                    case '-':
                        AddRoom(bridge, position);
                        break;
                    case '|':
                        AddRoom(bridge, position, Mathf.Pi / 2);
                        break;
                }
            }
        }
    }

    private void AddRoom(PackedScene room, Vector3 position, float rotationY = 0f)
    {
        Node3D roomInstance = (Node3D)room.Instantiate();
        roomInstance.Position = position;
        roomInstance.RotationDegrees = new Vector3(0, Mathf.RadToDeg(rotationY), 0);
        AddChild(roomInstance);
    }
}