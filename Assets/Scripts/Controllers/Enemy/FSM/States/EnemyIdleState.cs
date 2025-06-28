using UnityEngine;

public class EnemyIdleState : BaseState<EEnemyStates> {
    private EnemyStateContext enemyContext;
    private EnemyIdleSO idleData;

    public EnemyIdleState(EnemyStateContext context, EnemyIdleSO stateData) : base(EEnemyStates.Idle, stateData, context) {
        enemyContext = context;
        idleData = stateData;
    }

    public override void Enter() {
        enemyContext.Animator.SetBool("isIdle", true);
        enemyContext.EnemyController.StopMovement();
        enemyContext.ResetStateTimer();

        if (Random.value < idleData.lookAroundChance) {
            enemyContext.Animator.SetTrigger("lookAround");
        }
    }

    public override void Update() {
        enemyContext.UpdateStateTimer();
    }

    public override void Exit() {
        enemyContext.Animator.SetBool("isIdle", false);
    }

    public override EEnemyStates GetNextState() {
        if (enemyContext.EnemyController.CanSeePlayer()) {
            return EEnemyStates.Chase;
        }

        if (enemyContext.StateTimer >= idleData.idleTime) {
            if (enemyContext.EnemyController.PatrolPoints != null && enemyContext.EnemyController.PatrolPoints.Length > 0) {
                return EEnemyStates.Patrol;
            }
        }

        return EEnemyStates.Idle;
    }
}