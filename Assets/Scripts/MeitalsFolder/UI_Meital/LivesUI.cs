using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [Header("Heart Images (Left to Right)")]
    [SerializeField] private Image[] hearts;

    [Header("Heart Sprites")]
    [SerializeField] private Sprite fullHeartSprite; 
    [SerializeField] private Sprite emptyHeartSprite;
    private void OnEnable()
    {
        EventManagement.OnLivesChanged += UpdateHearts;
        EventManagement.RestartGame += ResetLives;
    }

    private void OnDisable()
    {
        EventManagement.OnLivesChanged -= UpdateHearts;
        EventManagement.RestartGame -= ResetLives;
    }

    private void ResetLives()
    {
        UpdateHearts(PlayerLives.MaxLives);
    }
    private void UpdateHearts(int currentLives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = true; 
            if (i < currentLives)
            {
                hearts[i].sprite = fullHeartSprite;
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite;
            }
        }
    }
}