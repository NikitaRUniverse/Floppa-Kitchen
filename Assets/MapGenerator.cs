using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject obstaclePrefab;
    public GameObject keyPrefab;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public int gridSize = 20;
    public int walks = 3;
    public int walkLength = 100;   // Length of the Drunkard’s Walk
    public int enemyCount = 5;     // Number of enemies to spawn
    public int keyCount = 6;

    public GameObject[,] grid;
    private HashSet<Vector2Int> floorPositions;

    void Start()
    {
        GenerateMap();
        PlacePlayer();
        PlaceKeys();
        PlaceEnemies();
    }

    void GenerateMap()
    {
        grid = new GameObject[gridSize, gridSize];
        floorPositions = new HashSet<Vector2Int>();
        Vector2Int currentPos = new Vector2Int(gridSize / 2, gridSize / 2);

        // Fill map with obstacles
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = Instantiate(obstaclePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
            }
        }
        for (int w = 0; w < walks; w++)
        {
            // Drunkard’s Walk to create floor tiles
            for (int i = 0; i < walkLength; i++)
            {
                if (!floorPositions.Contains(currentPos))
                {
                    Destroy(grid[currentPos.x, currentPos.y]);
                    grid[currentPos.x, currentPos.y] = Instantiate(floorPrefab, new Vector3(currentPos.x, currentPos.y, 0), Quaternion.identity, transform);
                    floorPositions.Add(currentPos);
                }

                // Move to a random adjacent cell
                Vector2Int direction = GetRandomDirection();
                currentPos += direction;

                currentPos.x = Mathf.Clamp(currentPos.x, 0, gridSize - 1);
                currentPos.y = Mathf.Clamp(currentPos.y, 0, gridSize - 1);
            }
        }

    }

    Vector2Int GetRandomDirection()
    {
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.down;
            case 2: return Vector2Int.left;
            case 3: return Vector2Int.right;
        }
        return Vector2Int.up;
    }

    void PlacePlayer()
    {
        Vector2Int playerStartPosition = new Vector2Int(gridSize / 2, gridSize / 2);
        Instantiate(playerPrefab, new Vector3(playerStartPosition.x, playerStartPosition.y, 0), Quaternion.identity);
    }

    void PlaceKeys()
    {
        List<Vector2Int> keyPositions = GetKeyPositions();

        // Place each key in its designated position
        foreach (Vector2Int pos in keyPositions)
        {
            Instantiate(keyPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, transform);
        }
    }

    List<Vector2Int> GetKeyPositions()
    {
        // Starting position of the player
        Vector2Int playerPosition = new Vector2Int(gridSize / 2, gridSize / 2);

        // Dictionary to store distances from player to each floor position
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // Initialize the starting position
        queue.Enqueue(playerPosition);
        distances[playerPosition] = 0;

        // Perform a breadth-first search to calculate distances from the player
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int currentDistance = distances[current];

            // Explore neighbors
            foreach (Vector2Int direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int neighbor = current + direction;

                // Ensure neighbor is within bounds, is a floor, and hasn’t been visited
                if (neighbor.x >= 0 && neighbor.x < gridSize && neighbor.y >= 0 && neighbor.y < gridSize &&
                    floorPositions.Contains(neighbor) && !distances.ContainsKey(neighbor))
                {
                    distances[neighbor] = currentDistance + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Sort floor positions by distance from the player
        List<KeyValuePair<Vector2Int, int>> sortedFloors = new List<KeyValuePair<Vector2Int, int>>(distances);
        sortedFloors.Sort((a, b) => a.Value.CompareTo(b.Value));

        // Select six keys at increasing distances from the player
        List<Vector2Int> keyPositions = new List<Vector2Int>();

        int interval = sortedFloors.Count / (keyCount + 1);

        for (int i = 1; i <= keyCount; i++)
        {
            int index = i * interval;  // Get progressively farther points
            keyPositions.Add(sortedFloors[index].Key);
        }

        return keyPositions;
    }


    void PlaceEnemies()
    {
        List<Vector2Int> availableFloors = new List<Vector2Int>(floorPositions);
        Vector2Int playerStartPosition = new Vector2Int(gridSize / 2, gridSize / 2);
        availableFloors.Remove(playerStartPosition);

        for (int i = 0; i < enemyCount; i++)
        {
            Vector2Int enemyPosition;

            do
            {
                enemyPosition = availableFloors[Random.Range(0, availableFloors.Count)];
            }
            while (grid[enemyPosition.x, enemyPosition.y].CompareTag("Player") ||
                   grid[enemyPosition.x, enemyPosition.y].CompareTag("Key") ||
                   grid[enemyPosition.x, enemyPosition.y].CompareTag("Enemy"));

            if (grid[enemyPosition.x, enemyPosition.y].CompareTag("Floor"))
            {
                Destroy(grid[enemyPosition.x, enemyPosition.y]);
            }

            // Place enemy on the grid
            GameObject enemy = Instantiate(enemyPrefab, new Vector3(enemyPosition.x, enemyPosition.y, 0), Quaternion.identity, transform);
            grid[enemyPosition.x, enemyPosition.y] = enemy;
            enemy.tag = "Enemy";
        }
    }
}
