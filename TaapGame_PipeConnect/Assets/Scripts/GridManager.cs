using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Grid grid;
    public List<GameObject> prefabs;

    public int width = 5;
    public int depth = 5;

    void Start()
    {
        if (grid == null)
            grid = GetComponent<Grid>();

        SpawnGrid();
    }

    void SpawnGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // Y = 0 (important!)
                Vector3Int cellPos = new Vector3Int(x, 0, z);

                Vector3 worldPos = grid.GetCellCenterWorld(cellPos);

                GameObject prefab =
                    prefabs[Random.Range(0, prefabs.Count)];

                Instantiate(prefab, worldPos, Quaternion.identity, transform);
            }
        }
    }
}