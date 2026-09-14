/*
    Character's Representation:
    s = start
    p = portal
    - = empty
    x = normal
    h = heal
    z = shop
    e = elite
    b = boss
    t = treasure

    GenerateMap() Goal:
    - Returns a 2d Map of the world
    - Creates a path from start to end
    - Rooms can be spawned 1 direction for each side

    Example:
    
    Map 2d Array:
    - - - - - - - - - -
    - - - - - - - - - -
    - - - - - - - - - -
    - - - - - - - - - -
    - - - - - - - - - -
    - - - p x t - - - -
    - - - - - x - - - -
    - - - - s x x h - -
    - - - - - - z - - -
    - - - - - - - - - -

    Rules:
    - s (start), p (portal), z (shop), h (heal), b (boss), t (treasure) can be only spawn 1 time in the same floor.
    - x (normal) can be spawn min of 3 and maximum of 5 times.
    - b (boss) must be spawned in the 4th floor. It cannot be spawned in 1st, 2nd, and 3rd floor.
    - e (elite) can be spawned twice for each floor.
    - s (start) and p (portal), z (shop), h (heal), b (boss), t (treasure), e (elite) cannot be spawned next to each other in any direction (up, down, left, right).
    - s (start) and p (portal) must have a minimum distance of 4 rooms.
    - 

*/

using Godot;
using System;
using System.Collections.Generic;
using GameGlobals;

public partial class MapSystem : Node3D
{
    private static int size = 10;
    private static char[,] map = new char[size, size];

    public override void _Ready()
    {
        // Put 'n' in every coordinate
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                map[x,y] = 'n';
            }
        }

        // Add starter rooms
        int half = size/2;
        map[half, half] = 's';
        Direction randomDir = getRandomDirection(half, half);
        addRoom(new int[]{half, half}, randomDir, getRandomNeighborRoom('s', 1));
        
        GenerateMap();
    }

    public static char[,] GenerateMap()
    {
        printMap();

        return map;
    }

    // Prints the 2d map
    // Use it for debugging
    private static void printMap()
    {
        for (int y = 0; y < size; y++)
        {
            string row = "";
            for (int x = 0; x < size; x++)
            {
                row += map[x,y] + " ";
            }
            GD.Print(row);
        }
    }

    // Adds a room in the direction of the room you pass
    // Returns true if successfully added the room, vice versa
    private static bool addRoom(int[] baseRoom, Direction dir, char room)
    {
        int x = baseRoom[0];
        int y = baseRoom[1];

        switch(dir)
        {
            case Direction.up:
                map[x, y-1] = room;
                return true;
            case Direction.down:
                map[x, y+1] = room;
                return true;
            case Direction.left:
                map[x-1, y] = room;
                return true;
            case Direction.right:
                map[x+1, y] = room;
                return true;
            default:
                GD.PushError($"Expected direction to be up, down, left, or right");
                return false;
        }
    }

    // Accepts one coordinate in the map
    // Then returns a possible direction to add a room base on that coordinate
    private static Direction getRandomDirection(int x, int y)
    {
        var possibleDirections = new List<Direction>();
        Direction randomDirection;
        
        // Check if passed coordinate is valid
        // Checks if coordinate is inside the map
        if (x < 0 || x >= size-1 || y < 0 || y >= size-1)
        {
            GD.PushError($"Expected x and y value to be between 0-{size-1}");
            //return null;
        }
        // Checks if coordinate is valid to add a room
        if (map[x, y] is ('n' or 'p' or 'b'))
        {
            GD.PushError($"Passed Coordinate {x},{y} is invalid to add a room");
            // return null;
        }

        // Checks every direction if it's empty.
        // If empty then it is a possible place to add a room
        if (map[x+1, y] == '-')
        {
            possibleDirections.Add(Direction.right);
        }
        if (map[x-1, y] == '-')
        {
            possibleDirections.Add(Direction.left);
        }
        if (map[x, y+1] == '-')
        {
            possibleDirections.Add(Direction.down);
        }
        if (map[x, y-1] == '-')
        {
            possibleDirections.Add(Direction.up);
        }

        // Checks if there is possible direction to add a room
        if (possibleDirections.Count == 0)
        {
            GD.PushError($"No possible direction to add a room at {x},{y}");
            // return null;
        }
        // Chooses one random direction from the possible directions
        randomDirection = possibleDirections[GD.RandRange(0, possibleDirections.Count) ];
        
        return randomDirection;
    }
    
    // Returns a coordinate that is possible to add a room
    private static int[] getRandomBaseRoom()
    {
        var possibleRooms = new List<int[]>();

        // Iterate every room then check if the room is possible to add a room
        for (int x = 0; x < size-1; x++)
        {
            for (int y = 0; y < size-1; y++)
            {
                if (map[x, y] is ('-' or 'p' or 's' or 'b'))
                {
                    if (map[x+1, y] != '-' || 
                        map[x-1, y] != '-' || 
                        map[x, y+1] != '-' || 
                        map[x, y-1] != '-')
                    {
                        continue;
                    }
                    possibleRooms.Add(new int[]{x, y});
                }
            }
        }

        // Checks if there is possible room to add a room
        if (possibleRooms.Count == 0)
        {
            GD.PushError($"No possible room to add a room");
            // return null;
        }
        // Chooses one random room from the possible rooms
        int[] randomRoom = possibleRooms[GD.RandRange(0, possibleRooms.Count)];
        
        return randomRoom;
    }

    // Returns a room that is possible to be a neighbor room
    private static char getRandomNeighborRoom(char baseRoom, int floor)
    {
        /*
            s = start
            p = portal
            - = empty
            x = normal
            h = heal
            z = shop
            e = elite
            b = boss
            t = treasure
            fight = x, e
            chill = h, z, t
            start = s
            end = p, b
        */
        var neighborDict = new Dictionary<string, char[]>()
        {
            {"fight", new char[]{'x', 'h', 'z', 't', 'e', 'b', 'p'}},
            {"chill", new char[]{'x', 'e', 'b'}}
        };
        char randomRoom = ' ';

        if (baseRoom is 'x' or 'e')
        {
            var possibleRooms = neighborDict["fight"];
            randomRoom = possibleRooms[GD.RandRange(0, possibleRooms.Length)];
        }
        else if (baseRoom is 'h' or 'z' or 't')
        {
            var possibleRooms = neighborDict["chill"];
            randomRoom = possibleRooms[GD.RandRange(0, possibleRooms.Length)];
        }
        else if (baseRoom is 's')
        {
            randomRoom = 'x';
        }

        return randomRoom;
    }
}
