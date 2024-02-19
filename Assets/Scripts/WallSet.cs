using System.Collections.Generic;
using UnityEngine;

public class WallSet
{
    // Level size
    public int width;
    public int height;

    // The set of walls
    // (technically a bag not a set, as we allow repeated instances)
    protected List<Wall> walls;

    // Start and goal positions
    protected Vector2Int spawn;
    protected Vector2Int exit;

    // Constructor
    public WallSet(int w, int h)
    {
        width = w;
        height = h;
        ClearWalls();
        SetSpawn(0, 0);
        SetExit(width - 1, height - 1);
    }

    // Check a position is within the level
    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    public Vector2Int GetSpawn()
    {
        return spawn;
    }

    public Vector2Int GetExit()
    {
        return exit;
    }

    // Set the start position
    public void SetSpawn(int x, int y)
    {
        if (IsWithinBounds(x,y))
        {
            spawn.x = x;
            spawn.y = y;
        }
    }

    // Set the goal position
    public void SetExit(int x, int y)
    {
        if (IsWithinBounds(x, y))
        {
            exit.x = x;
            exit.y = y;
        }
    }

    /*
     * Accessing/updating the wall set
     */

    // Return the walls
    public List<Wall> GetWalls()
    {
        return walls;
    }

    // Add a new wall
    public void AddWall(int x, int y, int len, Wall.Direction d)
    {
        Wall wall = new Wall(x, y, len, d);
        walls.Add(wall);
    }

    // Remove all walls
    public void ClearWalls()
    {
        walls = new List<Wall>();
    }

    // Add some random walls
    public void GenerateRandomWalls(int n, int min, int max, System.Random r)
    {
        while (n > 0)
        {
            AddRandomWall(min, max, r);
            n--;
        }
    }

    // Add a random wall
    public void AddRandomWall(int min, int max, System.Random r)
    {
        int x = r.Next(0, width);
        int y = r.Next(0, height);
        int len = r.Next(min, max + 1);
        Wall.Direction dir = (Wall.Direction)r.Next(0, 2);
        AddWall(x, y, len, dir);
    }

    public void RemoveRandomWall(System.Random r)
    {
        if (walls.Count > 0)
        {
            int index = r.Next(walls.Count);
            walls.RemoveAt(index);
        }
    }

    // A string summarising this wall set
    public override string ToString()
    {
        string str = "Dungeon ";
        str += (width + "x" + height);
        foreach (Wall w in walls)
        {
            str += " " + w.length;
        }

        return str;
    }

    /*
     * Operators for evolution
     * These crossover or mutate the wall set
     */


    /* 
     * Crossover operator for evolution.
     * Cut the two "parent" levels in half (horizontally or vertically)
     * and swap over the two halves.
     */
    public List<WallSet> Crossover(WallSet other, System.Random r)
    {
        List<WallSet> kids = new List<WallSet>();

        // 0 will cut vertically, 1 cuts horizontally
        bool vertical = (r.Next(0, 2) == 0);
        int cutIndex = r.Next(0, vertical ? width : height);

        WallSet kidA = new WallSet(width, height);
        kidA.SetSpawn(spawn.x, spawn.y);
        kidA.SetExit(exit.x, exit.y);

        WallSet kidB = new WallSet(other.width, other.height);
        kidB.SetSpawn(other.spawn.x, other.spawn.y);
        kidB.SetExit(other.exit.x, other.exit.y);

        // Share this genotype's walls between kids
        foreach (Wall w in walls)
        {
            // Give this wall to kidA or kidB? 
            bool forA = ((vertical && w.x < cutIndex)
                            || (!vertical && w.y < cutIndex));

            if (forA)
            {
                // Wall belongs to kidA
                kidA.AddWall(w.x, w.y, w.length, w.direction);
            }
            else
            {
                // Wall belongs to kidB
                kidB.AddWall(w.x, w.y, w.length, w.direction);
            }
        }

        // Share the other genotype's walls between kids
        foreach (Wall w in other.walls)
        {
            // Give this wall to kidA or kidB? 
            bool forB = ((vertical && w.x < cutIndex)
                            || (!vertical && w.y < cutIndex));

            if (forB)
            {
                // Wall belongs to kidB
                kidB.AddWall(w.x, w.y, w.length, w.direction);
            }
            else
            {
                // Wall belongs to kidA
                kidA.AddWall(w.x, w.y, w.length, w.direction);
            }
        }

        kids.Add(kidA);
        kids.Add(kidB);
        return kids;
    }

    /*
     * Mutation operator for evolution.
     */
    public WallSet Mutate(int min, int max, System.Random r)
    {
        // Copy walls
        WallSet mutation = new WallSet(width, height);
        mutation.SetSpawn(spawn.x, spawn.y);
        mutation.SetExit(exit.x, exit.y);

        foreach (Wall wall in walls)
        {
            mutation.AddWall(wall.x, wall.y, wall.length, wall.direction);
        }

        if (r.Next(11) < 8)
        {
            //Debug.Log("Add wall");
            mutation.AddRandomWall(min, max, r);
        }
        else
        {
            //Debug.Log("Remove wall");
            mutation.RemoveRandomWall(r);
        }

        return mutation;
    }

}
