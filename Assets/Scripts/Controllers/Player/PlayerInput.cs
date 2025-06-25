using UnityEngine;

public class PlayerInput : MonoBehaviour {
    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 100f;

    // Input properties
    public Vector2 MovementInput { get; private set; }
    public Vector2 MouseInput { get; private set; }
    public bool IsMoving => MovementInput.magnitude > 0.1f;
    public bool InteractPressed { get; private set; }

    // Jump input properties
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool JumpReleased { get; private set; }

    // Settings
    public float MouseSensitivity => mouseSensitivity;

    private void Update() {
        // Get movement input
        MovementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Get mouse input
        MouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Get interact input
        InteractPressed = Input.GetKeyDown(KeyCode.E);

        // Get jump input
        JumpPressed = Input.GetKeyDown(KeyCode.Space);
        JumpHeld = Input.GetKey(KeyCode.Space);
        JumpReleased = Input.GetKeyUp(KeyCode.Space);
    }
}