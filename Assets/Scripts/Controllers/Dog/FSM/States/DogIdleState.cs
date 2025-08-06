public class DogIdleState : BaseState<EDogStates> {
    protected new DogStateContext Context => base.Context as DogStateContext;
    private new DogIdleSO StateData => base.StateData as DogIdleSO;

    public DogIdleState(EDogStates stateKey, BaseStateDO<EDogStates> stateData, BaseStateContext<EDogStates> context)
        : base(stateKey, stateData, context) {
    }

    public override void OnEnter() {
        Context.DogController.StopMovement();
        Context.ResetStateTimer();
        Context.IsBarking = false;
    }

    public override void Update() {
        Context.UpdateStateTimer();
    }

    public override EDogStates GetNextState() {
        if (Context.CanSeePlayer())
            return EDogStates.Bark;

        if (Context.StateTimer >= StateData.IdleTime)
            return EDogStates.Walk;

        return EDogStates.Idle;
    }
}