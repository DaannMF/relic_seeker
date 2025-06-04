using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    private Rigidbody rb;
    [SerializeField] private float maxAngleMovement = 30f;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotation")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float maxAngle = 45f;
    private float rotationY = 0f;
    private float rotationX = 0f;

    [Header("Controllable")]
    [SerializeField] private float detectionRange = 6f;


    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        HandleRotation();
        HandleMovement();
        HandleDetectControllable();
    }

    public void ResetCameraPosition() {
        cameraTransform.forward = transform.forward;
    }

    private void HandleRotation() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotationY += mouseX;
        pivot.rotation = Quaternion.Euler(0f, rotationY, 0f);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxAngle, maxAngle);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0f);
    }

    private void HandleMovement() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;
        Vector3 velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);

        if (CanMove(moveDir)) {
            // PlayAudio();
            rb.velocity = velocity;
        }
    }

    private bool CanMove(Vector3 moveDir) {
        Terrain terrain = Terrain.activeTerrain;
        Vector3 relativePos = GetMapPos();
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
        float angle = Vector3.Angle(normal, Vector3.up);

        float currentHeight = terrain.SampleHeight(rb.position);
        float nextHeight = terrain.SampleHeight(rb.position + moveDir * 5);

        if (angle > maxAngleMovement && nextHeight > currentHeight)
            return false;
        return true;
    }

    private Vector3 GetMapPos() {
        Vector3 pos = rb.position;
        Terrain terrain = Terrain.activeTerrain;

        return new Vector3((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                           0,
                           (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z);
    }

    private void HandleDetectControllable() {
        DrawDebugRay();
        if (Input.GetKeyDown(KeyCode.E)) {
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, detectionRange)) {
                if (hit.collider.transform.parent.TryGetComponent(out Controllable controllable)) {
                    controllable.ControlEntity(this);
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
