using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 21;
    public int height = 21;
    public Vector3 mazeOrigin = Vector3.zero;
    public Vector3 floorOffset = Vector3.zero;
    public float doorwaySize = 1.2f; // Adjust to make doorways wider or narrower
    public float wallConnectionAdjustment = 0.2f; // Adjust to close gaps in wall connections
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject ceilingPrefab;
    public GameObject leverPrefab;
    private int[,] maze;

    private float scale = 3f;
    private float floorSpacing = 4f; // 25% more spacing for floors
    private float wallSpacing = 2.9f; // 20% less spacing for walls

    void Start()
    {
        GenerateMaze();
        EnsureMazeConnectivity();
        AddPermanentDoors();
        BuildMaze();
        PlaceLevers();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1;
            }
        }
        CarveMaze(1, 1);
    }

    void CarveMaze(int x, int y)
    {
        maze[x, y] = 0;
        List<Vector2Int> directions = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Shuffle(directions);

        foreach (Vector2Int dir in directions)
        {
            int nx = x + dir.x * 2;
            int ny = y + dir.y * 2;
            if (nx > 0 && ny > 0 && nx < width - 1 && ny < height - 1 && maze[nx, ny] == 1)
            {
                maze[x + dir.x, y + dir.y] = 0;
                CarveMaze(nx, ny);
            }
        }
    }

    void EnsureMazeConnectivity()
    {
        bool[,] visited = new bool[width, height];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(1, 1));
        visited[1, 1] = true;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int dir in new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                int nx = current.x + dir.x;
                int ny = current.y + dir.y;

                if (nx > 0 && ny > 0 && nx < width - 1 && ny < height - 1 && maze[nx, ny] == 0 && !visited[nx, ny])
                {
                    visited[nx, ny] = true;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }
    }

    void AddPermanentDoors()
    {
        int[] doorPositions = { 3, height / 2, height - 4 }; // Three fixed door positions
        foreach (int y in doorPositions)
        {
            maze[0, y] = 0;
            maze[1, y] = 0;
        }
    }

    void BuildMaze()
    {
        // Properly tile the floor and ceiling without scaling, covering only the maze area
        for (int x = 0; x < width - 6; x++)  // Limit width to avoid extra row
        {
            for (int y = 0; y < height - 6; y++) // Limit height to avoid extra column
            {
                Vector3 floorPos = mazeOrigin + new Vector3(x * floorSpacing, -0.1f, y * floorSpacing);
                Instantiate(floorPrefab, floorPos, Quaternion.identity);

                Vector3 ceilingPos = mazeOrigin + new Vector3(x * floorSpacing, 4, y * floorSpacing);
                Instantiate(ceilingPrefab, ceilingPos, Quaternion.identity);
            }
        }

        HashSet<Vector3> placedWalls = new HashSet<Vector3>(); // Track placed walls

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = mazeOrigin + new Vector3(x * wallSpacing, 0, y * wallSpacing);

                if (maze[x, y] == 1) // Only place walls if they are not already placed
                {
                    if (!placedWalls.Contains(pos)) 
                    {
                        placedWalls.Add(pos); // Mark this position as occupied

                        GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity);
                        Vector3 positionAdjustment = Vector3.zero;

                        if (IsDoorway(x, y))
                        {
                            // Adjust the width of doorways
                            wall.transform.localScale = new Vector3(doorwaySize, wall.transform.localScale.y, wall.transform.localScale.z);
                        }
                        else
                        {
                            positionAdjustment.x -= wallConnectionAdjustment;
                            positionAdjustment.z -= wallConnectionAdjustment;
                        }

                        // Apply the position adjustment
                        wall.transform.position += positionAdjustment;

                        // Adjust rotation for proper alignment
                        if ((x > 0 && maze[x - 1, y] == 0) || (x < width - 1 && maze[x + 1, y] == 0))
                        {
                            wall.transform.rotation = Quaternion.Euler(0, 90, 0);
                        }
                    }
                }
            }
        }
    }

    // Function to detect if a wall is part of a doorway
    bool IsDoorway(int x, int y)
    {
        return (maze[x, y] == 1 &&
                ((x > 0 && maze[x - 1, y] == 0) ||
                (x < width - 1 && maze[x + 1, y] == 0) ||
                (y > 0 && maze[x, y - 1] == 0) ||
                (y < height - 1 && maze[x, y + 1] == 0)));
    }



    void PlaceLevers()
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (maze[x, y] == 0 && CountWallsAround(x, y) == 3)
                {
                    deadEnds.Add(new Vector2Int(x, y));
                }
            }
        }
        if (deadEnds.Count >= 2)
        {
            Shuffle(deadEnds);
            Instantiate(leverPrefab, mazeOrigin + new Vector3(deadEnds[0].x * scale, 0.5f, deadEnds[0].y * scale), Quaternion.identity);
            Instantiate(leverPrefab, mazeOrigin + new Vector3(deadEnds[1].x * scale, 0.5f, deadEnds[1].y * scale), Quaternion.identity);
        }
    }

    int CountWallsAround(int x, int y)
    {
        int count = 0;
        foreach (Vector2Int dir in new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
        {
            if (maze[x + dir.x, y + dir.y] == 1)
            {
                count++;
            }
        }
        return count;
    }

    void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
