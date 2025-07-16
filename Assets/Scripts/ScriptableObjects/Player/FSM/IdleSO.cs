using UnityEngine;

[CreateAssetMenu(fileName = "PlayerIdle_Data", menuName = "FSM/Player/Idle", order = 1)]
public class IdleSO : BaseStateDO<EPlayerStates> {
    public override BaseState<EPlayerStates> GetState(BaseStateContext<EPlayerStates> context) {
        return new IdleState(context as PlayerStateContext, this);
    }
}