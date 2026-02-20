using UnityEngine;

public class GasWheelUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("גררי לכאן את האובייקט של תמונת ה-Enter מה-Hierarchy")]
    [SerializeField] private GameObject enterKeyPrompt;

    // משתנה ששומר האם הגלגל כבר סובב
    private bool isWheelActivated = false;

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
        
        // הפנייה לפונקציה החדשה שמטפלת בהפעלת הגלגל
        EventManagement.OnGasWheelActivated += HandleWheelActivated;
        
        // הפנייה לפונקציה החדשה שמטפלת בריסטרט
        EventManagement.RestartGame += ResetController;
    }

    private void OnDisable()
    {
        // ביטול הרשמה
        EventManagement.OnGasWheelEnter -= ShowPrompt;
        EventManagement.OnGasWheelExit -= HidePrompt;
        EventManagement.OnGasWheelActivated -= HandleWheelActivated;
        EventManagement.RestartGame -= ResetController;
    }

    private void ShowPrompt()
    {
        // אם הגלגל כבר הופעל, אנחנו פשוט יוצאים מהפונקציה ולא מציגים את ה-UI
        if (isWheelActivated) return;

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

    private void HandleWheelActivated()
    {
        isWheelActivated = true; // מסמנים שהגלגל הופעל
        HidePrompt(); // מעלימים את התמונה מיד
    }

    private void ResetController()
    {
        isWheelActivated = false; // מאפסים את המצב לקראת המשחק החדש
        HidePrompt(); // מוודאים שהתמונה מוסתרת
    }
}