using UnityEngine;
using System;

public class WinDetection : MonoBehaviour
{
    [SerializeField] private string endSceneName = "End_Scene";
    public Action GameWon;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision!");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("hit player");
            PlayerControl player = other.gameObject.GetComponent<PlayerControl>();
            if (player != null && player.hasKey)
            {
                Debug.Log("correct win condition");
                GameWon?.Invoke();
            }
        }
    }
}
