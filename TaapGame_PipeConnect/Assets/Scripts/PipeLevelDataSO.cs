using UnityEngine;

[CreateAssetMenu(fileName = "NewPipeLevel", menuName = "PipeGame/Level")]
public class PipeLevelDataSO : ScriptableObject
{
    public int width = 5;
    public int height = 5;

    public PipeRow[] rows;

    // Get Level Data (Top-down, Left-right 2D array)
    public PipeData Get(int x, int y)
    {
        int flippedY = height - 1 - y; // top row first in inspector
        return rows[flippedY].columns[x];
    }

    // Inspector code for 2D array
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Resize rows
        if (rows == null || rows.Length != height)
        {
            System.Array.Resize(ref rows, height);
        }

        // Resize each row
        for (int y = 0; y < height; y++)
        {
            if (rows[y] == null)
                rows[y] = new PipeRow();

            if (rows[y].columns == null || rows[y].columns.Length != width)
            {
                System.Array.Resize(ref rows[y].columns, width);
            }
        }
    }
#endif
}
