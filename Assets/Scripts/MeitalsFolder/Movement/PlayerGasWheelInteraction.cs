using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player-side gas wheel interaction:
/// - When the player enters a wheel trigger, it notifies UI (show prompt).
/// - When the player exits, it notifies UI (hide prompt).
/// - When the player presses Enter while on the wheel, it activates the wheel.
/// </summary>
public class PlayerGasWheelInteraction : MonoBehaviour
{
    private WheelGasSwitch currentWheel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var wheel = other.GetComponent<WheelGasSwitch>();
        if (wheel == null) return;

        currentWheel = wheel;
        EventManagement.OnGasWheelEnter?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var wheel = other.GetComponent<WheelGasSwitch>();
        if (wheel == null) return;

        if (currentWheel == wheel)
        {
            currentWheel = null;
            EventManagement.OnGasWheelExit?.Invoke();
        }
    }

    // New Input System: requires an action named "Interact" bound to Enter,
    // and PlayerInput set to "Send Messages"
    private void OnGasWheelUse(InputValue value)
    {
        if (!value.isPressed) return;
        if (currentWheel == null) return;

        currentWheel.Interact();
    }

}