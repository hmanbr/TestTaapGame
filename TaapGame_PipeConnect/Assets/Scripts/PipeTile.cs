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

        // Specific code to allow for pipe material change without having to compare instances
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
        if (!IsRotatable()) return;

        RotateInternal();
    }

    private void RotateInternal()
    {
        transform.Rotate(0, 90, 0);

        // rotate connection data
        bool temp = open[3];
        open[3] = open[2];
        open[2] = open[1];
        open[1] = open[0];
        open[0] = temp;
    }

    // Rotate pipe for level generator (allow fixed pipe bypass)
    public void ForceRotate(int times) 
    {
        for (int i = 0; i < times; i++)
            RotateInternal();
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
    public bool IsRotatable()
    {
        switch (type)
        {
            case PipeType.FixedStraight:
            case PipeType.FixedBend90:
            case PipeType.FixedTShape:
            case PipeType.Empty:
                return false;
            default:
                return true;
        }
    }

    void SetupConnections()
    {
        open = new bool[4];

        switch (type)
        {
            case PipeType.Straight:
            case PipeType.FixedStraight:
                open[(int)Dir.Up] = true;
                open[(int)Dir.Down] = true;
                break;

            case PipeType.Bend90:
            case PipeType.FixedBend90:
                open[(int)Dir.Up] = true;
                open[(int)Dir.Left] = true;
                break;

            case PipeType.TShape:
            case PipeType.FixedTShape:
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
