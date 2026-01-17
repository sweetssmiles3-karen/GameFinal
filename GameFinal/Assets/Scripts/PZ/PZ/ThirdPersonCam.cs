using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Targets")]
    public Transform player;       
    public Transform playerVisual; 

    [Header("Settings")]
    public float rotationSpeed = 2.0f;
    public float distanceFromPlayer = 5.0f;
    public float heightOffset = 1.5f;
    public Vector2 pitchLimits = new Vector2(-10f, 60f); 

    [Header("Smoothing")]
    public float smoothTime = 0.12f; 

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

   
    void LateUpdate()
    {
        if (!player) return;

        //  Get Mouse Input
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        //  Calculate Rotation
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y); // Clamp vertical look

       
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);

        
        Vector3 focusPoint = player.position + Vector3.up * heightOffset;
        Vector3 targetPosition = focusPoint - (targetRotation * Vector3.forward * distanceFromPlayer);

       
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        
        transform.LookAt(focusPoint);
    }
}