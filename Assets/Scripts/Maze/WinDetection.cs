using UnityEngine;

public class WinDetection : MonoBehaviour
{
    [SerializeField] private string endSceneName = "End_Scene";
    private bool hasTriggered;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControl player = other.gameObject.GetComponent<PlayerControl>();
            if (player != null && player.hasKey)
            {
                hasTriggered = true;
                GameResultState.LastResult = GameResult.Win;
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(endSceneName);
                Debug.Log("you win!");
            }
        }
    }
}
