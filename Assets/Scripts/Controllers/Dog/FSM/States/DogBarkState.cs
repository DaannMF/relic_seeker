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

        // Play bark sound at intervals during the bark state
        if (Context.StateTimer > 0f && (int)(Context.StateTimer / StateData.BarkInterval) > (int)((Context.StateTimer - Time.deltaTime) / StateData.BarkInterval)) {
            Context.DogController.PlayBarkSound();
        }
    }

    public override void Exit() {
        Context.IsBarking = false;
    }

    public override EDogStates GetNextState() {
        // If player is no longer visible, stop barking
        if (!Context.CanSeePlayer()) {
            return EDogStates.Idle;
        }

        // Continue barking while player is visible, up to max duration
        if (Context.StateTimer >= StateData.BarkDuration) {
            return EDogStates.Idle;
        }

        return EDogStates.Bark;
    }
}