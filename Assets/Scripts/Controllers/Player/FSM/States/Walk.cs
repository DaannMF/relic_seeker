using UnityEngine;

public class WalkState : BaseState<EPlayerStates> {
    private new PlayerStateContext Context => base.Context as PlayerStateContext;
    private new WalkSO StateData => base.StateData as WalkSO;

    public WalkState(PlayerStateContext context, WalkSO stateData) :
        base(EPlayerStates.Walk, stateData, context) {
    }

    public override void Update() {
        HandleMovement();
    }

    public override void FixedUpdate() {
        Context.ApplyGravity();
    }

    public override void Exit() {
        Vector3 velocity = Context.PlayerController.Rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        Context.PlayerController.Rb.velocity = velocity;
    }

    public override EPlayerStates GetNextState() {
        if (Context.Input.JumpPressed) {
            return EPlayerStates.Jump;
        }

        if (Context.Input.IsMoving) {

            if (Context.Input.RunHeld)
                return EPlayerStates.Run;


            return EPlayerStates.Walk;
        }

        return EPlayerStates.Idle;
    }

    private void HandleMovement() {
        Vector2 input = Context.Input.MovementInput;

        float horizontal = input.x;
        float vertical = input.y;

        Vector3 camForward = Context.PlayerController.GetCameraForward();
        Vector3 camRight = Context.PlayerController.GetCameraRight();

        Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;
        Vector3 velocity = new Vector3(moveDir.x * StateData.MoveSpeed, Context.PlayerController.Rb.velocity.y, moveDir.z * StateData.MoveSpeed);

        if (Context.CanMove(moveDir, StateData.MaxAngleMovement)) {
            Context.PlayerController.Rb.velocity = velocity;
        }
    }
}