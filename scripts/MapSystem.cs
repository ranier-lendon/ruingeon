/*
    Character's Representation:
    s = start
    p = portal
    n = empty
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
    n n n n n n n n n n
    n n n n n n n n n n
    n n n n n n n n n n
    n n n n n n n n n n
    n n n n n n n n n n
    n n n p x t n n n n
    n n n n n x n n n n
    n n n n s x x h n n
    n n n n n n z n n n
    n n n n n n n n n n

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
using GameGlobals;

public static class MapSystem
{
    private static int size = 10;
    private static char[,] map = new char[size, size];

    public static char[,] GenerateMap()
    {
        // Add Starter Room in the middle
        map[size / 2, size / 2] = 's';
        


        return map;
    }

    private static void addRoom()
    {
        
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
            return 0;
        }
        // Checks if coordinate is valid to add a room
        if (map[x, y] is ('n' or 'p' or 's' or 'b'))
        {
            GD.PushError($"Passed Coordinate {x},{y} is invalid to add a room");
            return 0;
        }

        // Checks every direction if it's empty.
        // If empty then it is a possible place to add a room
        if (map[x+1, y] == 'n')
        {
            possibleDirections.Add(Direction.right);
        }
        if (map[x-1, y] == 'n')
        {
            possibleDirections.Add(Direction.left);
        }
        if (map[x, y+1] == 'n')
        {
            possibleDirections.Add(Direction.down);
        }
        if (map[x, y-1] == 'n')
        {
            possibleDirections.Add(Direction.up);
        }

        // Checks if there is possible direction to add a room
        if (possibleDirections.Count == 0)
        {
            GD.PushError($"No possible direction to add a room at {x},{y}");
            return 0;
        }
        // Chooses one random direction from the possible directions
        randomDirection = possibleDirections[GD.randi() % possibleDirections.Count];
        
        return randomDirection;
    }
    
    // Returns a coordinate that is possible to add a room
    private static int[] getRandomRoom()
    {
        var possibleRooms = new List<int[]>();

        // Iterate every room then check if the room is possible to add a room
        for (int x = 0; x < size-1; x++)
        {
            for (int y = 0; y < size-1; y++)
            {
                if (map[x, y] is ('n' or 'p' or 's' or 'b'))
                {
                    if (map[x+1, y] != 'n' || 
                        map[x-1, y] != 'n' || 
                        map[x, y+1] != 'n' || 
                        map[x, y-1] != 'n')
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
            return 0;
        }
        // Chooses one random room from the possible rooms
        int[] randomRoom = possibleRooms[GD.randi() % possibleRooms.Count];
        
        return randomRoom;
    }
}
