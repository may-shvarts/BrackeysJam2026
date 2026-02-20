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
    [SerializeField] private int maxFloor = 4;
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
    
    //Initial states saved for game restart
    private Vector3 _initialPosition;
    private Vector3 _initialCameraPosition;
    private int _initialFloor;
    
    void Start()
    {
        _initialPosition = transform.position;
        if (cameraTarget != null) _initialCameraPosition = cameraTarget.position;
        _initialFloor = currentFloor;
        
        EventManagement.CurrentFloor = currentFloor;
        EventManagement.MaxFloor = maxFloor;
    }
    //Signing to Keys events
    void OnEnable()
    {
        EventManagement.OnFirstCollectedItem += UpdateFirstDoorEnter;
        EventManagement.OnLastCollectedItem += UpdateLastDoorEnter;
        
        EventManagement.RestartGame += ResetElevator;
        verticalInput.Enable();
    }

    void OnDisable()
    {
        EventManagement.OnFirstCollectedItem -= UpdateFirstDoorEnter;
        EventManagement.OnLastCollectedItem -= UpdateLastDoorEnter;
        
        EventManagement.RestartGame -= ResetElevator;
        verticalInput.Disable();
    }

    private void ResetElevator()
    {
        StopAllCoroutines();
        transform.DOKill();
        if (cameraTarget != null) cameraTarget.DOKill();

        transform.position = _initialPosition;
        if (cameraTarget != null) cameraTarget.position = _initialCameraPosition;
        
        currentFloor = _initialFloor;
        EventManagement.CurrentFloor = currentFloor;

        _isPlayerInside = false;
        _isMoving = false;
        _requiresKeyRelease = false;
        _canEnterFirstDoor = false;
        _canEnterLastDoor = false;
        _playerTransform = null;
    }
    
    private void UpdateFirstDoorEnter()
    {
        Debug.Log("changed the flag");
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
        if (inputValue > 0.5f)
        {
            _requiresKeyRelease = true;
            // If on max floor (4) and can enter last door, trigger Game Over instead of moving
            if (currentFloor == maxFloor && _canEnterLastDoor)
            {
                StartCoroutine(MoveElevatorSequence(1));
                return; // Stop here, no elevator movement
            }
            else if (currentFloor < maxFloor)
            {
                StartCoroutine(MoveElevatorSequence(1));
            }
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
            // Update the global spawn point so the player respawns here if they die
            EventManagement.CurrentFloorSpawnPoint = dropoffPosition;
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
        
        Debug.Log(_canEnterFirstDoor);
        //Block door opening / entry on Floor 0 if item not collected
        if (currentFloor == 0 && !_canEnterFirstDoor) return;

        //Block door opening / entry on Max Floor if last item not collected
        if (currentFloor == maxFloor && !_canEnterLastDoor) return;
        
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