using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CatPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float dashDistance = 10f;
    public float idleTimeout = 5f;
    public float idleSwitchInterval = 5f;

    [Header("Camera Settings")]
    public Transform cameraTransform;

    [Header("References")]
    public Animator animator;
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference jumpAction;
    public InputActionReference dashAction;
    public InputActionReference runAction;

    private CharacterController controller;
    private Vector2 inputMovement;
    private Vector3 velocity;
    private bool isGrounded;
    private bool canDash = false;
    private bool canRun = false;
    private float lastMovementTime;
    private float idleTimer;
    private float idleSwitchTimer;
    private bool isIdle = false;
    private bool idleToggle = false;

    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        jumpAction.action.Enable();
        dashAction.action.Enable();
        runAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        jumpAction.action.Disable();
        dashAction.action.Disable();
        runAction.action.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.stepOffset = 0.3f;
        controller.slopeLimit = 50f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5f))
{
    Vector3 groundPos = hit.point;
    transform.position = new Vector3(transform.position.x, groundPos.y + controller.height / 2f, transform.position.z);
}

    }

    void Update()
    {
        inputMovement = moveAction.action.ReadValue<Vector2>();

        isGrounded = controller.isGrounded;
        ApplyGravity();

        HandleMovement();
        HandleIdleAnimation();

        Debug.DrawRay(transform.position, Vector3.down * 0.5f, Color.red);
        Debug.Log($"Grounded: {controller.isGrounded}, Y: {transform.position.y}");
    }

    void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(inputMovement.x, 0f, inputMovement.y);
        moveDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * moveDirection;
        moveDirection.Normalize();

        float speed = canRun && runAction.action.IsPressed() ? runSpeed : walkSpeed;
        Vector3 motion = moveDirection * speed;
        motion.y = velocity.y;
        controller.Move(motion * Time.deltaTime);

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            lastMovementTime = Time.time;
            isIdle = false;
        }
        else
        {
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            if (camForward.sqrMagnitude > 0.1f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(camForward), Time.deltaTime * 5f);
        }

        animator.SetFloat("Speed", moveDirection.magnitude);

        if (jumpAction.action.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        if (dashAction.action.WasPressedThisFrame() && canDash)
        {
            Vector3 dashDirection = transform.forward;
            controller.Move(dashDirection * dashDistance);
            animator.SetTrigger("Dash");
            canDash = false;
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    void HandleIdleAnimation()
    {
         if (inputMovement.magnitude > 0.1f)
    {
        isIdle = false;
        animator.SetFloat("IdleVariant", 0);
        idleTimer = 0f;
        idleSwitchTimer = 0f;
        return;
    }

    idleTimer += Time.deltaTime;

    if (idleTimer > idleTimeout)
    {
        isIdle = true;
        idleSwitchTimer += Time.deltaTime;

        if (idleSwitchTimer > idleSwitchInterval)
        {
            idleToggle = !idleToggle;
            animator.SetFloat("IdleVariant", idleToggle ? 1f : 0f);
            idleSwitchTimer = 0f;
        }
    }
    }

    public void UnlockDash()
    {
        canDash = true;
    }

    public void UnlockRun()
    {
        canRun = true;
    }
}
