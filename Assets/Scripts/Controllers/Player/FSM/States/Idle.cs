using UnityEngine;
using UnityEngine.Assertions;

public class IdleState : BaseState<EPlayerStates>
{
    private PlayerStateContext playerContext;
    private IdleSO idleData;

    public IdleState(PlayerStateContext context, IdleSO stateData) : base(EPlayerStates.Idle, stateData, context)
    {
        playerContext = context;
        idleData = stateData;
    }

    public override void Enter()
    {
        if (idleData.EnableIdleAnimations)
            playerContext.Animator.SetBool("isIdle", true);

        Vector3 velocity = playerContext.Rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        playerContext.Rb.velocity = velocity;
    }

    public override void Update()
    {
        playerContext.HandleRotation();

        playerContext.HandleDetectControllable();

        Vector3 velocity = playerContext.Rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        playerContext.Rb.velocity = velocity;
    }

    public override void FixedUpdate()
    {
        playerContext.ApplyGravity();
    }

    public override void Exit()
    {
        if (idleData.EnableIdleAnimations)
            playerContext.Animator.SetBool("isIdle", false);
    }

    public override EPlayerStates GetNextState()
    {
        if (playerContext.Input.JumpPressed)
            return EPlayerStates.Jump;

        if (playerContext.Input.IsMoving)
        {
            if (playerContext.Input.RunHeld)
                return EPlayerStates.Run;

            return EPlayerStates.Walk;
        }

        return EPlayerStates.Idle;
    }
}