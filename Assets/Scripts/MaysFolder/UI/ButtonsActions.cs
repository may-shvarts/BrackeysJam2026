using System;
using UnityEngine;

public class ButtonsActions : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject pauseMenuUI;
    public void PauseGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        EventManagement.RestartGame?.Invoke();
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }
    
    public void ExitGame()
    {
        //TODO: add the restart game logic
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
        }
    }
    
    public void QuitGame()
    {
        
    }
}
