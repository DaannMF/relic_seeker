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
        playerContext.PlayerController.HandleRotation();

        playerContext.PlayerController.HandleDetectControllable();

        HandleMovement();
    }

    public override void FixedUpdate() {
        playerContext.ApplyGravity();
    }

    public override void Exit() {
        playerContext.Animator.SetBool("isRunning", false);

        Vector3 velocity = playerContext.PlayerController.Rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        playerContext.PlayerController.Rb.velocity = velocity;
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

        Vector3 camForward = playerContext.PlayerController.GetCameraForward();
        Vector3 camRight = playerContext.PlayerController.GetCameraRight();

        Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;
        Vector3 velocity = new Vector3(moveDir.x * runData.RunSpeed, playerContext.PlayerController.Rb.velocity.y, moveDir.z * runData.RunSpeed);

        if (playerContext.CanMove(moveDir, runData.MaxAngleMovement)) {
            playerContext.PlayerController.Rb.velocity = velocity;
        }
    }
}