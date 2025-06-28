using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Player Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxCameraAngle = 80f;
    [SerializeField] private float detectionRange = 5f;

    private PlayerInput input;
    private Rigidbody rb;
    private Transform pivot;
    private Transform cameraTransform;

    private float rotationY;
    private float rotationX;

    public float MouseSensitivity => mouseSensitivity;
    public float MaxCameraAngle => maxCameraAngle;
    public float DetectionRange => detectionRange;
    public Rigidbody Rb => rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();

        pivot = transform.Find("Pivot");
        cameraTransform = pivot.Find("Main Camera").transform;
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void HandleRotation() {
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
#if UNITY_EDITOR
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * detectionRange, Color.red);
#endif

        if (input.InteractPressed) {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, detectionRange)) {
                if (hit.collider.transform.parent.TryGetComponent(out Controllable controllable)) {
                    controllable.ControlEntity(this);
                }
                else {
                    Debug.Log("No controllable entity found.");
                }
            }
        }
    }

    public Vector3 GetCameraForward() {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    public Vector3 GetCameraRight() {
        Vector3 right = cameraTransform.right;
        right.y = 0f;
        return right.normalized;
    }
}
