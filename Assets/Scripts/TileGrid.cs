using System.Collections.Generic;
using UnityEngine;

/*
 * A rectangular grid of tiles representing a dungeon level.
 * 
 */
public class TileGrid
{
    // Grid width and height
    public int width; 
    public int height;
    protected float tileCount;
    // Tile values
    public enum Tile { Empty, Spawn, Goal, Wall};
    // The tile grid
    private Tile[,] tiles;
    // Spawn and exit positions
    public Vector2Int spawn;
    public Vector2Int exit;
    // Path (may be null)
    public List<Vector2Int> path;

    // Create an empty grid
    public TileGrid(int w, int h)
    {
        width = w;
        height = h;
        tiles = new Tile[w,h];
        tileCount = (float)(w * h);
    }

    // Create a tile grid based on a set of walls
    public TileGrid(WallSet g) : this(g.width, g.height)
    {
        foreach (Wall wall in g.GetWalls())
        {
            int x = wall.x;
            int y = wall.y;
            for (int i = 0; i < wall.length; i++)
            {
                if (IsWithinBounds(x,y))
                {
                    tiles[x, y] = Tile.Wall;
                    if (wall.direction == Wall.Direction.Vertical)
                    {
                        y++;
                    }
                    else
                    {
                        x++;
                    }
                }
                else
                {
                    // Wall goes outside dungeon bounds
                    break;
                }
            }
        }

        // Set the start and goal positions
        SetSpawn(g.GetSpawn());
        SetExit(g.GetExit());

        // Compute path, if one exists
        AStarPathfinder aStar = new AStarPathfinder(this);
        path = aStar.FindPath();
    }

    public List<Vector2Int> GetPath()
    {
        return path;
    }

    // Check a position is within the level
    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    // Set the spawn position
    public void SetSpawn(Vector2Int position)
    {
        if (IsWithinBounds(position.x, position.y))
        {
            spawn = position;
            tiles[position.x, position.y] = Tile.Spawn;
        }
    }

    // Set the exit position
    public void SetExit(Vector2Int position)
    {
        if (IsWithinBounds(position.x, position.y))
        {
            exit = position;
            tiles[position.x, position.y] = Tile.Goal;
        }
    }

    // Is there a wall at this position?
    public bool IsWall(int x, int y)
    {
        if (tiles != null && IsWithinBounds(x,y))
        {
            return tiles[x, y] == Tile.Wall;
        }
        else
        {
            return false;
        }
    }

    // Is the spawn at this position?
    public bool IsSpawn(int x, int y)
    {
        if (tiles != null && x >= 0 && x < width && y >= 0 && y < height)
        {
            return tiles[x, y] == Tile.Spawn;
        }
        else
        {
            return false;
        }
    }

    // Is the exit at this position?
    public bool IsExit(int x, int y)
    {
        if (tiles != null && x >= 0 && x < width && y >= 0 && y < height)
        {
            return tiles[x, y] == Tile.Goal;
        }
        else
        {
            return false;
        }
    }

    // Count the total number of this tile type
    public int Count(Tile tile)
    {
        int count = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j] == tile) count++;
            }
        }
        return count;
    }

    // Calculate the percentage of tiles that are walls
    public float Density()
    {
        int walls = Count(Tile.Wall);
        float density = (float)walls / tileCount;
        return density;
    }

}
