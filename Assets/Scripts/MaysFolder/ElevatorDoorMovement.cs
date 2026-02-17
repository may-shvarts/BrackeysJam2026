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
    [SerializeField] private string defaultSortingLayer = "Default";
    [SerializeField] private int    defaultSortingOrder = 0;
    [SerializeField] private string frontSortingLayer   = "Foreground";
    [SerializeField] private int    frontSortingOrder   = 10;
    
    void OnEnable()
    {
        EventManagement.OnElevatorEnter         += OnElevatorEnter;
        EventManagement.OnElevatorExit          += OnElevatorExit;
        EventManagement.OnElevatorPrepareToMove += OnPrepareToMove;
        EventManagement.OnElevatorArrived       += OnArrived;
    }

    void OnDisable()
    {
        EventManagement.OnElevatorEnter         -= OnElevatorEnter;
        EventManagement.OnElevatorExit          -= OnElevatorExit;
        EventManagement.OnElevatorPrepareToMove -= OnPrepareToMove;
        EventManagement.OnElevatorArrived       -= OnArrived;
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
        SetSorting(frontSortingLayer, frontSortingOrder);
        Close();
    }

    private void OnArrived(int arrivalFloor)
    {
        if (arrivalFloor != myFloor) return;
        SetSorting(defaultSortingLayer, defaultSortingOrder);
        Open();
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

    private void SetSorting(string layerName, int order)
    {
        if (doorRenderer != null)
        {
            doorRenderer.sortingLayerName = layerName;
            doorRenderer.sortingOrder     = order;
        }
    }
}