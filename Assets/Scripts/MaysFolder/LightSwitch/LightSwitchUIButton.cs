using UnityEngine;

public class LightSwitchUIButton : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("גרור לכאן את האובייקט של תמונת ה-E מה-Hierarchy")]
    [SerializeField] private GameObject interactPrompt;

    private void Awake()
    {
        // מוודאים שהתמונה מוסתרת בהתחלה
        HidePrompt();
    }

    private void OnEnable()
    {
        // הרשמה לאירועים
        EventManagement.OnLightHoverEnter += ShowPrompt;
        EventManagement.OnLightHoverExit += HidePrompt;
        
        // כשהשחקן לוחץ על E, אנחנו רוצים להעלים את התמונה
        EventManagement.OnLightInteracted += HidePrompt;
    }

    private void OnDisable()
    {
        // ביטול הרשמה
        EventManagement.OnLightHoverEnter -= ShowPrompt;
        EventManagement.OnLightHoverExit -= HidePrompt;
        EventManagement.OnLightInteracted -= HidePrompt;
    }

    private void ShowPrompt()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(true);
        }
    }

    private void HidePrompt()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }
}