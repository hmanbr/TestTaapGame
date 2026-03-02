using System.Collections.Generic;
using UnityEngine;
using static GameEnum;

public class PipeTile : MonoBehaviour
{
    public PipeType type;

    [SerializeField] private bool rotatable = true;

    // Which directions are open (Up, Right, Down, Left)
    public bool[] open = new bool[4];

    public Vector2Int gridPos; // assigned later when generate grid

    [Header("Water")]
    public bool hasWater = false;
    public Material pipeMaterial;
    public Material pipeWaterMaterial;
    private int pipeMatIndex = -1;

    [SerializeField] private MeshRenderer pipeRenderer = null;


    void Awake()
    {
        SetupConnections();

        var shared = pipeRenderer.sharedMaterials;

        for (int i = 0; i < shared.Length; i++)
        {
            if (shared[i].name.Contains(pipeMaterial.name))
            {
                pipeMatIndex = i;
                break;
            }
        }

        if (pipeMatIndex == -1 && type != PipeType.Empty)
            Debug.LogError("Pipe material not found on " + name);
    }

    public void Rotate()
    {
        if (!rotatable) return; // stop if locked

        transform.Rotate(0, 90, 0);

        // rotate connection data
        bool temp = open[3];
        open[3] = open[2];
        open[2] = open[1];
        open[1] = open[0];
        open[0] = temp;
    }

    public void SetWater(bool state)
    {
        if (pipeRenderer == null || type == PipeType.Empty) return;

        hasWater = state;

        if (pipeRenderer == null || pipeMatIndex < 0) return;

        var mats = pipeRenderer.materials; // runtime instance array

        mats[pipeMatIndex] = state ? pipeWaterMaterial : pipeMaterial;

        pipeRenderer.materials = mats;
    }

    void SetupConnections()
    {
        open = new bool[4];

        switch (type)
        {
            case PipeType.Straight:
                open[(int)Dir.Up] = true;
                open[(int)Dir.Down] = true;
                break;

            case PipeType.Bend90:
                open[(int)Dir.Up] = true;
                open[(int)Dir.Left] = true;
                break;

            case PipeType.TShape:
                open[(int)Dir.Up] = true;
                open[(int)Dir.Left] = true;
                open[(int)Dir.Down] = true;
                break;

            case PipeType.Cross:
                for (int i = 0; i < 4; i++) open[i] = true;
                rotatable = false;
                break;

            case PipeType.Start:
                open[(int)Dir.Up] = true;
                break;

            case PipeType.End:
                open[(int)Dir.Up] = true;
                break;
        }
    }

    public void DebugOpenDirections()
    {
        List<string> dirs = new List<string>();

        for (int i = 0; i < 4; i++)
        {
            if (open[i])
                dirs.Add(((Dir)i).ToString());
        }

        Debug.Log($"{name} open: {string.Join(", ", dirs)}");
    }
}
