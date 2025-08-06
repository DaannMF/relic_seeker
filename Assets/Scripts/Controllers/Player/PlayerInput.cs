using UnityEngine;

public class PlayerInput : MonoBehaviour {
    // Input properties
    public Vector2 MovementInput { get; private set; }
    public Vector2 MouseInput { get; private set; }
    public bool IsMoving => MovementInput.magnitude > 0.1f;
    public bool InteractPressed { get; private set; }

    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool JumpReleased { get; private set; }

    public bool RunPressed { get; private set; }
    public bool RunHeld { get; private set; }
    public bool RunReleased { get; private set; }

    public bool ReturnToPlayerPressed { get; private set; }

    public bool PausePressed { get; private set; }

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

        // Get run input (Shift key)
        RunPressed = Input.GetKeyDown(KeyCode.LeftShift);
        RunHeld = Input.GetKey(KeyCode.LeftShift);
        RunReleased = Input.GetKeyUp(KeyCode.LeftShift);

        // Get return to player input (1 key)
        ReturnToPlayerPressed = Input.GetKeyDown(KeyCode.Alpha1);

        // Get pause input (Esc key)
        PausePressed = Input.GetKeyDown(KeyCode.Escape);

        if (PausePressed) {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (!currentScene.Contains("MainMenu")) {
                GameEvents.OnTogglePauseRequested?.Invoke();
            }
        }
    }
}