using UnityEngine;

public class WalkState : BaseState<EPlayerStates> {
    private PlayerStateContext playerContext;
    private WalkSO walkData;

    public WalkState(PlayerStateContext context, WalkSO stateData) : base(EPlayerStates.Walk, stateData, context) {
        playerContext = context;
        walkData = stateData;
    }

    public override void Enter() {
        playerContext.Animator.SetBool("isWalking", true);
    }

    public override void Update() {
        // Handle rotation in all states
        playerContext.PlayerController.HandleRotation();

        // Handle controllable detection in all states
        playerContext.PlayerController.HandleDetectControllable();

        // Handle movement
        HandleMovement();
    }

    public override void FixedUpdate() {
        // Apply gravity to keep player grounded
        playerContext.ApplyGravity();
    }

    public override void Exit() {
        playerContext.Animator.SetBool("isWalking", false);

        // Reset horizontal velocity when exiting walk state
        Vector3 velocity = playerContext.PlayerController.Rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        playerContext.PlayerController.Rb.velocity = velocity;
    }

    public override EPlayerStates GetNextState() {
        // Check for jump input first (highest priority)
        if (playerContext.Input.JumpPressed) {
            return EPlayerStates.Jump;
        }

        // Then check for movement
        if (playerContext.Input.IsMoving) {

            if (playerContext.Input.RunHeld)
                return EPlayerStates.Run;


            return EPlayerStates.Walk;
        }

        // No movement, go to idle
        return EPlayerStates.Idle;
    }

    private void HandleMovement() {
        Vector2 input = playerContext.Input.MovementInput;

        float horizontal = input.x;
        float vertical = input.y;

        Vector3 camForward = playerContext.PlayerController.GetCameraForward();
        Vector3 camRight = playerContext.PlayerController.GetCameraRight();

        Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;
        Vector3 velocity = new Vector3(moveDir.x * walkData.MoveSpeed, playerContext.PlayerController.Rb.velocity.y, moveDir.z * walkData.MoveSpeed);

        if (playerContext.CanMove(moveDir, walkData.MaxAngleMovement)) {
            playerContext.PlayerController.Rb.velocity = velocity;
        }
    }
}