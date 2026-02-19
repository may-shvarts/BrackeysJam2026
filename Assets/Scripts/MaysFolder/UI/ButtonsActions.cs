using System;
using UnityEngine;

public class ButtonsActions : MonoSingleton<ButtonsActions>
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject pauseMenuUI;
    private bool _ableToPause = false;

    private void OnDisable()
    {
        throw new NotImplementedException();
    }

    private void Awake()
    {
        _ableToPause = false;
        EnableMainMenu();
        DisablePauseMenu();
        Time.timeScale = 0f;
    }
    private void EnableMainMenu()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
        }
    }
    private void DisableMainMenu()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(false);
        }
    }
    private void EnablePauseMenu()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
    }
    private void DisablePauseMenu()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }
    
    public void StartGame()
    {
        DisablePauseMenu();
        DisableMainMenu();
        EventManagement.RestartGame?.Invoke();
        _ableToPause = true;
        Time.timeScale = 1f;
    }
    
    public void PauseGame()
    {
        if (!_ableToPause) return;
        _ableToPause = false;
        EnablePauseMenu();
        DisableMainMenu();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        DisablePauseMenu();
        _ableToPause = true;
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        EventManagement.RestartGame?.Invoke();
        DisablePauseMenu();
        _ableToPause = true;
    }
    
    public void ExitGame()
    {
        EventManagement.RestartGame?.Invoke();
        Time.timeScale = 0f;
        DisablePauseMenu();
        EnableMainMenu();
        _ableToPause = false;
    }
    
    public void EndGame()
    {
        QuitGame();
    }
    
    private static void QuitGame()
    {
#if UNITY_EDITOR
// Application.Quit() does not work in the editor
// so we use this instead
        UnityEditor.EditorApplication.isPlaying = false;
#else
// Close the game!
Application.Quit();
#endif
    }
}
