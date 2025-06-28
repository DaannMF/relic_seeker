using UnityEngine;

[CreateAssetMenu(fileName = "EnemyChase_Data", menuName = "Enemy States/Chase")]
public class EnemyChaseSO : BaseStateDO<EEnemyStates> {
    [Header("Chase Settings")]
    public float chaseSpeed = 6f;
    public float maxChaseDistance = 15f;
    public float losePlayerTime = 3f;
    public float attackDistance = 2f;

    public override BaseState<EEnemyStates> GetState(BaseStateContext<EEnemyStates> context) {
        return new EnemyChaseState(context as EnemyStateContext, this);
    }
}