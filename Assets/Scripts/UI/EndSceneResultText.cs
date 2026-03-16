using UnityEngine;

public class EndSceneResultText : MonoBehaviour
{
    [SerializeField] private GameObject winTextObject;
    [SerializeField] private GameObject loseTextObject;

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
    }
}