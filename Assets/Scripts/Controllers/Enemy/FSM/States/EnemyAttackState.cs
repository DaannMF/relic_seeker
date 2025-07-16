using UnityEngine;

public class EnemyAttackState : BaseState<EEnemyStates> {
    private new EnemyStateContext Context => base.Context as EnemyStateContext;
    private new EnemyAttackSO StateData => base.StateData as EnemyAttackSO;
    private bool hasAttacked;

    public EnemyAttackState(EnemyStateContext context, EnemyAttackSO stateData) :
        base(EEnemyStates.Attack, stateData, context) {
    }

    public override void OnEnter() {
        Context.EnemyController.StopMovement();
        Context.ResetStateTimer();
        hasAttacked = false;

        Vector3 directionToPlayer = (Context.EnemyController.Player.position - Context.EnemyController.transform.position).normalized;
        directionToPlayer.y = 0f;
        Context.EnemyController.transform.rotation = Quaternion.LookRotation(directionToPlayer);
    }

    public override void Update() {
        Context.UpdateStateTimer();

        if (!hasAttacked && Context.StateTimer >= 0.5f) {
            PerformAttack();
            hasAttacked = true;
        }
    }

    public override void Exit() {
        hasAttacked = false;
    }

    public override EEnemyStates GetNextState() {
        if (Context.StateTimer >= StateData.attackCooldown) {
            float distanceToPlayer = Context.EnemyController.DistanceToPlayer();

            if (distanceToPlayer <= StateData.attackRange && Context.EnemyController.CanSeePlayer()) {
                return EEnemyStates.Attack;
            }
            else if (distanceToPlayer <= Context.EnemyController.DetectionRange && Context.EnemyController.CanSeePlayer()) {
                return EEnemyStates.Chase;
            }
            else {
                return EEnemyStates.Idle;
            }
        }

        return EEnemyStates.Attack;
    }

    private void PerformAttack() {
        Vector3 attackPosition = Context.EnemyController.transform.position + Context.EnemyController.transform.forward * (StateData.attackRange * 0.5f);
        Collider[] hitColliders = Physics.OverlapSphere(attackPosition, StateData.attackRange, StateData.attackLayerMask);

        foreach (Collider hitCollider in hitColliders) {
            if (hitCollider.CompareTag("Player")) {
                Vector3 knockbackDirection = (hitCollider.transform.position - Context.EnemyController.transform.position).normalized;
                knockbackDirection.y = 0f;

                if (hitCollider.TryGetComponent(out Rigidbody playerRb)) {
                    playerRb.AddForce(knockbackDirection * StateData.knockbackForce, ForceMode.Impulse);
                }

                Debug.Log($"Enemy dealt {StateData.attackDamage} damage to player!");
                break;
            }
        }

#if UNITY_EDITOR
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, StateData.attackRange);
#endif
    }
}