using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteriorExit : MonoBehaviour {
    [Header("Exit Settings")]
    [SerializeField] private bool autoExit = false;

    [Header("Outline Settings")]
    [SerializeField] private OutlineController outlineController;

    private bool playerInRange = false;
    private PlayerController currentPlayer;

    private void Start() {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        if (outlineController == null)
            outlineController = GetComponentInChildren<OutlineController>();
    }

    private void OnTriggerEnter(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = true;
            currentPlayer = player;

            if (autoExit)
                ExitInterior();
            else
                UpdatePrompt();

            ShowOutline();
        }
    }

    private void OnTriggerExit(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = false;
            currentPlayer = null;
            UIEvents.OnPromptHide.Invoke();

            HideOutline();
        }
    }

    private void Update() {
        if (playerInRange && currentPlayer != null && !autoExit) {
            PlayerInput input = currentPlayer.GetComponent<PlayerInput>();
            if (input != null && input.InteractPressed) {
                ExitInterior();
            }
        }
    }

    private void UpdatePrompt() {
        if (!playerInRange) return;

        if (!IsOriginalPlayer()) {
            UIEvents.OnPromptShow?.Invoke("Can't exit while controlling entity");
            return;
        }

        UIEvents.OnPromptShow?.Invoke("Press E to exit");
    }

    private void ExitInterior() {
        if (!IsOriginalPlayer())
            return;

        if (GameSceneManager.Instance != null) {
            UIEvents.OnPromptHide.Invoke();
            GameSceneManager.Instance.ExitInterior();
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);

        Vector3 forward = transform.forward;
        Vector3 arrowStart = transform.position;
        Vector3 arrowEnd = transform.position + forward * 1f;

        Gizmos.DrawLine(arrowStart, arrowEnd);
        Gizmos.DrawLine(arrowEnd, arrowEnd - forward * 0.3f + transform.right * 0.2f);
        Gizmos.DrawLine(arrowEnd, arrowEnd - forward * 0.3f - transform.right * 0.2f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f,
            $"Interior Exit\nAuto Exit: {(autoExit ? "Yes" : "No")}");
#endif
    }

    private bool IsOriginalPlayer() {
        if (currentPlayer == null) return false;

        Controllable controllable = currentPlayer.transform.parent.GetComponent<Controllable>();
        return controllable != null && controllable.IsOriginalPlayer();
    }

    private void ShowOutline() {
        if (outlineController != null)
            outlineController.ShowOutline();
    }

    private void HideOutline() {
        if (outlineController != null)
            outlineController.HideOutline();
    }
}