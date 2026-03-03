using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameplayUI : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [SerializeField] private TextMeshProUGUI movesText;

    void OnEnable()
    {
        gridManager.OnWinEvent += HandleWin;
        gridManager.OnLoseEvent += HandleLost;
        gridManager.OnMovesChanged += UpdateMovesText;
    }

    void OnDisable()
    {
        gridManager.OnWinEvent -= HandleWin;
        gridManager.OnLoseEvent -= HandleLost;
        gridManager.OnMovesChanged -= UpdateMovesText;
    }

    void HandleWin()
    {
        winPanel.SetActive(true);
    }

    void HandleLost()
    {
        losePanel.SetActive(true);
    }

    void UpdateMovesText(int remainingMoves)
    {
        movesText.text = "Moves: " + remainingMoves;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}