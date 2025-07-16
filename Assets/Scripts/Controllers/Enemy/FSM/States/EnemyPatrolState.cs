using UnityEngine;

public class EnemyPatrolState : BaseState<EEnemyStates> {
    private new EnemyStateContext Context => base.Context as EnemyStateContext;
    private new EnemyPatrolSO StateData => base.StateData as EnemyPatrolSO;
    private Vector3 targetPatrolPoint;
    private bool waitingAtPoint;

    public EnemyPatrolState(EnemyStateContext context, EnemyPatrolSO stateData) :
        base(EEnemyStates.Patrol, stateData, context) {
    }

    public override void OnEnter() {
        Context.ResetStateTimer();
        targetPatrolPoint = Context.GetNextPatrolPoint();
        waitingAtPoint = false;
    }

    public override void Update() {
        Context.UpdateStateTimer();

        if (Vector3.Distance(Context.EnemyController.transform.position, Context.EnemyController.Player.position) <= StateData.detectionWhilePatrolling) {
            if (Context.EnemyController.CanSeePlayer()) {
                return;
            }
        }
    }

    public override void FixedUpdate() {
        if (!waitingAtPoint) {
            Context.EnemyController.HandleMovement(targetPatrolPoint, StateData.patrolSpeed);

            if (Context.EnemyController.IsAtPosition(targetPatrolPoint, 1f)) {
                waitingAtPoint = true;
                Context.EnemyController.StopMovement();
                Context.ResetStateTimer();
            }
        }
    }

    public override void Exit() {
        Context.EnemyController.StopMovement();
    }

    public override EEnemyStates GetNextState() {
        if (Context.EnemyController.CanSeePlayer()) {
            return EEnemyStates.Chase;
        }

        if (waitingAtPoint && Context.StateTimer >= StateData.waitTimeAtPoint) {
            return EEnemyStates.Idle;
        }

        return EEnemyStates.Patrol;
    }
}