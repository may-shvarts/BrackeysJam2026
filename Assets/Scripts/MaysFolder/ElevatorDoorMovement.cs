using DG.Tweening;
using UnityEngine;

public class ElevatorDoorMovement : MonoBehaviour
{
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float startXPos = 6.93f;
    [SerializeField] private float endXPos = 5.65f;
    
    [SerializeField] private float secToWaitElevatorEnter = 2f;
    void Start()
    {
        MoveElevatorDoor();
    }

    void OnEnable()
    {
        EventManagement.OnElevatorEnter += MoveElevatorDoor;
    }

    void OnDisable()
    {
        EventManagement.OnElevatorEnter -= MoveElevatorDoor;
    }

    private void MoveElevatorDoor()
    {
        transform.position = new Vector3(startXPos, transform.position.y, transform.position.z);
        transform.DOMoveX(endXPos, duration)
            .SetEase(Ease.InOutQuad);
        DOVirtual.DelayedCall(secToWaitElevatorEnter, () => 
        {
            transform.DOMoveX(startXPos, duration)
                .SetEase(Ease.InOutQuad);
        });
        EventManagement.OnFloorChange?.Invoke();
    }
}
