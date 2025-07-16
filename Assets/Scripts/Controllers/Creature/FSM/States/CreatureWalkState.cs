public class CreatureWalkState : BaseState<ECreatureStates> {
    private new CreatureStateContext Context => base.Context as CreatureStateContext;
    public new CreatureWalkSO StateData => base.StateData as CreatureWalkSO;

    public CreatureWalkState(ECreatureStates stateKey, BaseStateDO<ECreatureStates> stateData, BaseStateContext<ECreatureStates> context)
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
            Context.CreatureController.HandleMovement(Context.TargetPosition, walkSpeed);

            if (Context.CreatureController.IsAtPosition(Context.TargetPosition, 1.5f))
                Context.HasTarget = false;
        }
    }

    public override void Exit() {
        Context.HasTarget = false;
        Context.CreatureController.StopMovement();
    }

    public override ECreatureStates GetNextState() {
        float walkTime = StateData.WalkTime;

        if (!Context.HasTarget || Context.StateTimer >= walkTime) {
            return ECreatureStates.Idle;
        }

        return ECreatureStates.Walk;
    }
}