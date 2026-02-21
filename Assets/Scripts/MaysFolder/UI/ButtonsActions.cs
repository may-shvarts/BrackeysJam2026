using System;
using UnityEngine;

public class ButtonsActions : MonoSingleton<ButtonsActions>
{
    [SerializeField] private GameObject firstStartScreenUI; 
    [SerializeField] private GameObject secondStartScreenUI;
    [SerializeField] private GameObject pauseMenuUI;
    private bool _ableToPause = false;

    private void Awake()
    {
        _ableToPause = false;
        EnableFirstScreen();
        DisablePauseMenu();
        Time.timeScale = 0f;
        EventManagement.OnMainMenuActive?.Invoke();
    }
    private void EnableFirstScreen()
    {
        if (firstStartScreenUI != null) firstStartScreenUI.SetActive(true);
        if (secondStartScreenUI != null) secondStartScreenUI.SetActive(false);
    }

    public void GoToSecondScreen()
    {
        if (firstStartScreenUI != null) firstStartScreenUI.SetActive(false);
        if (secondStartScreenUI != null) secondStartScreenUI.SetActive(true);
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
        if (secondStartScreenUI != null) secondStartScreenUI.SetActive(false);
        DisablePauseMenu();
        
        EventManagement.RestartGame?.Invoke();
        EventManagement.OnGameplayStarted?.Invoke();
        _ableToPause = true;
        Time.timeScale = 1f;
    }
    
    public void PauseGame()
    {
        if (!_ableToPause) return;
        _ableToPause = false;
        EnablePauseMenu();
        Time.timeScale = 0f;
        EventManagement.OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        DisablePauseMenu();
        _ableToPause = true;
        Time.timeScale = 1f;
        
        EventManagement.OnGameResumed?.Invoke();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        EventManagement.RestartGame?.Invoke();
        EventManagement.OnGameplayStarted?.Invoke();
        DisablePauseMenu();
        _ableToPause = true;
    }
    
    public void ExitGame()
    {
        EventManagement.RestartGame?.Invoke();
        EventManagement.OnMainMenuActive?.Invoke();
        Time.timeScale = 0f;
        DisablePauseMenu();
        EnableFirstScreen();
        _ableToPause = false;
    }
    
    public void FreezeGameForEndScreen()
    {
        Time.timeScale = 0f;
        DisablePauseMenu();
        if (firstStartScreenUI != null) firstStartScreenUI.SetActive(false);
        if (secondStartScreenUI != null) secondStartScreenUI.SetActive(false);
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
