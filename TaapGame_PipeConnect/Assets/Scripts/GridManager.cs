using System;
using System.Collections.Generic;
using UnityEngine;
using static GameEnum;

public class GridManager : MonoBehaviour
{
    public event Action OnWinEvent;
    public event Action OnLoseEvent;
    public event Action<int> OnMovesChanged;

    [Header("Pipe Prefabs")]
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject endPrefab;
    [SerializeField] private GameObject straightPrefab;
    [SerializeField] private GameObject bendPrefab;
    [SerializeField] private GameObject tShapePrefab;
    [SerializeField] private GameObject crossPrefab;
    [SerializeField] private GameObject fixedStraightPrefab;
    [SerializeField] private GameObject fixedBendPrefab;
    [SerializeField] private GameObject fixedTShapePrefab;
    [SerializeField] private GameObject emptyPrefab;

    [Header("Grid Settings")]
    [SerializeField] private Grid unityGrid;                  // Optional Unity Grid
    [SerializeField] private int width = 5;
    [SerializeField] private int depth = 5;

    [Header("Pipe Prefabs")]
    [SerializeField] private List<GameObject> prefabs;

    private PipeTile[,] gridData;

    private PipeLevelDataSO levelDataSO;

    // Direction helpers (Up, Right, Down, Left)
    private Vector2Int[] offsets =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    private bool hasWon = false;
    private bool hasLost = false;
    private int remainingMoves;

    void Start()
    {
        if (unityGrid == null)
            unityGrid = GetComponent<Grid>();

        levelDataSO = LevelLoader.SelectedLevel;

        if (levelDataSO == null)
        {
            Debug.LogError("No level selected!");
            return;
        }

        //SpawnTestWinableGrid();
        SpawnFromLevelData();
        StartWaterFlow();
    }

    void StartWaterFlow()
    {
        PipeTile start = FindPipe(PipeType.Start);
        if (start == null) return;

        PropagateWater(start);
    }

    public void TryRotatePipe(PipeTile pipe)
    {
        if (hasWon || hasLost) return;
        if (remainingMoves <= 0) return;
        if (!pipe.IsRotatable()) return;

        pipe.Rotate();

        remainingMoves--;

        OnMovesChanged?.Invoke(remainingMoves);   // notify UI

        RecalculateWater();
        CheckLoseCondition();
    }

    // Flowing water to neighbouring pipes (BFS Flood Problem)
    void PropagateWater(PipeTile start)
    {
        // STEP 1: Clear previous simulation
        // Reset all pipes so water recalculation is always deterministic
        foreach (var pipe in gridData)
            if (pipe != null)
                pipe.SetWater(false);

        // STEP 2: BFS queue
        // Breadth-First Search so water spread
        var queue = new Queue<PipeTile>();

        // STEP 3: Start from "start_pipe"
        start.SetWater(true);
        queue.Enqueue(start);

        // STEP 4: Fill pipe with water
        // Continue spreading water until no more valid pipes are reachable
        while (queue.Count > 0)
        {
            PipeTile current = queue.Dequeue();

            // STEP 5: Check all 4 directions (Up, Right, Down, Left)
            for (int dir = 0; dir < 4; dir++)
            {
                // If this pipe is closed in this direction, skip
                if (!current.open[dir]) continue;

                // Get neighbor in that direction
                PipeTile neighbor = GetNeighbor(current, dir);

                // Skip outside grid and empty tile
                if (neighbor == null) continue;

                // Skip if water already filled (prevents infinite loops)
                if (neighbor.hasWater) continue;

                // STEP 6: Connection validation
                // Pipes must connect BOTH ways: current -> neighbor AND neighbor -> current
                if (neighbor.open[OppositeDirection(dir)])
                {
                    // Valid connection: fill neighbor with water
                    neighbor.SetWater(true);

                    // Add neighbor to queue so it can continue spreading
                    queue.Enqueue(neighbor);
                }
            }
        }

        // After water fill finishes, check win condition.
        CheckWinCondition();
    }
    void CheckLoseCondition()
    {
        if (remainingMoves <= 0 && !hasWon)
        {
            hasLost = true;
            Debug.Log("OUT OF MOVES - YOU LOSE");

            OnLoseEvent?.Invoke();
        }
    }

    void CheckWinCondition()
    {
        if (hasWon) return;

        bool foundEnd = false;

        foreach (var pipe in gridData)
        {
            if (pipe == null) continue;

            if (pipe.type == PipeType.End)
            {
                foundEnd = true;

                if (!pipe.hasWater)
                    return; // One end not filled = not win
            }
        }

        // No end pipes safety check
        if (!foundEnd)
        {
            Debug.LogWarning("No end pipes in level!");
            return;
        }

        OnWin();
    }

    void OnWin()
    {
        if (hasWon) return; // prevent win spam
        hasWon = true;

        Debug.Log("Puzzle solved! YAAAAAAAAAAY");

        OnWinEvent?.Invoke();
    }

    public void RecalculateWater()
    {
        PipeTile start = FindPipe(PipeType.Start);
        if (start == null) return;

        PropagateWater(start);
    }

    //Helpers
    int OppositeDirection(int dir)
    {
        return (dir + 2) % 4;
    }

    PipeTile GetNeighbor(PipeTile pipe, int dir)
    {
        Vector2Int next = pipe.gridPos + offsets[dir];

        if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= depth)
            return null;

        return gridData[next.x, next.y];
    }

    // Find pipe by type
    PipeTile FindPipe(PipeType type)
    {
        foreach (var pipe in gridData)
        {
            if (pipe != null && pipe.type == type)
                return pipe;
        }
        return null;
    }
    GameObject GetPrefabFromType(PipeType type)
    {
        switch (type)
        {
            case PipeType.Start:
                return startPrefab;
            case PipeType.End:
                return endPrefab;
            case PipeType.Straight:
                return straightPrefab;
            case PipeType.Bend90:
                return bendPrefab;
            case PipeType.TShape:
                return tShapePrefab;
            case PipeType.Cross:
                return crossPrefab;
            case PipeType.FixedStraight:
                return fixedStraightPrefab;
            case PipeType.FixedBend90:
                return fixedBendPrefab;
            case PipeType.FixedTShape:
                return fixedTShapePrefab;
            default:
                return emptyPrefab;
        }
    }

    void SpawnFromLevelData()
    {
        if (levelDataSO == null)
        {
            Debug.LogError("No level assigned!");
            return;
        }
        remainingMoves = levelDataSO.moveLimit;

        width = levelDataSO.column;
        depth = levelDataSO.row;

        gridData = new PipeTile[width, depth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                PipeData data = levelDataSO.Get(x, z);

                GameObject prefab = GetPrefabFromType(data.type);

                Vector3 worldPos = unityGrid != null
                    ? unityGrid.GetCellCenterWorld(new Vector3Int(x, 0, z))
                    : new Vector3(x, 0, z);

                GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, transform);

                PipeTile pipe = obj.GetComponent<PipeTile>();
                pipe.gridPos = new Vector2Int(x, z);

                // Apply rotation
                pipe.ForceRotate(data.rotation);

                // Lock pipe (TEMP DISABLE)
                //pipe.SetRotatable(data.rotatable);

                gridData[x, z] = pipe;
            }
        }
        OnMovesChanged?.Invoke(remainingMoves);
        AlignCameraToMiddle();
    }

    void AlignCameraToMiddle()
    {
        Camera cam = Camera.main;

        if (unityGrid == null)
        {
            Debug.LogWarning("No Grid assigned!");
            return;
        }

        // Get bottom-left cell center
        Vector3 bottomLeft = unityGrid.GetCellCenterWorld(new Vector3Int(0, 0, 0));

        // Get top-right cell center
        Vector3 topRight = unityGrid.GetCellCenterWorld(new Vector3Int(width - 1, 0, depth - 1));

        // True center of grid
        Vector3 center = (bottomLeft + topRight) / 2f;

        Vector3 camPos = cam.transform.position;
        camPos.x = center.x;

        cam.transform.position = camPos;
    }

    void SpawnTestWinableGrid()
    {
        gridData = new PipeTile[width, depth];

        int pathRow = depth / 2; // middle row

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 worldPos;

                if (unityGrid != null)
                {
                    Vector3Int cell = new Vector3Int(x, 0, z);
                    worldPos = unityGrid.GetCellCenterWorld(cell);
                }
                else
                {
                    worldPos = new Vector3(x, 0, z);
                }

                GameObject prefabToSpawn;

                // ===== PATH ROW =====
                if (z == pathRow)
                {
                    if (x == 0)
                        prefabToSpawn = startPrefab;
                    else if (x == width - 1)
                        prefabToSpawn = endPrefab;
                    else
                        prefabToSpawn = straightPrefab;
                }
                else
                {
                    prefabToSpawn = emptyPrefab;
                }

                GameObject obj = Instantiate(prefabToSpawn, worldPos, Quaternion.identity, transform);
                PipeTile pipe = obj.GetComponent<PipeTile>();

                pipe.gridPos = new Vector2Int(x, z);
                gridData[x, z] = pipe;
            }
        }
    }

    void SpawnGrid()
    {
        gridData = new PipeTile[width, depth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 worldPos;

                if (unityGrid != null)
                {
                    Vector3Int cell = new Vector3Int(x, 0, z);
                    worldPos = unityGrid.GetCellCenterWorld(cell);
                }
                else
                {
                    worldPos = new Vector3(x, 0, z);
                }

                GameObject prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Count)];
                GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, transform);

                PipeTile pipe = obj.GetComponent<PipeTile>();
                pipe.gridPos = new Vector2Int(x, z);

                gridData[x, z] = pipe;
            }
        }
    }

}