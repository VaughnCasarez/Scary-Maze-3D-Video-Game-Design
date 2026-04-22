using UnityEngine;
using TMPro;
using System;

public class HUDManager : MonoBehaviour
{
    [Header("Control Variables")]
    [SerializeField] private TextMeshProUGUI timerText; 
    [SerializeField] private GameObject[] healthBars; 
    [SerializeField] private GameObject[] bulletIcons; 
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
    public void IncreaseHealth(int health)
    {
        healthBars[health].SetActive(true);
    }

    public void DecreaseBullets(int bullets)
    {
        bulletIcons[bullets].SetActive(false);
    }
    public void IncreaseBullets(int bullets)
    {
        bulletIcons[bullets].SetActive(true);
    }

    public void PauseMenu(bool isOpen)
    {
        pauseMenu.SetActive(isOpen);
        hud.SetActive(!isOpen);
    }
    
}
