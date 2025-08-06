using UnityEngine;
using UnityEngine.Assertions;

public class DogController : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] private float walkRadius = 10f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundMask = 1;
    [SerializeField] private float groundCheckDistance = 2f;

    [Header("Player Detection")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private LayerMask playerMask = 1;

    [Header("Audio Settings")]
    [SerializeField] private Transform audioSourcePosition;
    [SerializeField] private string barkAudioClip = "Dog_Bark";

    private Rigidbody _rb;
    private Vector3 _startPosition;
    private Transform _player;

    public float WalkRadius => walkRadius;
    public Vector3 StartPosition => _startPosition;
    public LayerMask GroundMask => groundMask;

    private void Awake() {
        _rb = transform.parent.GetComponentInChildren<Rigidbody>();
        _startPosition = transform.position;

        ValidateRequiredComponents();
    }

    private void Start() {
        _player = FindObjectOfType<PlayerController>()?.transform;
    }

    private void ValidateRequiredComponents() {
        Assert.IsNotNull(_rb, "Rigidbody is not assigned in DogController.");
    }

    public void HandleMovement(Vector3 targetPosition, float speed) {
        Vector3 direction = (targetPosition - _rb.position);
        float distanceToTarget = direction.magnitude;

        if (distanceToTarget < 0.5f) {
            StopMovement();
            return;
        }

        direction = direction.normalized;
        direction.y = 0f;

        _rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.Acceleration);

        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.Slerp(_rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(newRotation);
        }
    }

    public void StopMovement() {
        Vector3 velocity = _rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        _rb.velocity = velocity;
    }

    public bool IsAtPosition(Vector3 targetPosition, float threshold = 1f) {
        Vector3 horizontalDistance = targetPosition - _rb.position;
        horizontalDistance.y = 0f;
        return horizontalDistance.magnitude <= threshold;
    }

    public bool IsWithinRadius(Vector3 position) {
        Vector3 distance = position - _startPosition;
        distance.y = 0f;
        return distance.magnitude <= walkRadius;
    }

    public void PlayBarkSound() {
        if (audioSourcePosition != null && !string.IsNullOrEmpty(barkAudioClip)) {
            AudioEvents.OnPlayAudio3D?.Invoke(barkAudioClip, AudioType.SFX, audioSourcePosition.position);
        }
    }

    public bool CanSeePlayer() {
        if (_player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        return distanceToPlayer <= detectionRange;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Vector3 center = Application.isPlaying ? _startPosition : transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, walkRadius);

        Gizmos.color = Color.blue;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f + transform.forward * 0.5f;
        Gizmos.DrawRay(rayOrigin, Vector3.down * groundCheckDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
#endif
}