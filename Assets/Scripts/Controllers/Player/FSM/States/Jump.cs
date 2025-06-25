using UnityEngine;

public class JumpState : BaseState<EPlayerStates> {
    private PlayerStateContext playerContext;
    private JumpSO jumpData;

    // Jump state variables
    private bool hasJumped;

    public JumpState(PlayerStateContext context, JumpSO stateData) : base(EPlayerStates.Jump, stateData, context) {
        playerContext = context;
        jumpData = stateData;
    }

    public override void Enter() {
        Debug.Log("Entering Jump State");

        // Set gravity settings from JumpSO
        playerContext.SetGravitySettings(jumpData.Gravity, jumpData.MaxFallSpeed);

        // Initialize jump
        hasJumped = false;

        // Apply initial jump force
        ApplyJumpForce();

        // Handle jump animations
        // if (playerContext.Animator != null) {
        //     playerContext.Animator.SetBool("IsJumping", true);
        //     playerContext.Animator.SetFloat("JumpVelocity", playerContext.Rb.velocity.y);
        // }
    }

    public override void Update() {
        // Handle rotation in all states
        playerContext.HandleRotation();

        // Handle controllable detection in all states
        playerContext.HandleDetectControllable();

        // Update animations
        // playerContext.Animator?.SetFloat("JumpVelocity", playerContext.Rb.velocity.y);
    }

    public override void FixedUpdate() {
        // Apply gravity using context (consistent across all states)
        playerContext.ApplyGravity();

        // Handle air movement
        HandleAirMovement();
    }

    public override void Exit() {
        Debug.Log("Exiting Jump State");

        // Reset jump state
        hasJumped = false;

        // Update animations
        playerContext.Animator?.SetBool("IsJumping", false);
    }

    public override EPlayerStates GetNextState() {
        // Check if we're grounded using context method
        if (playerContext.IsGrounded(jumpData.GroundCheckDistance, jumpData.GroundLayerMask)) {
            // Determine next ground state based on input
            if (playerContext.Input.IsMoving) {
                return EPlayerStates.Walk;
            }
            else {
                return EPlayerStates.Idle;
            }
        }

        // Stay in jump state while airborne
        return EPlayerStates.Jump;
    }

    private void ApplyJumpForce() {
        if (!hasJumped) {
            // Apply jump force
            Vector3 jumpForceVector = Vector3.up * jumpData.JumpForce;
            playerContext.Rb.AddForce(jumpForceVector, jumpData.JumpForceMode);

            hasJumped = true;
        }
    }

    private void HandleAirMovement() {
        Vector2 input = playerContext.Input.MovementInput;

        if (input.magnitude > 0.1f) {
            float horizontal = input.x;
            float vertical = input.y;

            Vector3 camForward = playerContext.CameraTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = playerContext.CameraTransform.right;
            camRight.y = 0f;
            camRight.Normalize();

            Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;

            // Calculate current horizontal velocity
            Vector3 currentVelocity = playerContext.Rb.velocity;
            Vector3 currentHorizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

            // Only apply force if we haven't reached max air speed
            if (currentHorizontalVelocity.magnitude < jumpData.MaxAirSpeed) {
                Vector3 airForce = moveDir * jumpData.AirMoveForce;
                playerContext.Rb.AddForce(airForce, ForceMode.Force);

                // Clamp horizontal velocity to max air speed
                Vector3 newVelocity = playerContext.Rb.velocity;
                Vector3 newHorizontalVelocity = new Vector3(newVelocity.x, 0f, newVelocity.z);

                if (newHorizontalVelocity.magnitude > jumpData.MaxAirSpeed) {
                    newHorizontalVelocity = newHorizontalVelocity.normalized * jumpData.MaxAirSpeed;
                    playerContext.Rb.velocity = new Vector3(newHorizontalVelocity.x, newVelocity.y, newHorizontalVelocity.z);
                }
            }
        }
    }
}