using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteriorEntrance : MonoBehaviour {
    [Header("Interior Settings")]
    [SerializeField] private string interiorSceneName;
    [SerializeField] private string spawnPointID = ""; // Optional specific spawn point ID

    [Header("Key Requirements")]
    [SerializeField] private int requiredKeys = 0;

    private bool playerInRange = false;
    private PlayerController currentPlayer;
    private int playerCurrentKeys = 0;

    private void Start() {
        // Ensure the collider is set as trigger
        Collider col = GetComponent<Collider>();
        if (col != null) {
            col.isTrigger = true;
        }

        // Subscribe to key count changes
        InventoryEvents.OnKeyCountChanged += OnKeyCountChanged;
    }

    private void OnDestroy() {
        // Unsubscribe from events
        InventoryEvents.OnKeyCountChanged -= OnKeyCountChanged;
    }

    private void OnTriggerEnter(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = true;
            currentPlayer = player;

            // Get current key count and update prompt
            InventoryEvents.OnGetKeyCount?.Invoke(OnKeyCountReceived);
        }
    }

    private void OnTriggerExit(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = false;
            currentPlayer = null;
            UIEvents.OnPromptHide?.Invoke();
        }
    }

    private void Update() {
        if (playerInRange && currentPlayer != null) {
            // Check for interaction input
            PlayerInput input = currentPlayer.GetComponent<PlayerInput>();
            if (input != null && input.InteractPressed) {
                TryEnterInterior();
            }
        }
    }

    private void OnKeyCountReceived(int keyCount) {
        playerCurrentKeys = keyCount;
        UpdatePrompt();
    }

    private void OnKeyCountChanged(int newKeyCount) {
        if (playerInRange) {
            playerCurrentKeys = newKeyCount;
            UpdatePrompt();
        }
    }

    private void UpdatePrompt() {
        if (!playerInRange) return;

        if (requiredKeys == 0) {
            // No keys required
            UIEvents.OnPromptShow?.Invoke("Press E to enter");
        }
        else if (playerCurrentKeys >= requiredKeys) {
            // Has enough keys - show access level
            UIEvents.OnPromptShow?.Invoke($"Press E to enter (Access Level {requiredKeys})");
        }
        else {
            // Not enough keys - show requirement
            UIEvents.OnPromptShow?.Invoke($"Need {requiredKeys} master keys (You have {playerCurrentKeys})");
        }
    }

    private void TryEnterInterior() {
        if (requiredKeys > 0 && playerCurrentKeys < requiredKeys) {
            // Not enough keys - could add error feedback here
            return;
        }

        // Has enough keys or no keys required
        if (GameSceneManager.Instance != null && !string.IsNullOrEmpty(interiorSceneName)) {
            UIEvents.OnPromptHide?.Invoke();
            GameSceneManager.Instance.LoadInteriorScene(interiorSceneName, spawnPointID);
        }
        else {
            Debug.LogWarning("GameSceneManager not found or scene name is empty!");
        }
    }

    private void OnDrawGizmosSelected() {
        // Draw entrance indicator with different colors based on access level
        Color gizmoColor = requiredKeys switch {
            0 => Color.cyan,    // Free access
            1 => Color.yellow,  // Level 1 access
            2 => Color.red,     // Level 2 access
            _ => Color.magenta  // Higher levels
        };

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);

        // Draw arrow pointing into the entrance
        Vector3 forward = transform.forward;
        Vector3 arrowStart = transform.position - forward * 1f;
        Vector3 arrowEnd = transform.position;

        Gizmos.DrawLine(arrowStart, arrowEnd);
        Gizmos.DrawLine(arrowEnd, arrowEnd - forward * 0.3f + transform.right * 0.2f);
        Gizmos.DrawLine(arrowEnd, arrowEnd - forward * 0.3f - transform.right * 0.2f);

        // Label with access level information
#if UNITY_EDITOR
        string accessLevel = requiredKeys switch {
            0 => "Free Access",
            1 => "Level 1 Access",
            2 => "Level 2 Access",
            _ => $"Level {requiredKeys} Access"
        };

        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f,
            $"Interior Entrance\nScene: {interiorSceneName}\nSpawn ID: {(string.IsNullOrEmpty(spawnPointID) ? "Default" : spawnPointID)}\n{accessLevel}");
#endif
    }
}