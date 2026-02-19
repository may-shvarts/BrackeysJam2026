using System;
using UnityEngine;

public class PausingBotton : MonoBehaviour
{
    // Keeps track of whether the game is currently paused
    private bool isPaused = false;

    // This method needs to be public so the UI Button can find it
    public void TogglePause()
    {
        isPaused = !isPaused; // Flip the state

        if (isPaused)
        {
            Time.timeScale = 0f; // Freezes time
            AudioListener.pause = true; // Optional: Mutes all in-game audio
        }
        else
        {
            Time.timeScale = 1f; // Resumes time
            AudioListener.pause = false; // Optional: Resumes audio
        }
    }
}
