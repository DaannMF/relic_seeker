using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteriorExit : MonoBehaviour {
    [Header("Exit Settings")]
    [SerializeField] private bool autoExit = false;

    private bool playerInRange = false;
    private PlayerController currentPlayer;

    private void Start() {
        Collider col = GetComponent<Collider>();
        if (col != null) {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = true;
            currentPlayer = player;

            if (autoExit) {
                ExitInterior();
            }
            else {
                UIEvents.OnPromptShow.Invoke("Press E to exit");
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = false;
            currentPlayer = null;
            UIEvents.OnPromptHide.Invoke();
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

    private void ExitInterior() {
        if (GameSceneManager.Instance != null) {
            UIEvents.OnPromptHide.Invoke();
            GameSceneManager.Instance.ExitInterior();
        }
        else {
            Debug.LogWarning("GameSceneManager not found!");
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
}