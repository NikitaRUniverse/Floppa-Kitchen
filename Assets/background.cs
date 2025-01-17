using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class background : MonoBehaviour
{
    private int gridSize;
    public GameObject[,] grid;
    public GameObject cell;
    public GameObject cameraGM;
    private MapGenerator mapGenerator;

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        gridSize = mapGenerator.gridSize;
        grid = new GameObject[gridSize, gridSize];
        Vector2Int currentPos = new Vector2Int(gridSize / 2, gridSize / 2);

        // Fill map with obstacles
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = Instantiate(cell, new Vector3(x, y, 0), Quaternion.identity, transform);
            }
        }
        transform.position += new Vector3(0, 0, 0.1f);
        cameraGM.transform.position = new Vector3((float)gridSize/2, (float)gridSize/2, -20);
    }
}

