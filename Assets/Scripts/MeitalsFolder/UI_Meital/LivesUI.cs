using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [Header("Heart Images (Left to Right)")]
    [SerializeField] private Image[] hearts;

    private void OnEnable()
    {
        EventManagement.OnLivesChanged += UpdateHearts;
    }

    private void OnDisable()
    {
        EventManagement.OnLivesChanged -= UpdateHearts;
    }

    private void UpdateHearts(int currentLives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentLives;
        }
    }
}