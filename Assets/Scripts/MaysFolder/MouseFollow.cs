using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Check this to make the movement smooth, uncheck for instant snapping.")]
    public bool smoothMovement = true;
    
    [Tooltip("How fast the object follows the mouse if smooth movement is enabled.")]
    public float followSpeed = 10f;

    private Camera mainCamera;

    void Start()
    {
        // Cache the main camera for performance
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. Get the mouse position in pixel coordinates
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. Set the Z distance to match the distance between the camera and the object.
        // This ensures the object doesn't jump to the camera's near clip plane.
        mouseScreenPosition.z = mainCamera.WorldToScreenPoint(transform.position).z;

        // 3. Convert the screen coordinates to world coordinates
        Vector3 targetWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        // 4. Move the object
        if (smoothMovement)
        {
            // Move smoothly towards the mouse
            transform.position = Vector3.Lerp(transform.position, targetWorldPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            // Snap instantly to the mouse
            transform.position = targetWorldPosition;
        }
    }
}