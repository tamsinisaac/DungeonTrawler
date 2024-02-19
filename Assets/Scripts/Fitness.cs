using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Calculate the fitness for a dungeon level
 */
public static class Fitness
{
    // Compute fitness (an int) for an individual phenotype (a TileGrid)
    // May use parameters specificed in the inspector via DungeonEvolution.
    public static int GetFitness(TileGrid g, Evolution d)
    {
        int fitness = DensityFitness(g,d) + PathFitness(g,d);
        if (fitness < 0) fitness = 0; // Must be non-negative
        return fitness;
    }

    // Fitness gained from level density
    public static int DensityFitness(TileGrid g, Evolution d)
    {
        // Check if density is contributing to fitness
        if (d.densityWeight > 0)
        {
            // Difference between actual and target values
            float delta = Math.Abs(g.Density() - d.densityTarget);

            if (delta > d.maxDensityError)
            {
                // The difference is greater than the max error, so
                // density does not contribute to fitness
                return 0;
            }
            else
            {
                // Scale fitness by error from target value
                float normFitness = 1f - (delta / d.maxDensityError);
                // Weight the result
                return (int)(d.densityWeight * normFitness);
            }
        }
        else
        {
            // Density weight is 0, so don't bother computing density
            return 0;
        }
    }

    // Fitness gained from path length
    public static int PathFitness(TileGrid g, Evolution d)
    {
        // Check if path length is contributing to fitness
        if (d.pathWeight > 0)
        {
            List<Vector2Int> path = g.GetPath();
            if (path == null)
            {
                // There is no path, so apply the penalty
                return 0 - d.noPathPenalty;

            }
            else
            {
                // Difference between actual and target path lengths
                int delta = Math.Abs(path.Count - d.pathTarget);

                if (delta > d.maxPathError)
                {
                    // The difference is greater than the max error, so
                    // path length does not contribute to fitness
                    return 0;
                }
                else
                {
                    // Scale fitness by error from target value
                    float normFitness = 1f - ((float) delta / (float) d.maxPathError);
                    // Weight the result
                    return (int)(d.pathWeight * normFitness);
                }
            }
        }
        else
        {
            // Path length weight is 0, so don't bother checking path
            return 0;
        }
    }

}
