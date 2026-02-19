using System;
using UnityEngine;

public class GameOverManagement : MonoSingleton<GameOverManagement>
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winGameUI;

    private void Awake()
    {
        DisableGameOverPanel();
        DisableGameWinPanel();
    }

    private void OnEnable()
    {
        EventManagement.OnPlayerDied += ShowGameOver;
        EventManagement.OnWin += ShowWinPanel;
        EventManagement.RestartGame += HideAllPanels;
    }

    private void OnDisable()
    {
        EventManagement.OnPlayerDied -= ShowGameOver;
        EventManagement.OnWin -= ShowWinPanel;
        EventManagement.RestartGame -= HideAllPanels;
    }

    private void HideAllPanels()
    {
        DisableGameOverPanel();
        DisableGameWinPanel();
    }

    private void ShowGameOver()
    {
        ButtonsActions.Instance.FreezeGameForEndScreen();
        EnableGameOverPanel();
    }
    
    private void ShowWinPanel()
    {
        ButtonsActions.Instance.FreezeGameForEndScreen();
        EnableGameWinPanel();
    }
    //TODO: make a swipe (fade out) after losing or winning using coroutine
    public void DisableGameWinPanel()
    {
        if (winGameUI != null)
        {
            winGameUI.SetActive(false);
        }
    }
    public void EnableGameWinPanel()
    {
        if (winGameUI != null)
        {
            winGameUI.SetActive(true);
        }
    }
    public void DisableGameOverPanel()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }
    public void EnableGameOverPanel()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }
}
