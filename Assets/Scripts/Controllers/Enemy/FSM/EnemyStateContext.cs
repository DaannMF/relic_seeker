using UnityEngine;

public class EnemyStateContext : BaseStateContext<EEnemyStates> {
    private EnemyController enemyController;
    private int currentPatrolIndex;
    private float stateTimer;

    public EnemyController EnemyController => enemyController;
    public float StateTimer { get => stateTimer; set => stateTimer = value; }

    public EnemyStateContext(Animator animator, EnemyController enemyController) {
        this._animator = animator;
        this.enemyController = enemyController;
        this.currentPatrolIndex = 0;
        this.stateTimer = 0f;
    }

    public Vector3 GetNextPatrolPoint() {
        if (enemyController.PatrolPoints == null || enemyController.PatrolPoints.Length == 0) {
            return enemyController.transform.position;
        }

        currentPatrolIndex = (currentPatrolIndex + 1) % enemyController.PatrolPoints.Length;
        return enemyController.PatrolPoints[currentPatrolIndex].position;
    }

    public void UpdateStateTimer() {
        stateTimer += Time.deltaTime;
    }

    public void ResetStateTimer() {
        stateTimer = 0f;
    }
}