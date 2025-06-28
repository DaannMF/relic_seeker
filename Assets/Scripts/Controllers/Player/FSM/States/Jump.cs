using UnityEngine;

public class JumpState : BaseState<EPlayerStates> {
    private PlayerStateContext playerContext;
    private JumpSO jumpData;

    private bool hasJumped;
    private float jumpTimer;
    private const float MIN_JUMP_TIME = 0.1f;

    public JumpState(PlayerStateContext context, JumpSO stateData) : base(EPlayerStates.Jump, stateData, context) {
        playerContext = context;
        jumpData = stateData;
    }

    public override void Enter() {
        playerContext.Animator.SetBool("isJumping", true);

        playerContext.SetGravitySettings(jumpData.Gravity, jumpData.MaxFallSpeed);

        hasJumped = false;
        jumpTimer = 0f;

        ApplyJumpForce();
    }

    public override void Update() {
        jumpTimer += Time.deltaTime;

        playerContext.PlayerController.HandleRotation();
        playerContext.PlayerController.HandleDetectControllable();
    }

    public override void FixedUpdate() {
        playerContext.ApplyGravity();
        HandleAirMovement();
    }

    public override void Exit() {
        playerContext.Animator.SetBool("isJumping", false);
        hasJumped = false;
        jumpTimer = 0f;
    }

    public override EPlayerStates GetNextState() {
        if (jumpTimer >= MIN_JUMP_TIME &&
            playerContext.PlayerController.Rb.velocity.y <= 0.1f &&
            playerContext.IsGrounded(jumpData.GroundCheckDistance, jumpData.GroundLayerMask)) {
            return playerContext.Input.IsMoving ? EPlayerStates.Walk : EPlayerStates.Idle;
        }

        return EPlayerStates.Jump;
    }

    private void ApplyJumpForce() {
        if (!hasJumped) {
            Vector3 jumpForceVector = Vector3.up * jumpData.JumpForce;
            playerContext.PlayerController.Rb.AddForce(jumpForceVector, jumpData.JumpForceMode);

            hasJumped = true;
        }
    }

    private void HandleAirMovement() {
        Vector2 input = playerContext.Input.MovementInput;

        if (input.magnitude > 0.1f) {
            float horizontal = input.x;
            float vertical = input.y;

            Vector3 camForward = playerContext.PlayerController.GetCameraForward();
            Vector3 camRight = playerContext.PlayerController.GetCameraRight();

            Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;

            Vector3 currentVelocity = playerContext.PlayerController.Rb.velocity;
            Vector3 currentHorizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

            if (currentHorizontalVelocity.magnitude < jumpData.MaxAirSpeed) {
                Vector3 airForce = moveDir * jumpData.AirMoveForce;
                playerContext.PlayerController.Rb.AddForce(airForce, ForceMode.Force);

                Vector3 newVelocity = playerContext.PlayerController.Rb.velocity;
                Vector3 newHorizontalVelocity = new Vector3(newVelocity.x, 0f, newVelocity.z);

                if (newHorizontalVelocity.magnitude > jumpData.MaxAirSpeed) {
                    newHorizontalVelocity = newHorizontalVelocity.normalized * jumpData.MaxAirSpeed;
                    playerContext.PlayerController.Rb.velocity = new Vector3(newHorizontalVelocity.x, newVelocity.y, newHorizontalVelocity.z);
                }
            }
        }
    }
}