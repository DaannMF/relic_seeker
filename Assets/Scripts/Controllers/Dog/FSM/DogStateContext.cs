using UnityEngine;

public class DogStateContext : BaseStateContext<EDogStates> {
    private DogController dogController;
    private Vector3 targetPosition;
    private float stateTimer;
    private bool hasTarget;
    private bool isBarking;

    public DogController DogController => dogController;
    public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
    public float StateTimer { get => stateTimer; set => stateTimer = value; }
    public bool HasTarget { get => hasTarget; set => hasTarget = value; }
    public bool IsBarking { get => isBarking; set => isBarking = value; }

    public bool CanSeePlayer() {
        return dogController.CanSeePlayer();
    }

    public DogStateContext(Animator animator, DogController dogController) {
        this._animator = animator;
        this.dogController = dogController;
        this.targetPosition = Vector3.zero;
        this.stateTimer = 0f;
        this.hasTarget = false;
        this.isBarking = false;
    }

    public void UpdateStateTimer() {
        stateTimer += Time.deltaTime;
    }

    public void ResetStateTimer() {
        stateTimer = 0f;
    }

    public Vector3 GetRandomWalkPosition() {
        for (int attempts = 0; attempts < 10; attempts++) {
            float randomRadius = Random.Range(1f, dogController.WalkRadius * 0.7f);
            Vector3 randomDirection = Random.insideUnitCircle.normalized * randomRadius;
            Vector3 randomDirection3D = new Vector3(randomDirection.x, 0f, randomDirection.y);

            Vector3 potentialPosition = dogController.StartPosition + randomDirection3D;

            if (dogController.IsWithinRadius(potentialPosition) && IsValidWalkPosition(potentialPosition)) {
                return potentialPosition;
            }
        }

        Vector3 fallbackDirection = Random.insideUnitCircle.normalized * 2f;
        Vector3 fallbackPosition = dogController.transform.position + new Vector3(fallbackDirection.x, 0f, fallbackDirection.y);

        return dogController.IsWithinRadius(fallbackPosition) ? fallbackPosition : dogController.transform.position;
    }

    private bool IsValidWalkPosition(Vector3 position) {
        Vector3 rayOrigin = position + Vector3.up * 2f;
        return Physics.Raycast(rayOrigin, Vector3.down, 5f, dogController.GroundMask);
    }
}