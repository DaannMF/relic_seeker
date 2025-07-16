using UnityEngine;

public class CreatureIdleState : BaseState<ECreatureStates> {
    protected new CreatureStateContext Context => base.Context as CreatureStateContext;
    private new CreatureIdleSO StateData => base.StateData as CreatureIdleSO;

    public CreatureIdleState(ECreatureStates stateKey, BaseStateDO<ECreatureStates> stateData, BaseStateContext<ECreatureStates> context)
        : base(stateKey, stateData, context) {
    }

    public override void OnEnter() {
        Context.CreatureController.StopMovement();
        Context.ResetStateTimer();
    }

    public override void Update() {
        Context.UpdateStateTimer();
    }

    public override ECreatureStates GetNextState() {
        if (Context.StateTimer >= StateData.IdleTime) {
            if (Random.Range(0f, 1f) <= StateData.LookChance) {
                return ECreatureStates.Look;
            }

            return ECreatureStates.Walk;
        }

        return ECreatureStates.Idle;
    }
}