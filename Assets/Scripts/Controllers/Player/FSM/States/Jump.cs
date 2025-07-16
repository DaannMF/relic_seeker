using UnityEngine;

public class JumpState : BaseState<EPlayerStates> {
    private const float MIN_JUMP_TIME = 0.1f;

    private new PlayerStateContext Context => base.Context as PlayerStateContext;
    private new JumpSO StateData => base.StateData as JumpSO;

    private bool hasJumped;
    private float jumpTimer;

    public JumpState(PlayerStateContext context, JumpSO stateData) :
        base(EPlayerStates.Jump, stateData, context) {
    }

    public override void OnEnter() {
        Context.SetGravitySettings(StateData.Gravity, StateData.MaxFallSpeed);

        hasJumped = false;
        jumpTimer = 0f;

        ApplyJumpForce();
    }

    public override void Update() {
        jumpTimer += Time.deltaTime;
    }

    public override void FixedUpdate() {
        Context.ApplyGravity();
        HandleAirMovement();
    }

    public override void Exit() {
        hasJumped = false;
        jumpTimer = 0f;
    }

    public override EPlayerStates GetNextState() {
        if (jumpTimer >= MIN_JUMP_TIME &&
            Context.PlayerController.Rb.velocity.y <= 0.1f &&
            Context.IsGrounded(StateData.GroundCheckDistance, StateData.GroundLayerMask)) {
            return Context.Input.IsMoving ? EPlayerStates.Walk : EPlayerStates.Idle;
        }

        return EPlayerStates.Jump;
    }

    private void ApplyJumpForce() {
        if (!hasJumped) {
            Vector3 jumpForceVector = Vector3.up * StateData.JumpForce;
            Context.PlayerController.Rb.AddForce(jumpForceVector, StateData.JumpForceMode);

            hasJumped = true;
        }
    }

    private void HandleAirMovement() {
        Vector2 input = Context.Input.MovementInput;

        if (input.magnitude > 0.1f) {
            float horizontal = input.x;
            float vertical = input.y;

            Vector3 camForward = Context.PlayerController.GetCameraForward();
            Vector3 camRight = Context.PlayerController.GetCameraRight();

            Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;

            Vector3 currentVelocity = Context.PlayerController.Rb.velocity;
            Vector3 currentHorizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

            if (currentHorizontalVelocity.magnitude < StateData.MaxAirSpeed) {
                Vector3 airForce = moveDir * StateData.AirMoveForce;
                Context.PlayerController.Rb.AddForce(airForce, ForceMode.Force);

                Vector3 newVelocity = Context.PlayerController.Rb.velocity;
                Vector3 newHorizontalVelocity = new Vector3(newVelocity.x, 0f, newVelocity.z);

                if (newHorizontalVelocity.magnitude > StateData.MaxAirSpeed) {
                    newHorizontalVelocity = newHorizontalVelocity.normalized * StateData.MaxAirSpeed;
                    Context.PlayerController.Rb.velocity = new Vector3(newHorizontalVelocity.x, newVelocity.y, newHorizontalVelocity.z);
                }
            }
        }
    }
}