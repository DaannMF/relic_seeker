using UnityEngine;

[CreateAssetMenu(fileName = "PlayerIdle_Data", menuName = "FSM/Player/Idle", order = 1)]
public class IdleSO : BaseStateDO<EPlayerStates> {
    [Header("Idle Settings")]
    [SerializeField] private float idleAnimationTransitionTime = 0.1f;
    [SerializeField] private bool enableIdleAnimations = true;

    // Properties to access the settings
    public float IdleAnimationTransitionTime => idleAnimationTransitionTime;
    public bool EnableIdleAnimations => enableIdleAnimations;

    public override BaseState<EPlayerStates> GetState(BaseStateContext<EPlayerStates> context) {
        return new IdleState(context as PlayerStateContext, this);
    }
}