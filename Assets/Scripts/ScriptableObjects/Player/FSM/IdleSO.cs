using UnityEngine;

[CreateAssetMenu(fileName = "PlayerIdle_Data", menuName = "FSM/Player/Idle", order = 1)]
public class IdleSO : BaseStateDO<EPlayerStates> {
    [Header("Idle Settings")]
    [SerializeField] private bool enableIdleAnimations = true;

    public bool EnableIdleAnimations => enableIdleAnimations;

    public override BaseState<EPlayerStates> GetState(BaseStateContext<EPlayerStates> context) {
        return new IdleState(context as PlayerStateContext, this);
    }
}