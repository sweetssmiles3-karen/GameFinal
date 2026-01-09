using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float movementSpeed = 3f;

    public float rotSpeed = 450f;

    Animator anim;
    CharacterController CC;

    [SerializeField] float fallingSpeed;

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

        // Camera directions flattened on Y
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 movementDirection = (camForward * vertical) + (camRight * horizontal);
        float movementAmount = Mathf.Clamp01(movementDirection.magnitude);

        // Gravity
        if (CC.isGrounded)
            fallingSpeed = -1f;
        else
            fallingSpeed += Physics.gravity.y * Time.deltaTime;

        // Move
        Vector3 finalMove = movementDirection.normalized * movementSpeed;
        finalMove.y = fallingSpeed;

        if (CC.enabled)
            CC.Move(finalMove * Time.deltaTime);

        // Rotate player toward movement
        if (movementDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(movementDirection.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
        }

        anim.SetFloat("Speed", movementAmount);
    }
}
