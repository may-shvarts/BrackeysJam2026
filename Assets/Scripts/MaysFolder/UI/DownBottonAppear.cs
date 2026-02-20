using System;
using UnityEngine;
using UnityEngine.UI;

public class DownBottonAppear : MonoBehaviour
{
    private bool _playerOnElevator = false;
    private bool _elevatorMoving = false;
    private bool _collectedFirstItem = false;
    private bool _collectedLastItem = false;
    private int _currentFloor = 0;
    private int _maxFloor = 4;

    private String DOWN_BOTTON_TAG = "DownBotton"; 
    private String UP_BOTTON_TAG = "UpBotton"; 
    private Image _uiImage;
    void Start()
    {
        _uiImage = GetComponent<Image>();
        _currentFloor = EventManagement.CurrentFloor;
        UpdateVisibility(); 
    }

    private void OnEnable()
    {
        EventManagement.OnElevatorEnter += HandleElevatorEnter;
        EventManagement.OnElevatorExit += HandleElevatorExit;
        EventManagement.OnElevatorPrepareToMove += HandleElevatorPrepareToMove;
        EventManagement.OnElevatorArrived += HandleElevatorArrived;
        EventManagement.OnFirstCollectedItem += HandleFirstCollectedItem;
        EventManagement.OnLastCollectedItem += HandleLastCollectedItem;
        EventManagement.RestartGame += HandleRestart;
    }

    private void OnDisable()
    {
        EventManagement.OnElevatorEnter -= HandleElevatorEnter;
        EventManagement.OnElevatorExit -= HandleElevatorExit;
        EventManagement.OnElevatorPrepareToMove -= HandleElevatorPrepareToMove;
        EventManagement.OnElevatorArrived -= HandleElevatorArrived;
        EventManagement.OnFirstCollectedItem -= HandleFirstCollectedItem;
        EventManagement.OnLastCollectedItem -= HandleLastCollectedItem;
        EventManagement.RestartGame -= HandleRestart;
    }
    private void HandleFirstCollectedItem()
    {
        _collectedFirstItem = true;
        UpdateVisibility();
    }
    private void HandleLastCollectedItem()
    {
        _collectedLastItem = true;
        UpdateVisibility();
    }
    private void HandleElevatorEnter(int floor)
    {
        _playerOnElevator = true;
        _currentFloor =  floor;
        UpdateVisibility();
    }

    private void HandleElevatorExit(int floor)
    {
        _playerOnElevator = false;
        _currentFloor =  floor;
        UpdateVisibility();
    }

    private void HandleElevatorPrepareToMove(int floor)
    {
        _elevatorMoving = true;
        _currentFloor =  floor;
        UpdateVisibility();
    }

    private void HandleElevatorArrived(int floor)
    {
        _elevatorMoving = false;
        _currentFloor =  floor;
        UpdateVisibility();
    }
    private void UpdateVisibility()
    {
        // 1. אם השחקן לא במעלית או שהמעלית זזה - מסתירים את הכפתורים מיד
        if (!_playerOnElevator || _elevatorMoving)
        {
            HideObject();
            return; // ה-return עוצר את המשך הפונקציה כדי לחסוך בדיקות מיותרות
        }

        // --- 2. לוגיקה עבור כפתור ירידה (למטה) ---
        if (this.gameObject.CompareTag(DOWN_BOTTON_TAG))
        {
            if (_currentFloor > 0)
            {
                ShowObject();
            }
            else
            {
                HideObject(); // בקומה 0 (קרקע) אי אפשר לרדת
            }
            return; 
        }

        // --- 3. לוגיקה עבור כפתור עלייה (למעלה) ---
        if (this.gameObject.CompareTag(UP_BOTTON_TAG))
        {
            // קומה 5 (קומת הבונוס/הסיום) - לעולם לא מראים חץ למעלה
            if (_currentFloor == 5)
            {
                HideObject();
            }
            // קומה 4 (_maxFloor) - מראים רק אם נאסף החפץ האחרון כדי לאפשר עלייה ל-5
            else if (_currentFloor == _maxFloor) 
            {
                if (_collectedLastItem) ShowObject();
                else HideObject();
            }
            // קומת הקרקע (0) - מראים רק אם נאסף החפץ הראשון
            else if (_currentFloor == 0)
            {
                if (_collectedFirstItem) ShowObject();
                else HideObject();
            }
            // קומות ביניים (1, 2, 3) - מראים חץ למעלה תמיד
            else if (_currentFloor > 0 && _currentFloor < _maxFloor)
            {
                ShowObject();
            }
            // מנגנון ביטחון לכל קומה לא צפויה אחרת
            else
            {
                HideObject();
            }
        }
    }
    private void HideObject()
    {
        if (_uiImage != null && _uiImage.enabled)
        {
            _uiImage.enabled = false;
        }
    }

    private void ShowObject()
    {
        if (_uiImage != null && !_uiImage.enabled)
        {
            _uiImage.enabled = true;
        }
    }
    
    private void HandleRestart()
    {
        _playerOnElevator = false;
        _elevatorMoving = false;
        _collectedFirstItem = false;
        _collectedLastItem = false;
        
        _currentFloor = 0; 

        UpdateVisibility();
    }
}