using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Targets")]
    public Transform player;       // Drag your Player object here
    public Transform playerVisual; // (Optional) Drag the model inside the player if you have a separate mesh

    [Header("Settings")]
    public float rotationSpeed = 2.0f;
    public float distanceFromPlayer = 5.0f;
    public float heightOffset = 1.5f;
    public Vector2 pitchLimits = new Vector2(-10f, 60f); // Stops camera from flipping over

    [Header("Smoothing")]
    public float smoothTime = 0.12f; // Higher = "Heavier" camera feel

    private float yaw;   // Horizontal rotation
    private float pitch; // Vertical rotation
    private Vector3 currentVelocity; // For the smoothing function

    void Start()
    {
        // Lock cursor so you can look around without clicking
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize rotation to current camera rotation
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    // LateUpdate is CRITICAL for cameras. It runs AFTER the player has moved.
    void LateUpdate()
    {
        if (!player) return;

        // 1. Get Mouse Input
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // 2. Calculate Rotation
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y); // Clamp vertical look

        // 3. Calculate Target Position (Orbit)
        // We create a rotation based on mouse input
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);

        // We calculate position: Player Position + Offset - (Forward * Distance)
        // "Forward * Distance" pushes the camera BACKWARDS away from the player
        Vector3 focusPoint = player.position + Vector3.up * heightOffset;
        Vector3 targetPosition = focusPoint - (targetRotation * Vector3.forward * distanceFromPlayer);

        // 4. Apply Smoothing (SmoothDamp)
        // This makes the camera catch up to the player rather than snapping instantly
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        // 5. Look at Player
        transform.LookAt(focusPoint);
    }
}