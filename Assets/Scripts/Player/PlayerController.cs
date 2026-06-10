using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动")]
    public float originalMoveSpeed = 15f;
    public float moveSpeed = 15f;
    public float walkSpeed = 8f;

    [Header("旋转")]
    public float rotationSpeed = 10f;

    [Header("跳跃")]
    public float jumpHeight = 2.5f;
    public float gravity = -30f;
    public float groundedGravity = -2f;

    [Header("攻击")]
    public float attackCooldown = 0.5f;

    [Header("组件")]
    private CharacterController characterController;
    private PlayerAnimation playerAnimation;
    private PlayerAttack playerAttack;

    [HideInInspector] public Vector3 movement;

    private float lastAttackTime;
    private float verticalVelocity;
    private bool isAttacking;

    [HideInInspector] public bool isInvulnerable;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerAttack = GetComponentInChildren<PlayerAttack>();
    }

    private void OnEnable()
    {
        moveSpeed = originalMoveSpeed;
        verticalVelocity = groundedGravity;
        isAttacking = false;
        isInvulnerable = false;
    }

    private void Update()
    {
        UpdateVerticalVelocity();

        Vector3 horizontalVelocity = Vector3.zero;
        if (!isAttacking)
        {
            HandleJump();
            horizontalVelocity = GetHorizontalMoveVelocity();
        }

        movement = horizontalVelocity + Vector3.up * verticalVelocity;
        characterController.Move(movement * Time.deltaTime);

        HandleAttack();
    }

    private Vector3 GetHorizontalMoveVelocity()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;
        if (inputDirection == Vector3.zero)
        {
            return Vector3.zero;
        }

        Vector3 moveDir = transform.TransformDirection(inputDirection);

        Camera activeCamera = Camera.main;
        if (activeCamera != null)
        {
            Vector3 camForward = activeCamera.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = activeCamera.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            moveDir = (camForward * inputDirection.z + camRight * inputDirection.x).normalized;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        return moveDir * moveSpeed;
    }

    private void HandleJump()
    {
        if (!characterController.isGrounded)
            return;

        if (Input.GetButtonDown("Jump"))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void UpdateVerticalVelocity()
    {
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedGravity;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void HandleAttack()
    {
        if (isAttacking)
            return;

        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            if (GameStatsManager.Instance != null)
                GameStatsManager.Instance.totalAttacks += 1;

            isAttacking = true;
            isInvulnerable = true;
            lastAttackTime = Time.time;

            playerAnimation.TriggerAttack();
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        isInvulnerable = false;
        playerAnimation.TriggerAttackEnd();
    }

    public void OnAttackAnimationFinished()
    {
        if (!isActiveAndEnabled || !isAttacking)
            return;

        EndAttack();
    }

    public void StopMove()
    {
        movement = Vector3.zero;
    }

    public void ReturnOriginalMoveSpeed()
    {
        moveSpeed = originalMoveSpeed;
    }
}
