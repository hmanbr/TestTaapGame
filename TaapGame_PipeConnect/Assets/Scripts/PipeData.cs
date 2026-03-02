using static GameEnum;

[System.Serializable]
public struct PipeData
{
    public PipeType type;
    public int rotation;      // 0,1,2,3 (90° steps)
    public bool rotatable;
}
