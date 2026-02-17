using System;
using DG.Tweening;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [Header("Elevator Settings")]
    [SerializeField] private float floorHeight = 10f;
    [SerializeField] private float moveDuration = 2f;
    
    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTarget;
    
    [SerializeField] private String playerTag = "Player";

    void OnEnable()
    {
        EventManagement.OnFloorChange += MoveElevatorUp;
    }

    void OnDisable()
    {
        EventManagement.OnFloorChange -= MoveElevatorUp;
    }

    private void MoveElevatorUp()
    {
        transform.DOMoveY(transform.position.y + floorHeight, moveDuration)
            .SetEase(Ease.InOutSine);
        if (cameraTarget != null)
        {
            cameraTarget.DOMoveY(cameraTarget.position.y + floorHeight, moveDuration)
                .SetEase(Ease.InOutSine);
        }
    }
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            other.transform.SetParent(null);
        }
    }
    */
}