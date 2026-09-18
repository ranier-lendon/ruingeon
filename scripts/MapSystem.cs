/*
    Character's Representation:
    s = start
    p = portal
    * = empty
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
    * * * * * * * * * *
    * * * * * * * * * *
    * * * * * * * * * *
    * * * * * * * * * *
    * * * * * * * * * *
    * * * p x t * * * *
    * * * * x * * * * *
    * * * * s x x h * *
    * * * * * * z * * *
    * * * * * * * * * *

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
    private static int size = 9;
    private static int roomCount = 7;
    private static char[,] map = new char[size, size];
    private static List<int[]> baseRooms = new List<int[]>();

    public override void _Ready()
    {
        // Put '*' in every coordinate
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                map[x,y] = '*';
            }
        }

        // Add starter rooms
        int half = size/2;
        map[half, half] = 's';
        Direction randomDir = getRandomDirection(half, half);
        addRoom(new int[]{half, half}, randomDir, getRandomNeighborRoom('s'));
        
        GenerateMap(1);
    }

    public static char[,] GenerateMap(int floor)
    {
        for (int i = 0; i < roomCount; i++)
        {
            int[] randomRoom = getRandomBaseRoom();
            Direction randomDir = getRandomDirection(randomRoom[0], randomRoom[1]);
            char neighbor = getRandomNeighborRoom(map[randomRoom[0], randomRoom[1]]);
            

            if (i == roomCount-1)
            {
                addRoom(randomRoom, randomDir, 'p');

                if (floor == 4)
                {
                    randomRoom = getRandomBaseRoom();
                    randomDir = getRandomDirection(randomRoom[0], randomRoom[1]);
                    addRoom(randomRoom, randomDir, 'b');
                }
                
                break;
            }

            addRoom(randomRoom, randomDir, neighbor);
        }
        
        printMap(floor);

        return map;
    }

    // Prints the 2d map
    // Use it for debugging
    private static void printMap(int floor)
    {
        GD.Print($"Floor {floor}");
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

    // Adds a room and bridge in the direction of the room you pass
    // Returns true if successfully added the room, vice versa
    private static bool addRoom(int[] baseRoom, Direction dir, char room)
    {
        int x = baseRoom[0];
        int y = baseRoom[1];

        char bridge = '|';
        int bx = x;
        int by = y;

        // Checks if there is room for the bridge and the room
        if (x <= 1 || x >= size-2 || y <= 1 || y >= size-2)
        {
            GD.PushError($"addRoom: base room at {x},{y} is too close to the edge");
            return false;
        }

        switch(dir)
        {
            case Direction.up:
                bridge = '|';
                by -= 1;
                y -= 2;
                break;
            case Direction.down:
                bridge = '|';
                by += 1;
                y += 2;
                break;
            case Direction.left:
                bridge = '-';
                bx -= 1;
                x -= 2;
                break;
            case Direction.right:
                bridge = '-';
                bx += 1;
                x += 2;
                break;
            default:
                GD.PushError($"Expected direction to be up, down, left, or right");
                return false;
        }

        map[bx, by] = bridge;
        map[x, y] = room;

        if (room is 'x' or 'e' or 'h' or 'z' or 't')
        {
            baseRooms.Add(new int[]{x, y});
        }

        return true;
    }

    // Accepts one coordinate in the map
    // Then returns a possible direction to add a room base on that coordinate
    private static Direction getRandomDirection(int x, int y)
    {
        var possibleDirections = new List<Direction>();
        Direction randomDirection;
        
        // Check if passed coordinate is valid
        // Checks if coordinate is inside the map
        if (x < 2 || x > size-3 || y < 2 || y > size-3)
        {
            GD.PushError($"getRandomDirection: Expected x and y value to be between 2-{size-3}. Got {x},{y} instead");
            return Direction.none;
        }
        // Checks if coordinate is valid to add a room
        if (map[x, y] is ('*' or 'p' or 'b'))
        {
            GD.PushError($"getRandomDirection: Passed Coordinate {x},{y} is invalid to add a room");
            return Direction.none;
        }

        // Checks every direction if it's empty.
        // If empty then it is a possible place to add a room
        if (map[x+2, y] == '*')
        {
            possibleDirections.Add(Direction.right);
        }
        if (map[x-2, y] == '*')
        {
            possibleDirections.Add(Direction.left);
        }
        if (map[x, y+2] == '*')
        {
            possibleDirections.Add(Direction.down);
        }
        if (map[x, y-2] == '*')
        {
            possibleDirections.Add(Direction.up);
        }

        // Checks if there is possible direction to add a room
        if (possibleDirections.Count == 0)
        {
            GD.PushError($"getRandomDirection: No possible direction to add a room at {x},{y}");
            return Direction.none;
        }
        // Chooses one random direction from the possible directions
        randomDirection = possibleDirections[GD.RandRange(0, possibleDirections.Count-1) ];
        
        return randomDirection;
    }
    
    // Returns a coordinate that is possible to add a room
    private static int[] getRandomBaseRoom()
    {
        int[] randomRoom = new int[2];
        
        while (true)
        {
            if (baseRooms.Count > 0)
            {
                randomRoom = baseRooms[GD.RandRange(0, baseRooms.Count-1)];
            }
            else
            {
                randomRoom = new int[]{size/2, size/2};
            }

            // Checks if randomRoom has a possible direction to add a room
            if (getRandomDirection(randomRoom[0], randomRoom[1]) != Direction.none)
            {
                break;
            }
            else 
            {
                // Remove the room from baseRooms
                baseRooms.Remove(randomRoom);
            }
        }
        
        
        return randomRoom;
    }

    // Returns a room that is possible to be a neighbor room
    private static char getRandomNeighborRoom(char baseRoom)
    {
        /*
            fight = x, e
            chill = h, z, t
        */


        char randomRoom = ' ';
        bool canBeChill = baseRoom is 'x' or 'e';

        // 80% chance to choose fight room
        // 20% chance for chill room
        if (canBeChill)
        {
            float rng = (float)GD.RandRange(0.0f, 1.0f);
            if (rng < 0.8f)
            {
                randomRoom = pickRandomFight();
            }
            else 
            {
                randomRoom = pickRandomChill();
            }
        }
        else
        {
            randomRoom = pickRandomFight();
        }

        return randomRoom;
    }

    // 70% chance to pull normal room ('x')
    // 30% for elite room ('e')
    private static char pickRandomFight()
    {
        float number = (float)GD.RandRange(0.0f, 1.0f);
        char room = ' ';
        if (number < 0.7f)
        {
            room = 'x';
        }
        else{
            room = 'e';
        }
        return room;
    }

    // 33.33% chance for each room to choose
    private static char pickRandomChill()
    {
        float number = (float)GD.RandRange(0.0f, 1.0f);
        char room = ' ';
        if (number < 0.3333f)
        {
            room = 'z';
        }
        else if (number < 0.6667f)
        {
            room = 'h';
        }
        else
        {
            room = 't';
        }

        return room;
    }
}
