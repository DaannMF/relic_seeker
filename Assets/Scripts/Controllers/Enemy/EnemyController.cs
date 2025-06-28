using UnityEngine;

public class EnemyController : MonoBehaviour {
    [Header("Enemy Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask playerMask = 1;
    [SerializeField] private Transform patrolPointsParent;

    private Rigidbody rb;
    private Transform player;
    private Transform[] patrolPoints;

    public Transform Player => player;
    public Transform[] PatrolPoints => patrolPoints;
    public float DetectionRange => detectionRange;
    public float AttackRange => attackRange;
    public LayerMask PlayerMask => playerMask;

    private void Awake() {
        rb = GetComponent<Rigidbody>();

        if (patrolPointsParent != null) {
            patrolPoints = new Transform[patrolPointsParent.childCount];
            for (int i = 0; i < patrolPointsParent.childCount; i++) {
                patrolPoints[i] = patrolPointsParent.GetChild(i);
            }
        }
    }

    private void Start() {
        player = FindObjectOfType<PlayerController>()?.transform;
    }

    public bool CanSeePlayer() {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange) {
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit hit, detectionRange, ~playerMask)) {
                return hit.transform == player;
            }
        }
        return false;
    }

    public float DistanceToPlayer() {
        if (player == null) return float.MaxValue;
        return Vector3.Distance(transform.position, player.position);
    }

    public void HandleMovement(Vector3 targetPosition, float speed) {
        Vector3 direction = (targetPosition - rb.position).normalized;
        direction.y = 0f;

        Vector3 moveVelocity = direction * speed;
        moveVelocity.y = rb.velocity.y;

        rb.velocity = moveVelocity;

        if (direction != Vector3.zero) {
            rb.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void StopMovement() {
        Vector3 velocity = rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        rb.velocity = velocity;
    }

    public bool IsAtPosition(Vector3 targetPosition, float threshold = 1f) {
        Vector3 horizontalDistance = targetPosition - rb.position;
        horizontalDistance.y = 0f;
        return horizontalDistance.magnitude <= threshold;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}