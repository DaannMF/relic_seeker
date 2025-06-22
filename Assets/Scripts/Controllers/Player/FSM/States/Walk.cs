public class WalkState : BaseState<EPlayerStates> {
    public WalkState(WalkSO stateData) : base(EPlayerStates.Walk, stateData) {
    }

    public override EPlayerStates GetNextState() {
        throw new System.NotImplementedException();
    }
}