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
    [Tooltip("הקומה הכי נמוכה שהמעלית יכולה להגיע אליה")]
    [SerializeField] private int minFloor = 0;
    [Tooltip("הקומה הכי גבוהה שהמעלית יכולה להגיע אליה")]
    [SerializeField] private int maxFloor = 3;
    [Tooltip("הקומה בה המעלית מתחילה")]
    [SerializeField] private int currentFloor = 0;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTarget;

    [Header("Input Settings")]
    [Tooltip("הגדר כאן Action מסוג 1D Axis עבור החצים למעלה/למטה")]
    [SerializeField] private InputAction verticalInput;

    [SerializeField] private string playerTag = "Player";

    private Transform    playerTransform;
    private Rigidbody2D  playerRigidbody;

    private bool isPlayerInside = false;
    private bool isMoving       = false;
    
    // FIX: Variable to force the player to let go of the key before pressing again
    private bool requiresKeyRelease = false; 

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    void Start()
    {
        EventManagement.CurrentFloor = currentFloor;
    }

    void OnEnable()  => verticalInput.Enable();
    void OnDisable() => verticalInput.Disable();

    void Update()
    {
        if (!isPlayerInside || isMoving) return;

        float inputValue = verticalInput.ReadValue<float>();

        // FIX: Require the user to let go of the button (value nears 0) before accepting new input
        if (Mathf.Abs(inputValue) < 0.1f)
        {
            requiresKeyRelease = false;
        }

        if (requiresKeyRelease) return;

        if (inputValue > 0.5f && currentFloor < maxFloor)
        {
            requiresKeyRelease = true;
            StartCoroutine(MoveElevatorSequence(1));
        }
        else if (inputValue < -0.5f && currentFloor > minFloor)
        {
            requiresKeyRelease = true;
            StartCoroutine(MoveElevatorSequence(-1));
        }
    }

    // ─── Movement sequence ────────────────────────────────────────────────────

    private IEnumerator MoveElevatorSequence(int directionMultiplier)
    {
        isMoving = true;

        // FIX: Cache references. If the player steps out of the elevator right before it moves,
        // the global variables turn null. This local caching prevents NullReferenceExceptions.
        Transform cachedPlayerTransform = playerTransform;
        Rigidbody2D cachedPlayerRb = playerRigidbody;

        int departureFloor = currentFloor;
        EventManagement.OnElevatorPrepareToMove?.Invoke(departureFloor);

        yield return new WaitForSeconds(doorsWaitDuration);

        RigidbodyType2D previousBodyType = RigidbodyType2D.Dynamic;
        if (cachedPlayerRb != null)
        {
            previousBodyType = cachedPlayerRb.bodyType;
            cachedPlayerRb.bodyType = RigidbodyType2D.Kinematic;
        }

        float delta = floorHeight * directionMultiplier;

        transform.DOMoveY(transform.position.y + delta, moveDuration)
                 .SetEase(Ease.InOutSine);

        if (cameraTarget != null)
            cameraTarget.DOMoveY(cameraTarget.position.y + delta, moveDuration)
                        .SetEase(Ease.InOutSine);

        if (cachedPlayerTransform != null)
            cachedPlayerTransform.DOMoveY(cachedPlayerTransform.position.y + delta, moveDuration)
                                 .SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(moveDuration);

        currentFloor += directionMultiplier;
        EventManagement.CurrentFloor = currentFloor;

        if (cachedPlayerRb != null)
        {
            cachedPlayerRb.position = cachedPlayerTransform.position;
            cachedPlayerRb.bodyType = previousBodyType;
        }

        EventManagement.OnElevatorArrived?.Invoke(currentFloor);

        isMoving = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag) || isMoving) return;

        isPlayerInside  = true;
        playerTransform = other.transform;
        playerRigidbody = other.GetComponent<Rigidbody2D>();

        EventManagement.CurrentFloor = currentFloor;
        EventManagement.OnElevatorEnter?.Invoke(currentFloor);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        isPlayerInside  = false;
        playerTransform = null;
        playerRigidbody = null;

        if (!isMoving)
            EventManagement.OnElevatorExit?.Invoke(currentFloor);
    }
}