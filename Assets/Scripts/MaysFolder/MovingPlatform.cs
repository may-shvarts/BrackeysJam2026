using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    [Tooltip("כיוון התנועה: למשל (1, 0) לימינה, (0, 1) למעלה, (-1, 0) שמאלה")]
    [SerializeField] private Vector2 moveDirection = Vector2.right;
    
    [Tooltip("המרחק שהפלטפורמה תעבור מהנקודה ההתחלתית שלה")]
    [SerializeField] private float moveDistance = 5f;
    
    [Tooltip("מהירות התנועה של הפלטפורמה")]
    [SerializeField] private float moveSpeed = 3f;

    private Vector2 startingPosition;
    private Vector2 targetPosition;
    private bool movingToTarget = true;

    private void Start()
    {
        // שמירת נקודת ההתחלה וחישוב היעד
        startingPosition = transform.position;
        targetPosition = startingPosition + (moveDirection.normalized * moveDistance);
    }

    private void FixedUpdate()
    {
        // קביעת היעד הנוכחי (הלוך או חזור)
        Vector2 currentPos = transform.position;
        Vector2 destination = movingToTarget ? targetPosition : startingPosition;

        // הזזת הפלטפורמה בקצב קבוע לעבר היעד
        transform.position = Vector2.MoveTowards(currentPos, destination, moveSpeed * Time.fixedDeltaTime);

        // בדיקה אם הפלטפורמה הגיעה ליעד, ואם כן - הפיכת הכיוון
        if (Vector2.Distance(currentPos, destination) < 0.01f)
        {
            movingToTarget = !movingToTarget;
        }
    }

    // הפונקציה הזו מציירת קו עזר בעורך של יוניטי (לא ייראה במשחק עצמו)
    private void OnDrawGizmos()
    {
        // כדי שהקו יצויר רק כשלא משחקים וכדי למנוע שגיאות
        if (Application.isPlaying) return; 

        Gizmos.color = Color.green;
        Vector2 start = transform.position;
        Vector2 end = start + (moveDirection.normalized * moveDistance);
        
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(start, 0.2f);
        Gizmos.DrawWireSphere(end, 0.2f);
    }
}