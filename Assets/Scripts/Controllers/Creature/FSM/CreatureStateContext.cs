using UnityEngine;

public class CreatureStateContext : BaseStateContext<ECreatureStates> {
    private CreatureController creatureController;
    private Vector3 targetPosition;
    private float stateTimer;
    private bool hasTarget;

    public CreatureController CreatureController => creatureController;
    public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
    public float StateTimer { get => stateTimer; set => stateTimer = value; }
    public bool HasTarget { get => hasTarget; set => hasTarget = value; }

    public CreatureStateContext(Animator animator, CreatureController creatureController) {
        this._animator = animator;
        this.creatureController = creatureController;
        this.targetPosition = Vector3.zero;
        this.stateTimer = 0f;
        this.hasTarget = false;
    }

    public void UpdateStateTimer() {
        stateTimer += Time.deltaTime;
    }

    public void ResetStateTimer() {
        stateTimer = 0f;
    }

    public Vector3 GetRandomWalkPosition() {
        for (int attempts = 0; attempts < 10; attempts++) {
            // Generate a random position within a smaller radius for more controlled movement
            float randomRadius = Random.Range(1f, creatureController.WalkRadius * 0.7f);
            Vector3 randomDirection = Random.insideUnitCircle.normalized * randomRadius;
            Vector3 randomDirection3D = new Vector3(randomDirection.x, 0f, randomDirection.y);

            Vector3 potentialPosition = creatureController.StartPosition + randomDirection3D;

            if (creatureController.IsWithinRadius(potentialPosition) && IsValidWalkPosition(potentialPosition)) {
                return potentialPosition;
            }
        }

        // If no valid position found, return a position slightly away from current position
        Vector3 fallbackDirection = Random.insideUnitCircle.normalized * 2f;
        Vector3 fallbackPosition = creatureController.transform.position + new Vector3(fallbackDirection.x, 0f, fallbackDirection.y);

        return creatureController.IsWithinRadius(fallbackPosition) ? fallbackPosition : creatureController.transform.position;
    }

    private bool IsValidWalkPosition(Vector3 position) {
        Vector3 rayOrigin = position + Vector3.up * 2f;
        return Physics.Raycast(rayOrigin, Vector3.down, 5f, creatureController.GroundMask);
    }
}