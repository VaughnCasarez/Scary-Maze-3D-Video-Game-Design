using UnityEngine;

public class EndSceneResultText : MonoBehaviour
{
    [SerializeField] private GameObject winTextObject;
    [SerializeField] private GameObject loseTextObject;
    [SerializeField] private GameObject hero;
    [SerializeField] private Material winSkybox;
    [SerializeField] private Material loseSkybox;

    void Start()
    {
        Time.timeScale = 1f;

        bool isWin = GameResultState.LastResult == GameResult.Win;
        bool isLose = GameResultState.LastResult == GameResult.Lose;

        if (winTextObject != null)
        {
            winTextObject.SetActive(isWin);
        }

        if (loseTextObject != null)
        {
            loseTextObject.SetActive(isLose);
        }

        if (hero != null)
        {
            hero.SetActive(isWin);
        }

        if (isWin)
        {
            RenderSettings.skybox = winSkybox;
        } else
        {
            RenderSettings.skybox = loseSkybox;
        }
    }
}