using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttack_Data", menuName = "Enemy States/Attack")]
public class EnemyAttackSO : BaseStateDO<EEnemyStates> {
    [Header("Attack Settings")]
    public float attackDamage = 25f;
    public float attackCooldown = 1.5f;
    public float attackRange = 2f;
    public float knockbackForce = 5f;
    public LayerMask attackLayerMask = 1;

    public override BaseState<EEnemyStates> GetState(BaseStateContext<EEnemyStates> context) {
        return new EnemyAttackState(context as EnemyStateContext, this);
    }
}