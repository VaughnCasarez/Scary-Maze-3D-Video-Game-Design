using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Control Variables")]
    [SerializeField] private float maxTime = 60f;
    [SerializeField] private string endSceneName = "End_Scene";
    private float timeRemaining;
    private bool gameStarted = false;
    private bool paused = false;
    private MazeGeneration maze;
    private HUDManager hud;

    void Start()
    {
        maze = GameObject.Find("Maze").GetComponent<MazeGeneration>();
        hud = GameObject.Find("UI").GetComponent<HUDManager>();
        maze.MazeGenerated += OnMazeGenerated;
        maze.BeginMaze(UnityEngine.Random.Range(0, int.MaxValue));
    }
    void OnMazeGenerated()
    {
        gameStarted = true;
        timeRemaining = maxTime;
        maze = GameObject.Find("Maze").GetComponent<MazeGeneration>();
        maze.player.GetComponent<PlayerControl>().PlayerDeath += OnLoss;
        maze.player.GetComponent<PlayerControl>().PlayerDamage += OnDamage;
        maze.player.GetComponent<PlayerControl>().PausePressed += OnPausePressed;
        maze.player.GetComponent<PlayerControl>().PlayerHeal += OnHeal;
        maze.player.GetComponent<PlayerControl>().BulletGained += OnBulletGain;
        maze.player.GetComponent<PlayerControl>().BulletUsed += OnBulletUsed;
        maze.winBox.GetComponent<WinDetection>().GameWon += OnWin;
        maze.key.GetComponent<Key>().KeyCollected += OnKeyCollect;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if (gameStarted)
        {
            timeRemaining -= Time.deltaTime;
            hud.UpdateTimer(timeRemaining);
            if (timeRemaining <= 0)
            {
                OnLoss();
            }
        }
    }

    public void GainTime(float seconds)
    {
        timeRemaining += seconds;
    }

    public void OnPausePressed()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1f;
        hud.PauseMenu(paused);
        if (paused)
        {
            UnlockCursor();
        } else
        {
            LockCursor();
        }
    }
    void OnKeyCollect()
    {
        hud.UpdateGoal();
    }

    void OnDamage(int health)
    {
        hud.DecreaseHealth(health);
    }
    void OnHeal(int health)
    {
        hud.IncreaseHealth(health);
    }

    void OnBulletGain(int bullet)
    {
        hud.IncreaseBullets(bullet);
    }
    void OnBulletUsed(int bullet)
    {
        hud.DecreaseBullets(bullet);
    }

    void OnWin()
    {
        UnlockCursor();
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
        UnlockCursor();
        LoadEndScene(GameResult.Lose);
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
