using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private PipeLevelDataSO levelData;

    public void OnClick()
    {
        LevelLoader.LoadLevel(levelData);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
