using UnityEngine;

public class IdleState : BaseState<EPlayerStates> {
    private new PlayerStateContext Context => base.Context as PlayerStateContext;

    public IdleState(PlayerStateContext context, IdleSO stateData) :
        base(EPlayerStates.Idle, stateData, context) {
    }

    public override void OnEnter() {
        Vector3 velocity = Context.PlayerController.Rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        Context.PlayerController.Rb.velocity = velocity;
    }

    public override void Update() {
        Vector3 velocity = Context.PlayerController.Rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        Context.PlayerController.Rb.velocity = velocity;
    }

    public override void FixedUpdate() {
        Context.ApplyGravity();
    }

    public override EPlayerStates GetNextState() {
        if (Context.Input.JumpPressed)
            return EPlayerStates.Jump;

        if (Context.Input.IsMoving) {
            if (Context.Input.RunHeld)
                return EPlayerStates.Run;

            return EPlayerStates.Walk;
        }

        return EPlayerStates.Idle;
    }
}