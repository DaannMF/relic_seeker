using UnityEngine;

public class IdleState : BaseState<EPlayerStates> {
    private PlayerStateContext playerContext;
    private IdleSO idleData;

    public IdleState(PlayerStateContext context, IdleSO stateData) : base(EPlayerStates.Idle, stateData, context) {
        playerContext = context;
        idleData = stateData;
    }

    public override void Enter() {
        Debug.Log("Entering Idle State");

        // Reset horizontal velocity to stop movement
        Vector3 velocity = playerContext.Rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        playerContext.Rb.velocity = velocity;

        // // Handle idle animations if enabled
        // if (idleData.EnableIdleAnimations && playerContext.Animator != null) {
        //     // Set idle animation parameters or states here
        //     playerContext.Animator.SetFloat("MovementSpeed", 0f);
        // }
    }

    public override void Update() {
        // Handle rotation in all states
        playerContext.HandleRotation();

        // Handle controllable detection in all states
        playerContext.HandleDetectControllable();

        // Ensure we stay stopped while in idle (in case of external forces)
        Vector3 velocity = playerContext.Rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        playerContext.Rb.velocity = velocity;
    }

    public override void FixedUpdate() {
        // Apply gravity to keep player grounded
        playerContext.ApplyGravity();
    }

    public override EPlayerStates GetNextState() {
        // Check for jump input first (highest priority)
        if (playerContext.Input.JumpPressed) {
            return EPlayerStates.Jump;
        }

        // Then check for movement
        if (playerContext.Input.IsMoving) {
            return EPlayerStates.Walk;
        }

        // Stay in idle
        return EPlayerStates.Idle;
    }
}