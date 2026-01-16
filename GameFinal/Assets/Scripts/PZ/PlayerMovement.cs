using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;

    [Header("Physics")]
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Camera")]
    public Transform cameraTransform; // Assign Main Camera
    public float turnSmoothTime = 0.1f;

    // Internals
    private CharacterController controller;
    private Animator anim;
    private Vector3 velocity;
    private float turnSmoothVelocity;
    private bool isDashing;
    private float nextDashTime;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked; // Hides mouse cursor
    }

    void Update()
    {
        // 1. Gravity & Ground Check
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // 2. Dash Logic (F Key)
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextDashTime)
        {
            StartCoroutine(PerformDash());
        }

        if (isDashing) return; // Don't allow normal movement while dashing

        // 3. Normal Movement (WASD)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(x, 0f, z).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // 4. Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            anim.SetTrigger("Jump");
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Animations
        float speedPercent = direction.magnitude;
        anim.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
    }

    System.Collections.IEnumerator PerformDash()
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown;
        anim.SetTrigger("Dash");

        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }
        isDashing = false;
    }
}