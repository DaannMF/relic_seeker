using UnityEngine;

public class EnemyPatrolState : BaseState<EEnemyStates> {
    private EnemyStateContext enemyContext;
    private EnemyPatrolSO patrolData;
    private Vector3 targetPatrolPoint;
    private bool waitingAtPoint;

    public EnemyPatrolState(EnemyStateContext context, EnemyPatrolSO stateData) : base(EEnemyStates.Patrol, stateData, context) {
        enemyContext = context;
        patrolData = stateData;
    }

    public override void Enter() {
        enemyContext.Animator.SetBool("isWalking", true);
        enemyContext.ResetStateTimer();
        targetPatrolPoint = enemyContext.GetNextPatrolPoint();
        waitingAtPoint = false;
    }

    public override void Update() {
        enemyContext.UpdateStateTimer();

        if (Vector3.Distance(enemyContext.EnemyController.transform.position, enemyContext.EnemyController.Player.position) <= patrolData.detectionWhilePatrolling) {
            if (enemyContext.EnemyController.CanSeePlayer()) {
                return;
            }
        }
    }

    public override void FixedUpdate() {
        if (!waitingAtPoint) {
            enemyContext.EnemyController.HandleMovement(targetPatrolPoint, patrolData.patrolSpeed);

            if (enemyContext.EnemyController.IsAtPosition(targetPatrolPoint, 1f)) {
                waitingAtPoint = true;
                enemyContext.EnemyController.StopMovement();
                enemyContext.ResetStateTimer();
            }
        }
    }

    public override void Exit() {
        enemyContext.Animator.SetBool("isWalking", false);
        enemyContext.EnemyController.StopMovement();
    }

    public override EEnemyStates GetNextState() {
        if (enemyContext.EnemyController.CanSeePlayer()) {
            return EEnemyStates.Chase;
        }

        if (waitingAtPoint && enemyContext.StateTimer >= patrolData.waitTimeAtPoint) {
            return EEnemyStates.Idle;
        }

        return EEnemyStates.Patrol;
    }
}