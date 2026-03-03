using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelLoader
{
    public static PipeLevelDataSO SelectedLevel;

    public static void LoadLevel(PipeLevelDataSO level)
    {
        SelectedLevel = level;
        SceneManager.LoadScene("Gameplay");
    }
}
