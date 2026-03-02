using UnityEngine;

[CreateAssetMenu(fileName = "NewPipeLevel", menuName = "PipeGame/Level")]
public class PipeLevelDataSO : ScriptableObject
{
    public int column = 5;
    public int row = 5;

    public PipeRow[] rows;

    public int moveLimit = 20;

    // Get Level Data (Top-down, Left-right 2D array)
    public PipeData Get(int x, int y)
    {
        int flippedY = row - 1 - y; // top row first in inspector
        return rows[flippedY].columns[x];
    }

    // Inspector code for 2D array
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Resize rows
        if (rows == null || rows.Length != row)
        {
            System.Array.Resize(ref rows, row);
        }

        // Resize each row
        for (int y = 0; y < row; y++)
        {
            if (rows[y] == null)
                rows[y] = new PipeRow();

            if (rows[y].columns == null || rows[y].columns.Length != column)
            {
                System.Array.Resize(ref rows[y].columns, column);
            }
        }
    }
#endif
}
