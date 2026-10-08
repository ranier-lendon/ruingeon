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
using System;
using System.Collections.Generic;
using GameGlobals;

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
    PackedScene wall = GD.Load<PackedScene>("res://scenes/dungeonRooms/wall.tscn");
    PackedScene wallDoor = GD.Load<PackedScene>("res://scenes/dungeonRooms/wall_door.tscn");
    
    public override void _Ready()
    {
        map = MapSystem.GenerateMap(1);

        for (int y=0; y<mapSize; y++)
        {
            for (int x=0; x<mapSize; x++)
            {
                char room = map[x,y];

                if (room == ' ')
                {
                    continue;
                }

                BuildMap(x, y, room);
            }
        }
    }

    public void BuildMap(int x, int y, char room)
    {
        Vector3 position = new Vector3(x * 20 - (mapSize/2) * 20, 0, y * 20 - (mapSize/2) * 20);
        switch(room)
        {
            case 's':
                AddInstance(start, position);
                break;
            case 'x':
                AddInstance(normal, position);
                break;
            case 'p':
                AddInstance(portal, position);
                break;
            case 'z':
                AddInstance(shop, position);
                break;
            case 'h':
                AddInstance(heal, position);
                break;
            case 'b':
                AddInstance(boss, position);
                break;
            case 't':
                AddInstance(treasure, position);
                break;
            case 'e':
                AddInstance(elite, position);
                break;
            case '-':
                AddInstance(bridge, position);
                break;
            case '|':
                AddInstance(bridge, position, 90f);
                break;
        }

        if (room != '|' && room != '-')
        {
            AddWall(x, y, position);
        }
    }

    private void AddWall(int x, int y, Vector3 basePosition)
    {
        /* 
        - Make a function that checks the direction of the bridge and add a wall_door to
        that direction.

        const north = Pos = (0, 3, 9.5), Rot = (0, 0, 0)
        const south = Pos = (0, 3, -9.5), Rot = (0, 0, 0)
        const east = Pos = (9.5, 3, 0), Rot = (0, 90, 0)
        const west = Pos = (-9.5, 3, 0), Rot = (0, 90, 0)
        */

        var transformData = new Dictionary<Direction, Vector3[]>
        {
            { Direction.up, new Vector3[]{new Vector3(0, 3, 9.5f), new Vector3(0, 0, 0)} },
            { Direction.down, new Vector3[]{new Vector3(0, 3, -9.5f), new Vector3(0, 0, 0)} },
            { Direction.left, new Vector3[]{new Vector3(9.5f, 3, 0), new Vector3(0, 90, 0)} },
            { Direction.right, new Vector3[]{new Vector3(-9.5f, 3, 0), new Vector3(0, 90, 0)} }
        };
        Direction[] bridgeDirections = GetAllBridgeRotation(x, y);
        
        if (Array.Exists(bridgeDirections, d => d == Direction.up))
        {
            Vector3 pos = basePosition + transformData[Direction.up][0];
            float rot = transformData[Direction.up][1].Y;
            AddInstance(wallDoor, pos, rot);
        }
        else
        {
            Vector3 pos = basePosition + transformData[Direction.up][0];
            float rot = transformData[Direction.up][1].Y;
            AddInstance(wall, pos, rot);
        }

        if (Array.Exists(bridgeDirections, d => d == Direction.down))
        {
            Vector3 pos = basePosition + transformData[Direction.down][0];
            float rot = transformData[Direction.down][1].Y;
            AddInstance(wallDoor, pos, rot);
        }
        else
        {
            Vector3 pos = basePosition + transformData[Direction.down][0];
            float rot = transformData[Direction.down][1].Y;
            AddInstance(wall, pos, rot);
        }

        if (Array.Exists(bridgeDirections, d => d == Direction.left))
        {
            Vector3 pos = basePosition + transformData[Direction.left][0];
            float rot = transformData[Direction.left][1].Y;
            AddInstance(wallDoor, pos, rot);
        }
        else
        {
            Vector3 pos = basePosition + transformData[Direction.left][0];
            float rot = transformData[Direction.left][1].Y;
            AddInstance(wall, pos, rot);
        }

        if (Array.Exists(bridgeDirections, d => d == Direction.right))
        {
            Vector3 pos = basePosition + transformData[Direction.right][0];
            float rot = transformData[Direction.right][1].Y;
            AddInstance(wallDoor, pos, rot);
        }
        else
        {
            Vector3 pos = basePosition + transformData[Direction.right][0];
            float rot = transformData[Direction.right][1].Y;
            AddInstance(wall, pos, rot);
        }
    }

    private void AddInstance(PackedScene room, Vector3 position, float rotationY = 0f)
    {
        Node3D roomInstance = (Node3D)room.Instantiate();
        roomInstance.Position = position;
        roomInstance.RotationDegrees = new Vector3(0, rotationY, 0);
        AddChild(roomInstance);
    }


    private Direction[] GetAllBridgeRotation(int x, int y)
    {
        List<Direction> dir = new List<Direction>();

        if (y > 0 && map[x, y-1] is '|' or '-')
        {
            dir.Add(Direction.down);
        }

        if (y < mapSize-1 && map[x, y+1] is '|' or '-')
        {
            dir.Add(Direction.up);
        }

        if (x > 0 && map[x-1, y] is '|' or '-')
        {
            dir.Add(Direction.right);
        }

        if (x < mapSize-1 && map[x+1, y] is '|' or '-')
        {
            dir.Add(Direction.left);
        }

        return dir.ToArray();
    }
}