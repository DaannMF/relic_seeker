using UnityEngine;

[CreateAssetMenu(fileName = "EnemyIdle_Data", menuName = "Enemy States/Idle")]
public class EnemyIdleSO : BaseStateDO<EEnemyStates> {
    [Header("Idle Settings")]
    public float idleTime = 2f;
    public float lookAroundChance = 0.3f;

    public override BaseState<EEnemyStates> GetState(BaseStateContext<EEnemyStates> context) {
        return new EnemyIdleState(context as EnemyStateContext, this);
    }
}