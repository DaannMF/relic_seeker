using UnityEngine;

public class IdleSate : BaseState<EPlayerStates> {

    public IdleSate(BaseStateDO<EPlayerStates> stateData) : base(EPlayerStates.Idle, stateData) { }

    public override EPlayerStates GetNextState() {
        throw new System.NotImplementedException();
    }
}