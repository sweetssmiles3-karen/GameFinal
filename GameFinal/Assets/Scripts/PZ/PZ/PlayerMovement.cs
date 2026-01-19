using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public bool canMove = true;

    [Header("Movement Settings")]
    public float baseMoveSpeed = 5f;
    public float rotSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;

    [Header("Camera Settings")]
    public Transform cameraTransform;

    [Header("Audio SFX")] // <--- NEW: Drag Audio Clip Here
    public AudioClip dashSound;

    // Internal Variables
    private float speedMultiplier = 1f;
    private CharacterController controller;
    private Animator anim;
    private AudioSource audioSource; // <--- NEW: The Speaker
    private Vector3 velocity;
    private bool isDashing = false;
    private float lastDashTime = -100f;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>(); // <--- NEW: Find AudioSource

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (!canMove)
        {
            anim.SetFloat("Speed", 0);
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(x, 0f, z).normalized;

        // 1. MOVEMENT
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            float finalSpeed = baseMoveSpeed * speedMultiplier;
            controller.Move(moveDir.normalized * finalSpeed * Time.deltaTime);

        }

        anim.SetFloat("Speed", direction.magnitude, 0.1f, Time.deltaTime);

        // 2. JUMP
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            if (direction.magnitude > 0.1f) anim.SetTrigger("RunJump");
            else anim.SetTrigger("StandJump");

            velocity.y = Mathf.Sqrt(2f * -2f * gravity);
        }

        // 3. DASH WITH COOLDOWN
        if (Input.GetKeyDown(KeyCode.F) && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(DashRoutine());
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        anim.SetTrigger("Dash");

        // <--- NEW: Play Sound Logic ---
        if (audioSource != null && dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }
        isDashing = false;
    }

    // 给外部IceTrap调用
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Clamp(multiplier, 0f, 2f);
    }

}