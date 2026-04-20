using UnityEngine;
using TMPro;
using System;

public class HUDManager : MonoBehaviour
{
    [Header("Control Variables")]
    [SerializeField] private TextMeshProUGUI timerText; 
    [SerializeField] private GameObject[] healthBars; 
    [SerializeField] private GameObject hud; 
    [SerializeField] private GameObject pauseMenu; 

    public void UpdateTimer(float time)
    {
        int seconds = Mathf.CeilToInt(time);
        timerText.text = $"{seconds / 60:00}:{seconds % 60:00}";
    }

    public void DecreaseHealth(int health)
    {
        healthBars[health].SetActive(false);
    }

    public void PauseMenu(bool isOpen)
    {
        pauseMenu.SetActive(isOpen);
        hud.SetActive(!isOpen);
    }
    
}
