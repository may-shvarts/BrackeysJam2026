using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ElevatorDoorMovement : MonoBehaviour
{
    [Header("Floor Identity")]
    [Tooltip("Which floor does this door belong to? Must match ElevatorController floor numbers.")]
    [SerializeField] private int myFloor = 0;

    [Header("Door Movement")]
    [SerializeField] private float openDuration  = 1.5f;
    [SerializeField] private float closeDuration = 1.0f;

    [Tooltip("World X when this door is fully OPEN.")]
    [SerializeField] private float openXPos   = 5.65f;

    [Tooltip("World X when this door is fully CLOSED (covering the doorway).")]
    [SerializeField] private float closedXPos = 6.93f;

    [Header("Sorting Layer")]
    [SerializeField] private SpriteRenderer doorRenderer;
    [SerializeField] private int defaultSortingOrder = 3;
    [SerializeField] private int frontSortingOrder = 5;
    
    void OnEnable()
    {
        EventManagement.OnElevatorEnter += OnElevatorEnter;
        EventManagement.OnElevatorExit += OnElevatorExit;
        EventManagement.OnElevatorPrepareToMove += OnPrepareToMove;
        EventManagement.OnElevatorArrived += OnArrived;
        EventManagement.RestartGame += ResetDoor;
    }

    void OnDisable()
    {
        EventManagement.OnElevatorEnter -= OnElevatorEnter;
        EventManagement.OnElevatorExit -= OnElevatorExit;
        EventManagement.OnElevatorPrepareToMove -= OnPrepareToMove;
        EventManagement.OnElevatorArrived -= OnArrived;
        EventManagement.RestartGame -= ResetDoor;
    }

    private void OnElevatorEnter(int floor)
    {
        if (floor != myFloor) return;
        Open();
    }

    private void OnElevatorExit(int floor)
    {
        if (floor != myFloor) return;
        Close();
    }

    private void OnPrepareToMove(int departureFloor)
    {
        if (departureFloor != myFloor) return;
        SetSorting(frontSortingOrder);
        Close();
    }

    private void OnArrived(int arrivalFloor)
    {
        if (arrivalFloor != myFloor) return;
        SetSorting(frontSortingOrder);
        StartCoroutine(ResetSortingAfterDelay(openDuration));
        Open();
    }

    private IEnumerator ResetSortingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetSorting(defaultSortingOrder);
    }
    
    private void Open()
    {
        // FIX: Kill any active movement tweens on this object so they don't fight
        transform.DOKill();
        transform.DOMoveX(openXPos, openDuration).SetEase(Ease.InOutQuad);
    }

    private void Close()
    {
        // FIX: Kill any active movement tweens on this object so they don't fight
        transform.DOKill();
        transform.DOMoveX(closedXPos, closeDuration).SetEase(Ease.OutCubic);
    }

    private void SetSorting(int order)
    {
        if (doorRenderer != null)
        {
            doorRenderer.sortingOrder = order;
        }
    }
    
    private void ResetDoor()
    {
        StopAllCoroutines();
        transform.DOKill();
        
        Vector3 currentPos = transform.position;
        currentPos.x = closedXPos;
        transform.position = currentPos;
        
        SetSorting(defaultSortingOrder);
    }
}