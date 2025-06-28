using UnityEngine;

public class EnemyChaseState : BaseState<EEnemyStates> {
    private EnemyStateContext enemyContext;
    private EnemyChaseSO chaseData;
    private Vector3 lastKnownPlayerPosition;
    private float timeSincePlayerSeen;

    public EnemyChaseState(EnemyStateContext context, EnemyChaseSO stateData) : base(EEnemyStates.Chase, stateData, context) {
        enemyContext = context;
        chaseData = stateData;
    }

    public override void Enter() {
        enemyContext.Animator.SetBool("isChasing", true);
        enemyContext.ResetStateTimer();
        timeSincePlayerSeen = 0f;

        if (enemyContext.EnemyController.Player != null) {
            lastKnownPlayerPosition = enemyContext.EnemyController.Player.position;
        }
    }

    public override void Update() {
        enemyContext.UpdateStateTimer();

        if (enemyContext.EnemyController.CanSeePlayer()) {
            timeSincePlayerSeen = 0f;
            lastKnownPlayerPosition = enemyContext.EnemyController.Player.position;
        }
        else {
            timeSincePlayerSeen += Time.deltaTime;
        }
    }

    public override void FixedUpdate() {
        Vector3 targetPosition = enemyContext.EnemyController.CanSeePlayer() ?
            enemyContext.EnemyController.Player.position : lastKnownPlayerPosition;

        enemyContext.EnemyController.HandleMovement(targetPosition, chaseData.chaseSpeed);
    }

    public override void Exit() {
        enemyContext.Animator.SetBool("isChasing", false);
        enemyContext.EnemyController.StopMovement();
    }

    public override EEnemyStates GetNextState() {
        float distanceToPlayer = enemyContext.EnemyController.DistanceToPlayer();

        if (distanceToPlayer <= chaseData.attackDistance && enemyContext.EnemyController.CanSeePlayer()) {
            return EEnemyStates.Attack;
        }

        if (timeSincePlayerSeen >= chaseData.losePlayerTime || distanceToPlayer > chaseData.maxChaseDistance) {
            return EEnemyStates.Idle;
        }

        return EEnemyStates.Chase;
    }
}