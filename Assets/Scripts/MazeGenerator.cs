using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 21;
    public int height = 21;
    public Vector3 mazeOrigin = Vector3.zero;
    public Vector3 floorOffset = Vector3.zero;
    public float doorwaySize = 1.2f; 
    public float ScaleX = 1.1f; // Scale for horizontal walls
    public float ScaleZ = 1.1f; // Scale for vertical walls
    public float WallOffsetX = 0.1f; // Moves horizontal walls closer
    public float WallOffsetZ = 0.1f; // Moves vertical walls closer
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject ceilingPrefab;
    public GameObject leverPrefab;
    private int[,] maze;

    private float floorSpacing = 4f; 
    private float wallSpacing = 2.9f; 

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
        int[] doorPositions = { 3, width / 2, width - 4 }; // Adjusted for Z-side

        foreach (int x in doorPositions)
        {
            maze[x, height - 1] = 0; // Move to the positive Z side
            maze[x, height - 2] = 0; // Make the doorway 2 tiles deep
            maze[x + 1, height - 1] = 0; // Make doorway wider (Right side)
        }
    }

    void BuildMaze()
    {
        HashSet<Vector3> placedWalls = new HashSet<Vector3>(); 

        // **Calculate the proper grid size for floor and ceiling**
        int gridWidth = width - 3;  // Keeps the floor in bounds
        int gridHeight = height - 5;

        // **Place floor and ceiling tiles in a perfect square**
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 floorPos = mazeOrigin + new Vector3(x * floorSpacing, -0.1f, y * floorSpacing) + floorOffset;
                Instantiate(floorPrefab, floorPos, Quaternion.identity);

                Vector3 ceilingPos = mazeOrigin + new Vector3(x * floorSpacing, 4, y * floorSpacing);
                Instantiate(ceilingPrefab, ceilingPos, Quaternion.identity);
            }
        }

        // **Wall placement logic (unchanged)**
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 1)
                {
                    Vector3 pos = mazeOrigin + new Vector3(x * wallSpacing, 0, y * wallSpacing);

                    if (!placedWalls.Contains(pos))
                    {
                        placedWalls.Add(pos);
                        GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity);
                        Vector3 scale = wall.transform.localScale;
                        Quaternion rotation = Quaternion.identity;
                        Vector3 positionAdjustment = Vector3.zero;

                        bool hasLeft = x > 0 && maze[x - 1, y] == 1;
                        bool hasRight = x < width - 1 && maze[x + 1, y] == 1;
                        bool hasUp = y < height - 1 && maze[x, y + 1] == 1;
                        bool hasDown = y > 0 && maze[x, y - 1] == 1;

                        // **Adjust scale & offset to eliminate wall gaps**
                        if ((hasUp || hasDown) && (!hasLeft || !hasRight)) 
                        {
                            rotation = Quaternion.Euler(0, 90, 0);
                            scale.z *= ScaleZ; 
                            positionAdjustment.z -= WallOffsetZ;
                        }
                        else if ((hasLeft || hasRight) && (!hasUp || !hasDown))
                        {
                            rotation = Quaternion.Euler(0, 0, 0);
                            scale.x *= ScaleX;
                            positionAdjustment.x += WallOffsetX;
                        }

                        wall.transform.rotation = rotation;
                        wall.transform.localScale = scale;
                        wall.transform.position += positionAdjustment;
                    }
                }
            }
        }
    }

    void PlaceLevers()
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        // Identify dead-end locations
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

        // Ensure at least two levers are placed
        if (deadEnds.Count >= 2)
        {
            Shuffle(deadEnds);

            PlaceLeverAt(deadEnds[0]);
            PlaceLeverAt(deadEnds[1]);
        }
    }

    // 🔹 New function to correctly place and rotate the lever
    void PlaceLeverAt(Vector2Int position)
    {
        Vector3 leverPos = mazeOrigin + new Vector3(position.x * wallSpacing, 1.2f, position.y * wallSpacing);
        Quaternion leverRotation = Quaternion.identity;

        // Determine the closest wall direction
        if (position.x > 0 && maze[position.x - 1, position.y] == 1)
        {
            // Wall is to the left
            leverPos.x -= wallSpacing * 0.8f; // Adjust to touch the wall
            leverRotation = Quaternion.Euler(0, 90, 0); // Rotate to face right
        }
        else if (position.x < width - 1 && maze[position.x + 1, position.y] == 1)
        {
            // Wall is to the right
            leverPos.x += wallSpacing * 0.8f;
            leverRotation = Quaternion.Euler(0, -90, 0); // Rotate to face left
        }
        else if (position.y > 0 && maze[position.x, position.y - 1] == 1)
        {
            // Wall is below
            leverPos.z -= wallSpacing * 0.8f;
            leverRotation = Quaternion.Euler(0, 180, 0); // Rotate to face up
        }
        else if (position.y < height - 1 && maze[position.x, position.y + 1] == 1)
        {
            // Wall is above
            leverPos.z += wallSpacing * 0.8f;
            leverRotation = Quaternion.Euler(0, 0, 0); // Rotate to face down
        }

        // Instantiate the lever with corrected position and rotation
        Instantiate(leverPrefab, leverPos, leverRotation);
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
