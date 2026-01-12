using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public float movementSpeed = 3f;
    public float rotSpeed = 450f;

    [Header("Jump / Gravity")]
    public float jumpHeight = 1.5f;      // how high the character jumps
    [SerializeField] float fallingSpeed; // current vertical velocity

    Animator anim;
    CharacterController CC;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponent<Animator>();
        CC = GetComponent<CharacterController>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Camera directions flattened on Y (camera-relative movement)
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 movementDirection = (camForward * vertical) + (camRight * horizontal);
        float movementAmount = Mathf.Clamp01(movementDirection.magnitude);

        // ============================================================
        // JUMP + GRAVITY
        // ============================================================
        if (CC.isGrounded)
        {
            fallingSpeed = -2f; // keeps controller grounded properly

            // Jump input
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Standing jump vs running jump
                if (movementAmount < 0.1f)
                    anim.SetTrigger("StandJump");
                else
                    anim.SetTrigger("RunJump");

                // Apply upward velocity
                float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
                fallingSpeed = jumpVelocity;
            }
        }
        else
        {
            // In-air gravity
            fallingSpeed += Physics.gravity.y * Time.deltaTime;
        }

        // ============================================================
        // ATTACK (LEFT CLICK) — only when grounded!
        // ============================================================
        if (CC.isGrounded && Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
        }

        // ============================================================
        // MOVE
        // ============================================================
        Vector3 finalMove = movementDirection.normalized * movementSpeed;
        finalMove.y = fallingSpeed;

        if (CC.enabled)
            CC.Move(finalMove * Time.deltaTime);

        // Rotate toward movement direction
        if (movementDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(movementDirection.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
        }

        // Blend tree parameter
        anim.SetFloat("Speed", movementAmount);
    }
}
