using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*
 * Evolves a dungeon level
 */
public class Evolution : MonoBehaviour {

    private Generator generator;

    // Set to log fitness during evolution (and debug messages)
    private bool logging = false;

    [Header("Initial Population")]
    public int initialWalls; // Number of walls in initial levels
    public int minWallLength; // Min length of wall
    public int maxWallLength; // Max length of wall

    [Header("Overall Fitness")]
    public float densityWeight; // Weight for density in overall fitness
    public float pathWeight; // Weight for path in overall fitness

    [Header("Density Fitness")]
    // Density is the % tiles which are non-empty (walls)
    public float densityTarget; // Target density (= max fitness)
    public float maxDensityError; // Earn some fitness if error below threshold

    [Header("Path Fitness")]
    // Length of the shortest path from spawn to exit
    public int pathTarget; // Target path length (= max fitness)
    public int maxPathError; // Earn some fitness if error below threshold
    public int noPathPenalty; // Reduction in overall fitness if no path exists

    [Header("Evolution")]
    public int populationSize; // Individual levels per generation
    public int generations; // Number of iterations of EA
    public double mutationRate; // Chance for individual to be mutated

    // New population
    private List<WallSet> population;
    // Old population with associated fitness scores
    private Dictionary<WallSet,int> fitPopulation;
    // Total fitness for old population
    private int generationFitness;

    // The max fitness individual in a generation
    private WallSet bestGenotype;

    // Random number generation
    private System.Random random;
    // For logging fitness to a CSV file (turned off by default)
    private StringBuilder csvLog;

    public void SetGenerator(Generator g)
    {
        generator = g;
    }

    public WallSet GetDungeon()
    {
        return bestGenotype;
    }

    /*
     * Generate a random set of walls to start the evolutionary process
     */
    protected void InitialisePopulation()
    {
        // Initial population
        population = new List<WallSet>();
        fitPopulation = new Dictionary<WallSet, int>();
        for (int i = 0; i < populationSize; i++)
        {
            WallSet dungeon = new WallSet(generator.width, generator.height);
            dungeon.GenerateRandomWalls(initialWalls, minWallLength, maxWallLength, random);
            dungeon.SetSpawn(generator.spawn.x, generator.spawn.y);
            dungeon.SetExit(generator.exit.x, generator.exit.y);
            population.Add(dungeon);
        }
    }

    /*
     * The evolutionary algorithm
     */
    public void Evolve() {

        int generationFitness = 0;
        LogMessage("Evolving...");
        random = new System.Random();
        csvLog = new StringBuilder(); // Used to log fitness values

        InitialisePopulation();
        int maxFit = EvaluatePopulation();
        LogGeneration(0, maxFit, generationFitness);

        /**************************************************
         * 
         * Main Loop: Evolve for N Generations
         */
        for (int gen = 0; gen < generations; gen++)
        {
            // Create the next generation
            population = new List<WallSet>();
            // Continue until we have hit our pop size
            int required = populationSize;
            while (required > 0)
            {
                // Select two individuals to breed
                WallSet parentA = Select();
                WallSet parentB = Select();
                // Create their two children
                List<WallSet> kids = parentA.Crossover(parentB, random);
                // Add one or both to the new population
                AddToPopulation(kids[0]);
                if (required > 1)
                {
                    AddToPopulation(kids[1]);
                }
                required -= 2;
            }

            maxFit = EvaluatePopulation();
            LogGeneration(gen + 1, maxFit, generationFitness);
        }
        /***************************************************
         */

        // Write all data to a CSV file
        if (logging)
        {
            String timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            System.IO.File.WriteAllText(@"fitness" + timeStamp + ".csv", csvLog.ToString());
        }

        if (bestGenotype == null)
        {
            LogMessage("Evolution failed to generate non-zero fitness.");
        }

        LogMessage("Best fitness: " + maxFit);
    }

    /*
     * Calculate fitness for entire population
     * and return the max individual fitness
     */
    protected int EvaluatePopulation()
    {
        // Calculate fitness for entire population
        generationFitness = CalculateFitness();

        // Identify the max fitness individual (for logging)
        int maxFitness = 0;
        foreach (KeyValuePair<WallSet, int> kv in fitPopulation)
        {
            if (kv.Value > maxFitness)
            {
                bestGenotype = kv.Key;
                maxFitness = kv.Value;
            }
        }

        return maxFitness;
    }


    /*
     * Calculate fitness for the population
     */
    protected int CalculateFitness()
    {
        fitPopulation = new Dictionary<WallSet, int>();

        // Calculate fitness for each member of population
        foreach (WallSet geno in population)
        {
            // Generate the phenotype (tile grid)
            TileGrid grid = new TileGrid(geno);
            // Compute and record fitness
            int fitness = Fitness.GetFitness(grid, this);
            fitPopulation.Add(geno, fitness);
        }

        // Calculate total fitness
        int totalFitness = 0;
        foreach (int value in fitPopulation.Values)
        {
            totalFitness += value;
        }
        return totalFitness;
    }

    /*
     * Fitness proportionate selection
     * 
     * Randomly selects from the current population, with an individual's
     * selection chance being proportional to their fitness.
     */
    public WallSet Select()
    {
        if (fitPopulation == null)
        {
            return null;
        }
        else
        {
            // Amount of fitness consumed before we choose
            int fitness = random.Next(0, generationFitness) + 1;
            // The chosen individual
            WallSet selected = null;

            // Iterate over population
            foreach (KeyValuePair<WallSet, int> kv in fitPopulation)
            {
                selected = kv.Key; // Could it be you?
                fitness -= kv.Value; // Consume its fitness
                if (fitness <= 0) break; // Break if nothing left to consume
            }

            return selected;
        }
    }

    /*
     * Add individual to population, with a chance to mutate
     */
    private void AddToPopulation(WallSet g)
    {
        double m = random.NextDouble();

        if (m < mutationRate)
        {
            population.Add(g.Mutate(minWallLength, maxWallLength, random));
        }
        else
        {
            population.Add(g);
        }
    }

    protected void LogMessage(String str)
    {
        if (logging)
        {
            Debug.Log(str);
        }    
    }


    protected void LogGeneration(int generation, int maxFitness, int totalFitness)
    {
        if (logging)
        {
            csvLog.Append(generation);
            csvLog.Append(",");
            csvLog.Append(maxFitness);
            csvLog.Append(",");
            csvLog.Append(totalFitness);
            csvLog.Append("\n");
        }
    }
}
