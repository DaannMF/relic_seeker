using UnityEngine;

public class RunState : BaseState<EPlayerStates> {
    private PlayerStateContext playerContext;
    private RunSO runData;

    public RunState(PlayerStateContext context, RunSO stateData) : base(EPlayerStates.Run, stateData, context) {
        playerContext = context;
        runData = stateData;
    }

    public override void Enter() {
        playerContext.Animator.SetBool("isRunning", true);
    }

    public override void Update() {
        playerContext.HandleRotation();

        playerContext.HandleDetectControllable();

        HandleMovement();
    }

    public override void FixedUpdate() {
        playerContext.ApplyGravity();
    }

    public override void Exit() {
        playerContext.Animator.SetBool("isRunning", false);

        Vector3 velocity = playerContext.Rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        playerContext.Rb.velocity = velocity;
    }

    public override EPlayerStates GetNextState() {
        if (playerContext.Input.JumpPressed)
            return EPlayerStates.Jump;

        if (playerContext.Input.IsMoving) {
            if (playerContext.Input.RunHeld)
                return EPlayerStates.Run;
            else
                return EPlayerStates.Walk;

        }

        return EPlayerStates.Idle;
    }

    private void HandleMovement() {
        Vector2 input = playerContext.Input.MovementInput;

        float horizontal = input.x;
        float vertical = input.y;

        Vector3 camForward = playerContext.CameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = playerContext.CameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;
        Vector3 velocity = new Vector3(moveDir.x * runData.RunSpeed, playerContext.Rb.velocity.y, moveDir.z * runData.RunSpeed);

        if (playerContext.CanMove(moveDir, runData.MaxAngleMovement)) {
            playerContext.Rb.velocity = velocity;
        }
    }
}