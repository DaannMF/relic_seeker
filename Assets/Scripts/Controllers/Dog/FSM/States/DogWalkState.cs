public class DogWalkState : BaseState<EDogStates> {
    private new DogStateContext Context => base.Context as DogStateContext;
    public new DogWalkSO StateData => base.StateData as DogWalkSO;

    public DogWalkState(EDogStates stateKey, BaseStateDO<EDogStates> stateData, BaseStateContext<EDogStates> context)
        : base(stateKey, stateData, context) {
    }

    public override void OnEnter() {
        Context.ResetStateTimer();

        if (!Context.HasTarget) {
            Context.TargetPosition = Context.GetRandomWalkPosition();
            Context.HasTarget = true;
        }
    }

    public override void Update() {
        Context.UpdateStateTimer();
    }

    public override void FixedUpdate() {
        if (Context.HasTarget) {
            float walkSpeed = StateData.WalkSpeed;
            Context.DogController.HandleMovement(Context.TargetPosition, walkSpeed);

            if (Context.DogController.IsAtPosition(Context.TargetPosition, 1.5f))
                Context.HasTarget = false;
        }
    }

    public override void Exit() {
        Context.HasTarget = false;
        Context.DogController.StopMovement();
    }

    public override EDogStates GetNextState() {
        // Priority: If player is detected, bark immediately
        if (Context.CanSeePlayer()) {
            return EDogStates.Bark;
        }

        float walkTime = StateData.WalkTime;

        if (!Context.HasTarget || Context.StateTimer >= walkTime) {
            return EDogStates.Idle;
        }

        return EDogStates.Walk;
    }
}