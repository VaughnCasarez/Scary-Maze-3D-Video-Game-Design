using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public string game_scene_name;
    public string main_menu_scene_name;
    public GameObject instruction_panel;
    public GameObject main_menu;
    public string win_lose_scene_name;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instruction_panel != null)
        {
            instruction_panel.SetActive(false);
        }
        if (main_menu != null)
        {
            main_menu.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadGameScene()
    {
        Time.timeScale = 1f;
        GameResultState.Clear();
        UnityEngine.SceneManagement.SceneManager.LoadScene(game_scene_name);
    }

    public void LoadInstructionScene()
    {
        ShowInstructionPanel();
    }

    public void ShowInstructionPanel()
    {
        if (instruction_panel != null)
        {
            instruction_panel.SetActive(true);
        }
        if (main_menu != null)
        {
            main_menu.SetActive(false);
        }
    }

    public void HideInstructionPanel()
    {
        if (instruction_panel != null)
        {
            instruction_panel.SetActive(false);
        }
        if (main_menu != null)
        {
            main_menu.SetActive(true);
        }
    }

    public void LoadWinLoseScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(win_lose_scene_name);
    }

    public void LoadMainMenuScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(main_menu_scene_name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
