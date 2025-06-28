using UnityEngine;

public class EnemyAttackState : BaseState<EEnemyStates> {
    private EnemyStateContext enemyContext;
    private EnemyAttackSO attackData;
    private bool hasAttacked;

    public EnemyAttackState(EnemyStateContext context, EnemyAttackSO stateData) : base(EEnemyStates.Attack, stateData, context) {
        enemyContext = context;
        attackData = stateData;
    }

    public override void Enter() {
        enemyContext.Animator.SetBool("isAttacking", true);
        enemyContext.Animator.SetTrigger("attack");
        enemyContext.EnemyController.StopMovement();
        enemyContext.ResetStateTimer();
        hasAttacked = false;

        Vector3 directionToPlayer = (enemyContext.EnemyController.Player.position - enemyContext.EnemyController.transform.position).normalized;
        directionToPlayer.y = 0f;
        enemyContext.EnemyController.transform.rotation = Quaternion.LookRotation(directionToPlayer);
    }

    public override void Update() {
        enemyContext.UpdateStateTimer();

        if (!hasAttacked && enemyContext.StateTimer >= 0.5f) {
            PerformAttack();
            hasAttacked = true;
        }
    }

    public override void FixedUpdate() {
    }

    public override void Exit() {
        enemyContext.Animator.SetBool("isAttacking", false);
        hasAttacked = false;
    }

    public override EEnemyStates GetNextState() {
        if (enemyContext.StateTimer >= attackData.attackCooldown) {
            float distanceToPlayer = enemyContext.EnemyController.DistanceToPlayer();

            if (distanceToPlayer <= attackData.attackRange && enemyContext.EnemyController.CanSeePlayer()) {
                return EEnemyStates.Attack;
            }
            else if (distanceToPlayer <= enemyContext.EnemyController.DetectionRange && enemyContext.EnemyController.CanSeePlayer()) {
                return EEnemyStates.Chase;
            }
            else {
                return EEnemyStates.Idle;
            }
        }

        return EEnemyStates.Attack;
    }

    private void PerformAttack() {
        Vector3 attackPosition = enemyContext.EnemyController.transform.position + enemyContext.EnemyController.transform.forward * (attackData.attackRange * 0.5f);
        Collider[] hitColliders = Physics.OverlapSphere(attackPosition, attackData.attackRange, attackData.attackLayerMask);

        foreach (Collider hitCollider in hitColliders) {
            if (hitCollider.CompareTag("Player")) {
                Vector3 knockbackDirection = (hitCollider.transform.position - enemyContext.EnemyController.transform.position).normalized;
                knockbackDirection.y = 0f;

                if (hitCollider.TryGetComponent(out Rigidbody playerRb)) {
                    playerRb.AddForce(knockbackDirection * attackData.knockbackForce, ForceMode.Impulse);
                }

                Debug.Log($"Enemy dealt {attackData.attackDamage} damage to player!");
                break;
            }
        }

#if UNITY_EDITOR
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackData.attackRange);
#endif
    }
}