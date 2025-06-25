using UnityEngine;

public class PlayerStateContext : BaseStateContext<EPlayerStates> {
    private Rigidbody rb;
    private PlayerInput input;
    private Animator animator;
    private Transform pivot;
    private Transform cameraTransform;
    private PlayerController playerController;

    // Rotation settings
    private float maxAngle;
    private float rotationY;
    private float rotationX;

    // Detection settings
    private float detectionRange;

    // Gravity settings (shared across states)
    private float gravity;
    private float maxFallSpeed;

    // Properties
    public Rigidbody Rb => rb;
    public PlayerInput Input => input;
    public Animator Animator => animator;
    public Transform Pivot => pivot;
    public Transform CameraTransform => cameraTransform;
    public PlayerController PlayerController => playerController;

    public float MaxAngle => maxAngle;
    public float DetectionRange => detectionRange;

    public float RotationY { get => rotationY; set => rotationY = value; }
    public float RotationX { get => rotationX; set => rotationX = value; }

    public PlayerStateContext(Rigidbody rb, PlayerInput input, Animator animator,
                            Transform pivot, Transform cameraTransform,
                            float maxAngle, float detectionRange,
                            PlayerController playerController = null) {
        this.rb = rb;
        this.input = input;
        this.animator = animator;
        this.pivot = pivot;
        this.cameraTransform = cameraTransform;
        this.maxAngle = maxAngle;
        this.detectionRange = detectionRange;
        this.playerController = playerController;
        this.rotationY = 0f;
        this.rotationX = 0f;

        // Default gravity settings (can be overridden by JumpSO)
        this.gravity = 20f;
        this.maxFallSpeed = 15f;
    }

    // Method to set the player controller reference after initialization
    public void SetPlayerController(PlayerController controller) {
        this.playerController = controller;
    }

    // Method to set gravity settings (called by JumpState)
    public void SetGravitySettings(float gravity, float maxFallSpeed) {
        this.gravity = gravity;
        this.maxFallSpeed = maxFallSpeed;
    }

    // Apply gravity - should be called in FixedUpdate of all states
    public void ApplyGravity() {
        // Apply custom gravity since rigidbody gravity is disabled
        Vector3 gravityForce = Vector3.down * gravity * rb.mass;
        rb.AddForce(gravityForce, ForceMode.Force);

        // Limit fall speed to prevent infinite acceleration
        Vector3 velocity = rb.velocity;
        if (velocity.y < -maxFallSpeed) {
            velocity.y = -maxFallSpeed;
            rb.velocity = velocity;
        }
    }

    // Helper method for ground detection (can be used by multiple states)
    public bool IsGrounded(float checkDistance = 0.1f, LayerMask groundMask = default) {
        if (groundMask == default) groundMask = 1; // Default to layer 0

        Vector3 rayOrigin = rb.position + Vector3.up * 0.1f;
        Ray ray = new Ray(rayOrigin, Vector3.down);

        bool isGrounded = Physics.Raycast(ray, checkDistance + 0.1f, groundMask);

        // Debug visualization
#if UNITY_EDITOR
        Color rayColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(rayOrigin, Vector3.down * (checkDistance + 0.1f), rayColor);
#endif

        return isGrounded;
    }

    public void HandleRotation() {
        Vector2 mouseInput = input.MouseInput;
        float mouseX = mouseInput.x * input.MouseSensitivity * Time.deltaTime;
        float mouseY = mouseInput.y * input.MouseSensitivity * Time.deltaTime;

        rotationY += mouseX;
        pivot.rotation = Quaternion.Euler(0f, rotationY, 0f);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxAngle, maxAngle);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0f);
    }

    public void HandleDetectControllable() {
        DrawDebugRay();
        if (input.InteractPressed) {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, detectionRange)) {
                if (hit.collider.transform.parent.TryGetComponent(out Controllable controllable)) {
                    controllable.ControlEntity(playerController);
                }
                else {
                    Debug.Log("No controllable entity found.");
                }
            }
        }
    }

    private void DrawDebugRay() {
        if (Debug.isDebugBuild) {
            Debug.DrawRay(cameraTransform.position, cameraTransform.forward * detectionRange, Color.red);
        }
    }
}