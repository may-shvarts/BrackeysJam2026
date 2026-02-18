using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorController : MonoBehaviour
{
    [Header("Elevator Settings")]
    [SerializeField] private float floorHeight     = 10f;
    [SerializeField] private float moveDuration    = 2f;
    [SerializeField] private float doorsWaitDuration = 1.5f;

    [Header("Floor Limits")]
    [Tooltip("The Lowest Floor")]
    [SerializeField] private int minFloor = 0;
    [Tooltip("The Highest Floor")]
    [SerializeField] private int maxFloor = 3;
    [Tooltip("Starting Floor")]
    [SerializeField] private int currentFloor = 0;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTarget;

    [Header("Input Settings")]
    [Tooltip("Define Up or Down Arrows")]
    [SerializeField] private InputAction verticalInput;
    
    [Tooltip("Assign the exact transform points for each floor (Element 0 = Floor 0, etc.)")]
    [SerializeField] private Transform[] floorDropOffPoints;

    //Player Settings
    [SerializeField] private string playerTag = "Player";
    private Transform _playerTransform;

    //Status
    private bool _isPlayerInside = false;
    private bool _isMoving = false;
    private bool _requiresKeyRelease = false;
    
    //TODO: use this in order to let the player move forward with the level
    private bool _canEnterFirstDoor = false;
    private bool _canEnterLastDoor = false;
    
    void Start()
    {
        EventManagement.CurrentFloor = currentFloor;
    }
    //Signing to Keys events
    void OnEnable()
    {
        EventManagement.OnFirstCollectedItem += UpdateFirstDoorEnter;
        EventManagement.OnLastCollectedItem += UpdateLastDoorEnter;
        verticalInput.Enable();
    }

    void OnDisable()
    {
        EventManagement.OnFirstCollectedItem -= UpdateFirstDoorEnter;
        EventManagement.OnLastCollectedItem -= UpdateLastDoorEnter;
        verticalInput.Disable();
    }

    private void UpdateFirstDoorEnter()
    {
        _canEnterFirstDoor = true;
    }
    private void UpdateLastDoorEnter()
    {
        _canEnterLastDoor = true;
    }
    void Update()
    {
        if (!_isPlayerInside || _isMoving) return;

        float inputValue = verticalInput.ReadValue<float>();

        //Require the user to let go of the button (value nears 0) before accepting new input
        if (Mathf.Abs(inputValue) < 0.1f)
        {
            _requiresKeyRelease = false;
        }
        if (_requiresKeyRelease) return;

        //Choosing to go Up or Down in the elevator
        if (inputValue > 0.5f && currentFloor < maxFloor)
        {
            _requiresKeyRelease = true;
            StartCoroutine(MoveElevatorSequence(1));
        }
        else if (inputValue < -0.5f && currentFloor > minFloor)
        {
            _requiresKeyRelease = true;
            StartCoroutine(MoveElevatorSequence(-1));
        }
    }

    private IEnumerator MoveElevatorSequence(int directionMultiplier)
    {
        _isMoving = true;
        EventManagement.OnPlayerFreeze?.Invoke();
        
        Transform playerSnapshot = _playerTransform;

        int departureFloor = currentFloor;
        EventManagement.OnElevatorPrepareToMove?.Invoke(departureFloor);

        yield return new WaitForSeconds(doorsWaitDuration);

        float delta = floorHeight * directionMultiplier;

        Tween elevatorTween = transform.DOMoveY(transform.position.y + delta, moveDuration)
            .SetEase(Ease.InOutSine);

        if (cameraTarget != null)
            cameraTarget.DOMoveY(cameraTarget.position.y + delta, moveDuration)
                .SetEase(Ease.InOutSine);

        yield return elevatorTween.WaitForCompletion(); // wait for DOTween, not a timer
        
        currentFloor += directionMultiplier;
        EventManagement.CurrentFloor = currentFloor;
        
        // We rely purely on the snapshot we took before moving!
        if (playerSnapshot != null)
        {
            Vector3 dropoffPosition = transform.position;
            
            if (floorDropOffPoints != null && currentFloor >= 0 && currentFloor < floorDropOffPoints.Length)
            {
                dropoffPosition = floorDropOffPoints[currentFloor].position;
            }
            
            Rigidbody2D playerRb = playerSnapshot.GetComponentInParent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.position = new Vector2(dropoffPosition.x, dropoffPosition.y);
            }
        }

        EventManagement.OnElevatorArrived?.Invoke(currentFloor);
        
        //Wait for the door to actually open before giving control back
        yield return new WaitForSeconds(doorsWaitDuration); 

        EventManagement.OnPlayerUnfreeze?.Invoke();
        _isMoving = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag) || _isMoving) return;
        
        _isPlayerInside  = true;
        _playerTransform = other.transform;

        EventManagement.CurrentFloor = currentFloor;
        EventManagement.OnElevatorEnter?.Invoke(currentFloor);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        if (_isMoving) return;

        _isPlayerInside  = false;
        _playerTransform = null;
        EventManagement.OnElevatorExit?.Invoke(currentFloor);
    }
}