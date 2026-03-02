using UnityEngine;

public class GameEnum
{
    public enum PipeType
    {
        Empty,
        Start,
        End,
        Straight,
        Bend90,
        TShape,
        Cross,
        FixedStraight,
        FixedBend90,
        FixedTShape,
    }

    public enum Dir { Up, Right, Down, Left }
}
