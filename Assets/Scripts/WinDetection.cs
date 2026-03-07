using UnityEngine;

public class WinDetection : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControl player = other.gameObject.GetComponent<PlayerControl>();
            if (player.hasKey)
            {
                Time.timeScale = 0f;
                Debug.Log("you win!");
            }
        }
    }
}
