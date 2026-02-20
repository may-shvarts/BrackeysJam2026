using UnityEngine;

public class GasWheelUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("גררי לכאן את האובייקט של תמונת ה-Enter מה-Hierarchy")]
    [SerializeField] private GameObject enterKeyPrompt;

    private void Awake()
    {
        // נוודא שהתמונה מוסתרת כשהמשחק מתחיל
        HidePrompt();
    }

    private void OnEnable()
    {
        // הרשמה לאירועים שיצרת
        EventManagement.OnGasWheelEnter += ShowPrompt;
        EventManagement.OnGasWheelExit += HidePrompt;
        
        // כשהגלגל מופעל, אנחנו רוצים להעלים את התמונה
        EventManagement.OnGasWheelActivated += HidePrompt;
        
        // גם בריסטרט כדאי לוודא שהתמונה נעלמת
        EventManagement.RestartGame += HidePrompt;
    }

    private void OnDisable()
    {
        // ביטול הרשמה
        EventManagement.OnGasWheelEnter -= ShowPrompt;
        EventManagement.OnGasWheelExit -= HidePrompt;
        EventManagement.OnGasWheelActivated -= HidePrompt;
        EventManagement.RestartGame -= HidePrompt;
    }

    private void ShowPrompt()
    {
        if (enterKeyPrompt != null)
        {
            enterKeyPrompt.SetActive(true);
        }
    }

    private void HidePrompt()
    {
        if (enterKeyPrompt != null)
        {
            enterKeyPrompt.SetActive(false);
        }
    }
}