public class EnemyIdleState : BaseState<EEnemyStates> {
    private new EnemyStateContext Context => base.Context as EnemyStateContext;
    private new EnemyIdleSO StateData => base.StateData as EnemyIdleSO;

    public EnemyIdleState(EnemyStateContext context, EnemyIdleSO stateData) :
        base(EEnemyStates.Idle, stateData, context) {
    }

    public override void OnEnter() {
        Context.EnemyController.StopMovement();
        Context.ResetStateTimer();
    }

    public override void Update() {
        Context.UpdateStateTimer();
    }

    public override EEnemyStates GetNextState() {
        if (Context.EnemyController.CanSeePlayer()) {
            return EEnemyStates.Chase;
        }

        if (Context.StateTimer >= StateData.idleTime) {
            if (Context.EnemyController.PatrolPoints != null && Context.EnemyController.PatrolPoints.Length > 0) {
                return EEnemyStates.Patrol;
            }
        }

        return EEnemyStates.Idle;
    }
}