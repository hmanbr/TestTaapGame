using System.Collections.Generic;
using UnityEngine;
using static GameEnum;

public class PipeTile : MonoBehaviour
{
    public PipeType type;

    [SerializeField] private bool rotatable = true;

    // Which directions are open (Up, Right, Down, Left)
    public bool[] open = new bool[4];

    public Vector2Int gridPos; // assigned later

    void Awake()
    {
        SetupConnections();
    }

    public void Rotate()
    {
        if (!rotatable) return; // STOP if locked

        transform.Rotate(0, 90, 0);

        // rotate connection data
        bool temp = open[3];
        open[3] = open[2];
        open[2] = open[1];
        open[1] = open[0];
        open[0] = temp;
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
