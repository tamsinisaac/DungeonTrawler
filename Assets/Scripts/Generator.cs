using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; //Nicole adding for load level function

/*
 * Responsible for building a Unity level from an evolved TileGrid
 */
public class Generator : MonoBehaviour
{
    [Header("Dungeon")]
    public GameObject parent;
    public Text display;
    public int width;
    public int height;
    public Vector2Int spawn;
    public Vector2Int exit;

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject targetPrefab;

    private WallSet finalGenotype;

    private bool active = false;

    

    public void Generate()
    {
        if (!active)
        {
            // Nicole addition: load the nextlevel
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            active = true;
            EvolveDungeon();
            BuildDungeon();
            active = false;
        }
    }

    private void EvolveDungeon()
    {
        Evolution evolver = GetComponent<Evolution>();
        evolver.SetGenerator(this);
        evolver.Evolve();
        finalGenotype = evolver.GetDungeon();
    }

    private void ClearDungeon()
    {
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void BuildDungeon()
    {
        if (finalGenotype == null)
        {
            Debug.Log("No genotype available");

        }
        else if (parent == null)
        {
            Debug.Log("No parent object specified");
        }
        else
        {
            ClearDungeon();

            // Build the tile grid from the genotype
            TileGrid phenotype = new TileGrid(finalGenotype);
            width = phenotype.width;
            height = phenotype.height;

            BuildFloor();
            BuildWalls(phenotype);
            ShowPath(phenotype);
            UpdateUI(phenotype);
        }
    }

    protected void BuildFloor()
    {
        GameObject floor = Instantiate(floorPrefab);
        floor.transform.position = new Vector3(0, 0, 0);
        floor.transform.localScale = new Vector3(0.1f * (width + 1), 0, 0.1f * (height + 1));
        floor.transform.SetParent(parent.transform);
        Renderer rend = floor.GetComponent<Renderer>();
        rend.material.color = Color.blue;
    }


    protected void BuildWalls(TileGrid grid)
    {

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float xPos = i - (width / 2);
                float yPos = j - (height / 2);

                if (grid.IsWall(i, j))
                {
                    GameObject wall = Instantiate(wallPrefab);
                    wall.transform.position = new Vector3(xPos, 0.5f, yPos);
                    wall.transform.SetParent(parent.transform);

                }
                else if (grid.IsExit(i, j))
                {
                    GameObject goal = Instantiate(targetPrefab);
                    goal.transform.position = new Vector3(xPos, 0.5f, yPos);
                    goal.transform.SetParent(parent.transform);
                    Renderer rend = goal.GetComponent<Renderer>();
                    rend.material.color = Color.red;

                }
                else if (grid.IsSpawn(i, j))
                {
                    GameObject start = Instantiate(targetPrefab);
                    start.transform.position = new Vector3(xPos, 0.5f, yPos);
                    start.transform.SetParent(parent.transform);
                    Renderer rend = start.GetComponent<Renderer>();
                    rend.material.color = Color.yellow;
                }
            }
        }
    }

    /*
     * Visualise the shortest path in the level
     */ 
    protected void ShowPath(TileGrid grid) {

        List<Vector2Int> path = grid.GetPath();

        if (path != null)
        {
            foreach (Vector2Int step in path)
            {
                if (!grid.IsSpawn(step.x, step.y) &&
                    !grid.IsExit(step.x, step.y))
                {
                    float xPos = step.x - (width / 2);
                    float yPos = step.y - (height / 2);

                    GameObject crumb = Instantiate(targetPrefab);
                    crumb.transform.position = new Vector3(xPos, 0.5f, yPos);
                    crumb.transform.SetParent(parent.transform);
                    Renderer rend = crumb.GetComponent<Renderer>();
                    rend.material.color = Color.blue;
                }
            }
        }
    }

    /*
     * Display details of the generated level
     */
    protected void UpdateUI(TileGrid grid)
    {
        Evolution evo = GetComponent<Evolution>();

        StringBuilder text = new StringBuilder();

        float density = grid.Density();
        int densityFitness = Fitness.DensityFitness(grid, evo);

        text.Append("Density: " + density);
        text.Append(" (Fitness: " + densityFitness + ")");
        
        
        List<Vector2Int> path = grid.GetPath();
        if (path == null)
        {
            text.Append("\nNo path (Fitness: 0)");
        }
        else
        {
            int pathFitness = Fitness.PathFitness(grid, evo);
            text.Append("\nPath length: " + path.Count);
            text.Append(" (Fitness: " + pathFitness + ")");
        }

        text.Append("\nTotal fitness: " + Fitness.GetFitness(grid, evo));
        display.text = text.ToString();
    }
}

