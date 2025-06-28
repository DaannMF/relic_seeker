using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPatrol_Data", menuName = "Enemy States/Patrol")]
public class EnemyPatrolSO : BaseStateDO<EEnemyStates> {
    [Header("Patrol Settings")]
    public float patrolSpeed = 3f;
    public float waitTimeAtPoint = 1.5f;
    public float detectionWhilePatrolling = 8f;

    public override BaseState<EEnemyStates> GetState(BaseStateContext<EEnemyStates> context) {
        return new EnemyPatrolState(context as EnemyStateContext, this);
    }
}