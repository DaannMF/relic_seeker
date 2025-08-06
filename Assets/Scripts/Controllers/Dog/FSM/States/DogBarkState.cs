using UnityEngine;

public class DogBarkState : BaseState<EDogStates> {
    private new DogStateContext Context => base.Context as DogStateContext;
    private new DogBarkSO StateData => base.StateData as DogBarkSO;

    public DogBarkState(EDogStates stateKey, BaseStateDO<EDogStates> stateData, BaseStateContext<EDogStates> context)
        : base(stateKey, stateData, context) {
    }

    public override void OnEnter() {
        Context.DogController.StopMovement();
        Context.ResetStateTimer();
        Context.IsBarking = true;
        Context.DogController.PlayBarkSound();
    }

    public override void Update() {
        Context.UpdateStateTimer();

        if (Context.StateTimer > 0f && (int)(Context.StateTimer / StateData.BarkInterval) > (int)((Context.StateTimer - Time.deltaTime) / StateData.BarkInterval))
            Context.DogController.PlayBarkSound();
    }

    public override void Exit() {
        Context.IsBarking = false;
    }

    public override EDogStates GetNextState() {
        if (!Context.CanSeePlayer())
            return EDogStates.Idle;

        if (Context.StateTimer >= StateData.BarkDuration)
            return EDogStates.Idle;

        return EDogStates.Bark;
    }
}