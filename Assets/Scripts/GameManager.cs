using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Control Variables")]
    [SerializeField] private float maxTime = 60f;
    [SerializeField] private string endSceneName = "End_Scene";
    [SerializeField] private TextMeshProUGUI timerText; 
    private float timeRemaining;
    private bool gameStarted = false;

    void Start()
    {
        GameObject.Find("Maze").GetComponent<MazeGeneration>().MazeGenerated += OnMazeGenerated;
    }
    void OnMazeGenerated()
    {
        gameStarted = true;
        timeRemaining = maxTime;
        GameObject.Find("Player(Clone)").GetComponent<PlayerControl>().PlayerDeath += OnLoss;
        GameObject.Find("Maze").GetComponent<MazeGeneration>().winBox.GetComponent<WinDetection>().GameWon += OnWin;
    }

    void FixedUpdate()
    {
        if (gameStarted)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimer();
            if (timeRemaining <= 0)
            {
                OnLoss();
            }
        }
    }

    private void UpdateTimer()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = $"{seconds / 60:00}:{seconds % 60:00}";
    }
    void OnWin()
    {
        Debug.Log("you win!");
        LoadEndScene(GameResult.Win);
    }

    void LoadEndScene(GameResult result)
    {
        if (!gameStarted)
        {
            return;
        }

        gameStarted = false;
        GameResultState.LastResult = result;
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(endSceneName);
    }

    void OnLoss()
    {
        Debug.Log("You Lost!");
        LoadEndScene(GameResult.Lose);
    }
}
