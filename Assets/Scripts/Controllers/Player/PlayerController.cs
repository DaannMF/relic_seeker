using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : MonoBehaviour {
    [Header("Player Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float mouseSensitivity = 120f;
    [SerializeField] private float maxCameraAngle = 80f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform pivot;

    private PlayerInput input;
    private Rigidbody rb;
    private float rotationY;
    private float rotationX;
    private bool isInInterior = false;
    private Controllable currentDetectedControllable;

    public Rigidbody Rb => rb;
    public bool IsInInterior => isInInterior;

    private void Awake() {
        rb = transform.parent.GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();

        ValidateRequiredComponents();
    }

    private void Start() {
        rotationY = rb.transform.eulerAngles.y;
        rotationX = cameraTransform.localEulerAngles.x;
    }

    private void Update() {
        HandleCamera();
        HandleDetectControllable();
        HandleReturnToPlayer();
    }

    private void ValidateRequiredComponents() {
        Assert.IsNotNull(cameraTransform, "Camera Transform is not assigned in PlayerController.");
        Assert.IsNotNull(pivot, "Pivot Transform is not assigned in PlayerController.");
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * detectionRange);
    }

    private void HandleCamera() {

        Vector2 mouseInput = input.MouseInput;
        float mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;

        rotationY += mouseX;
        pivot.rotation = Quaternion.Euler(0f, rotationY, 0f);
        rb.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxCameraAngle, maxCameraAngle);
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0f);
    }

    public void HandleDetectControllable() {
        int layer = LayerMask.GetMask("Controllable");
        Controllable detectedControllable = null;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, detectionRange, layer)) {
            if (hit.collider.transform.parent.TryGetComponent(out Controllable controllable)) {
                detectedControllable = controllable;

                // If this is a new controllable, show prompt
                if (currentDetectedControllable != detectedControllable) {
                    if (currentDetectedControllable != null) {
                        currentDetectedControllable.OnStopLooking();
                    }

                    currentDetectedControllable = detectedControllable;
                    controllable.OnStartLooking();
                }

                // Handle interaction
                if (input.InteractPressed) {
                    controllable.OnInteract(this);
                }
            }
        }

        // If no controllable detected but we had one before, hide prompt
        if (detectedControllable == null && currentDetectedControllable != null) {
            currentDetectedControllable.OnStopLooking();
            currentDetectedControllable = null;
        }
    }

    private void HandleReturnToPlayer() {
        if (input.ReturnToPlayerPressed) {
            Controllable.ReturnToOriginalPlayer(this);
        }
    }

    public Vector3 GetCameraForward() {
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    public Vector3 GetCameraRight() {
        Vector3 right = Camera.main.transform.right;
        right.y = 0f;
        return right.normalized;
    }

    public void RefreshReferences() {
        rb = transform.parent.GetComponent<Rigidbody>();

        // Reset rotation variables with new entity's rotation
        if (rb != null) {
            rotationY = rb.transform.eulerAngles.y;
            rotationX = cameraTransform.localEulerAngles.x;

            // Handle case where rotationX might be > 180 (Unity's angle representation)
            if (rotationX > 180f) {
                rotationX -= 360f;
            }
        }

        // Clear any detected controllable when switching entities
        if (currentDetectedControllable != null) {
            currentDetectedControllable.OnStopLooking();
            currentDetectedControllable = null;
        }
    }

    public void SetCameraPosition(Vector3 newPosition) {
        cameraTransform.localPosition = newPosition;
    }

    public void SetIsInInterior(bool isInInterior) {
        this.isInInterior = isInInterior;
    }

    private void OnDisable() {
        // Clear any detected controllable when player is disabled
        if (currentDetectedControllable != null) {
            currentDetectedControllable.OnStopLooking();
            currentDetectedControllable = null;
        }
    }
}
