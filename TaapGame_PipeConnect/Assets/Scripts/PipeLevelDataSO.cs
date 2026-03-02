using UnityEngine;

[CreateAssetMenu(fileName = "NewPipeLevel", menuName = "PipeGame/Level")]
public class PipeLevelDataSO : ScriptableObject
{
    public int width = 5;
    public int height = 5;

    public PipeData[] pipes; // size = width * height

    public PipeData Get(int x, int y)
    {
        return pipes[y * width + x];
    }
}
