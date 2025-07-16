using UnityEngine;

public class EnemyChaseState : BaseState<EEnemyStates> {
    private new EnemyStateContext Context => base.Context as EnemyStateContext;
    private new EnemyChaseSO StateData => base.StateData as EnemyChaseSO;
    private Vector3 lastKnownPlayerPosition;
    private float timeSincePlayerSeen;

    public EnemyChaseState(EnemyStateContext context, EnemyChaseSO stateData) :
        base(EEnemyStates.Chase, stateData, context) {
    }

    public override void OnEnter() {
        Context.ResetStateTimer();
        timeSincePlayerSeen = 0f;

        if (Context.EnemyController.Player != null) {
            lastKnownPlayerPosition = Context.EnemyController.Player.position;
        }
    }

    public override void Update() {
        Context.UpdateStateTimer();

        if (Context.EnemyController.CanSeePlayer()) {
            timeSincePlayerSeen = 0f;
            lastKnownPlayerPosition = Context.EnemyController.Player.position;
        }
        else {
            timeSincePlayerSeen += Time.deltaTime;
        }
    }

    public override void FixedUpdate() {
        Vector3 targetPosition = Context.EnemyController.CanSeePlayer() ?
            Context.EnemyController.Player.position : lastKnownPlayerPosition;

        Context.EnemyController.HandleMovement(targetPosition, StateData.chaseSpeed);
    }

    public override void Exit() {
        Context.EnemyController.StopMovement();
    }

    public override EEnemyStates GetNextState() {
        float distanceToPlayer = Context.EnemyController.DistanceToPlayer();

        if (distanceToPlayer <= StateData.attackDistance && Context.EnemyController.CanSeePlayer()) {
            return EEnemyStates.Attack;
        }

        if (timeSincePlayerSeen >= StateData.losePlayerTime || distanceToPlayer > StateData.maxChaseDistance) {
            return EEnemyStates.Idle;
        }

        return EEnemyStates.Chase;
    }
}