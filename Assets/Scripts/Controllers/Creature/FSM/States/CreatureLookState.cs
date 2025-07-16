using UnityEngine;

public class CreatureLookState : BaseState<ECreatureStates> {
    private new CreatureStateContext Context => base.Context as CreatureStateContext;
    private new CreatureLookSO StateData => base.StateData as CreatureLookSO;

    public CreatureLookState(ECreatureStates stateKey, BaseStateDO<ECreatureStates> stateData, BaseStateContext<ECreatureStates> context)
        : base(stateKey, stateData, context) {
    }

    public override void OnEnter() {
        Context.CreatureController.StopMovement();
        Context.ResetStateTimer();

        PerformLookRotation();
    }

    public override void Update() {
        Context.UpdateStateTimer();
    }

    public override ECreatureStates GetNextState() {
        float lookTime = StateData.LookTime;

        if (Context.StateTimer >= lookTime) {
            return ECreatureStates.Walk;
        }

        return ECreatureStates.Look;
    }

    private void PerformLookRotation() {
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0f;
        randomDirection.Normalize();

        if (randomDirection != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(randomDirection);
            Context.CreatureController.transform.rotation = targetRotation;
        }
    }
}