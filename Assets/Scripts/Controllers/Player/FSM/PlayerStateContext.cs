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
        this.gravity = 20f;
        this.maxFallSpeed = 15f;
    }

    public void SetPlayerController(PlayerController controller) {
        this.playerController = controller;
    }

    public void SetGravitySettings(float gravity, float maxFallSpeed) {
        this.gravity = gravity;
        this.maxFallSpeed = maxFallSpeed;
    }

    public void ApplyGravity() {
        Vector3 gravityForce = Vector3.down * gravity * rb.mass;
        rb.AddForce(gravityForce, ForceMode.Force);

        Vector3 velocity = rb.velocity;
        if (velocity.y < -maxFallSpeed) {
            velocity.y = -maxFallSpeed;
            rb.velocity = velocity;
        }
    }

    public bool IsGrounded(float checkDistance = 0.1f, LayerMask groundMask = default) {
        if (groundMask == default) groundMask = 1; // Default to layer 0

        Vector3 rayOrigin = rb.position + Vector3.up * 0.1f;
        Ray ray = new Ray(rayOrigin, Vector3.down);

        bool isGrounded = Physics.Raycast(ray, checkDistance + 0.1f, groundMask);

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
        rb.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxAngle, maxAngle);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0f);
    }

    public void HandleDetectControllable() {
#if UNITY_EDITOR
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * detectionRange, Color.red);
#endif

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

    public bool CanMove(Vector3 moveDir, float maxAngleMovement) {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return true;

        Vector3 relativePos = GetMapPos();
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
        float angle = Vector3.Angle(normal, Vector3.up);

        float currentHeight = terrain.SampleHeight(rb.position);
        float nextHeight = terrain.SampleHeight(rb.position + moveDir * 5);

        if (angle > maxAngleMovement && nextHeight > currentHeight)
            return false;
        return true;
    }

    public Vector3 GetMapPos() {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return Vector3.zero;

        Vector3 pos = rb.position;
        return new Vector3((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                           0,
                           (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z);
    }
}