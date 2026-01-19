using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public float movementSpeed = 3f;
    public float rotSpeed = 450f;

    [Header("Jump / Gravity")]
    public float jumpHeight = 1.5f;
    [SerializeField] float fallingSpeed;

    [Header("Combo Settings")]
    public int maxCombo = 3;              // combo1, combo2, combo3
    public float comboResetTime = 1.0f;   // time to reset back to combo1

    private int comboIndex = 0;           // 0,1,2
    private float lastAttackTime = -999f;

    [Header("Reload Settings")]
    public float maxReloadDuration = 15f; // reload loops at most this long (seconds)
    private bool isReloading = false;
    private float reloadStartTime = 0f;

    [Header("Dash Settings")]
    public float dashDistance = 5f;       // how far to dash
    public float dashDuration = 0.15f;    // how long the dash burst lasts
    public float dashCooldown = 10f;      // 10 seconds cooldown

    private bool isDashing = false;
    private float dashStartTime = 0f;
    private float lastDashTime = -999f;
    private Vector3 dashDirection;

    [Header("Critical Attack Settings")]
    public float criticalAttackDuration = 1.0f; // how long the crit anim locks player
    private bool isCriticalAttacking = false;
    private float criticalStartTime = 0f;

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

        // camera-relative movement
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 movementDirection = (camForward * vertical) + (camRight * horizontal);
        float movementAmount = Mathf.Clamp01(movementDirection.magnitude);

        // ===================== JUMP / GRAVITY =====================
        if (CC.isGrounded)
        {
            fallingSpeed = -2f;

            if (Input.GetKeyDown(KeyCode.Space) && !isReloading && !isDashing && !isCriticalAttacking)
            {
                if (movementAmount < 0.1f)
                    anim.SetTrigger("StandJump");
                else
                    anim.SetTrigger("RunJump");

                float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
                fallingSpeed = jumpVelocity;
            }
        }
        else
        {
            fallingSpeed += Physics.gravity.y * Time.deltaTime;
        }

        // ===================== RELOAD (R key) =====================
        if (CC.isGrounded && !isReloading && !isDashing && !isCriticalAttacking &&
            movementAmount < 0.1f && Input.GetKeyDown(KeyCode.R))
        {
            // start reload loop
            isReloading = true;
            reloadStartTime = Time.time;
            anim.SetTrigger("Reload");
        }

        if (isReloading)
        {
            // stop reload if player starts walking
            if (movementAmount > 0.1f)
            {
                isReloading = false;
                // Animator transition Reload -> Blend Tree with Speed > 0 handles visuals
            }
            // or stop reload after max duration
            else if (Time.time - reloadStartTime > maxReloadDuration)
            {
                isReloading = false;
                // Force back to locomotion in case still stuck in Reload
                anim.CrossFade("Blend Tree", 0.1f); // <-- change "Blend Tree" to your locomotion state name
            }
        }

        // ===================== DASH (F key) =====================
        if (!isDashing && !isReloading && !isCriticalAttacking && CC.isGrounded &&
            Input.GetKeyDown(KeyCode.F))
        {
            float timeSinceLastDash = Time.time - lastDashTime;
            if (timeSinceLastDash >= dashCooldown)
            {
                // dash straight forward relative to where character faces
                dashDirection = transform.forward;
                dashDirection.y = 0f;
                dashDirection.Normalize();

                isDashing = true;
                dashStartTime = Time.time;
                lastDashTime = Time.time;

                // optional: play dash animation if you have one
                anim.SetTrigger("Dash");
            }
        }

        // ===================== CRITICAL ATTACK (Right Click) =====================
        if (!isCriticalAttacking && !isReloading && !isDashing && CC.isGrounded &&
            Input.GetMouseButtonDown(1))
        {
            isCriticalAttacking = true;
            criticalStartTime = Time.time;
            anim.SetTrigger("CriticalAttack");
        }

        // lock during crit based on timer
        if (isCriticalAttacking)
        {
            // lock input-based movement while crit is active
            movementDirection = Vector3.zero;
            movementAmount = 0f;

            // after the duration, unlock
            if (Time.time - criticalStartTime >= criticalAttackDuration)
            {
                isCriticalAttacking = false;
            }
        }

        // ===================== BASIC COMBO ATTACK (Left Click) =====================
        if (!isReloading && !isDashing && !isCriticalAttacking && CC.isGrounded &&
            Input.GetMouseButtonDown(0))
        {
            float timeSinceLast = Time.time - lastAttackTime;

            // if you waited too long, restart combo on next click
            if (timeSinceLast > comboResetTime)
            {
                comboIndex = 0;
            }

            // tell animator which combo state to use
            anim.SetInteger("ComboInt", comboIndex);
            anim.SetTrigger("Attack");

            // advance combo index for next click
            comboIndex++;
            if (comboIndex >= maxCombo)
                comboIndex = 0;

            lastAttackTime = Time.time;
        }

        // ===================== MOVE / DASH APPLY =====================
        Vector3 finalMove;

        if (isCriticalAttacking)
        {
            // lock horizontal movement during critical attack
            finalMove = Vector3.zero;
        }
        else if (isDashing)
        {
            float dashElapsed = Time.time - dashStartTime;
            float t = dashElapsed / dashDuration;

            if (t >= 1f)
            {
                // dash finished
                isDashing = false;
                finalMove = movementDirection.normalized * movementSpeed; // fall back to normal move
            }
            else
            {
                // move quickly in dashDirection
                // speed = distance / duration
                float dashSpeed = dashDistance / dashDuration;
                finalMove = dashDirection * dashSpeed;
            }
        }
        else
        {
            // normal movement
            finalMove = movementDirection.normalized * movementSpeed;
        }

        // apply gravity
        finalMove.y = fallingSpeed;

        if (CC.enabled)
            CC.Move(finalMove * Time.deltaTime);

        // rotate toward movement direction *unless* we are dashing, then face dashDirection
        if (isDashing)
        {
            if (dashDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dashDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
            }
        }
        else if (movementDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(movementDirection.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
        }

        anim.SetFloat("Speed", movementAmount);
    }
}
