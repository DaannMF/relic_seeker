using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteriorEntrance : MonoBehaviour {
    [Header("Interior Settings")]
    [SerializeField] private string interiorSceneName;
    [SerializeField] private string spawnPointID = "";

    [Header("Key Requirements")]
    [SerializeField] private int requiredKeys = 0;

    [Header("Outline Settings")]
    [SerializeField] private OutlineController outlineController;

    private bool playerInRange = false;
    private PlayerController currentPlayer;
    private int playerCurrentKeys = 0;

    private void Start() {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        InventoryEvents.OnKeyCountChanged += OnKeyCountChanged;

        if (outlineController == null)
            outlineController = GetComponentInChildren<OutlineController>();
    }

    private void OnDestroy() {
        InventoryEvents.OnKeyCountChanged -= OnKeyCountChanged;
    }

    private void OnTriggerEnter(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = true;
            currentPlayer = player;

            InventoryEvents.OnGetKeyCount?.Invoke(OnKeyCountReceived);

            ShowOutline();
        }
    }

    private void OnTriggerExit(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = false;
            currentPlayer = null;
            UIEvents.OnPromptHide?.Invoke();

            HideOutline();
        }
    }

    private void Update() {
        if (playerInRange && currentPlayer != null) {
            PlayerInput input = currentPlayer.GetComponent<PlayerInput>();
            if (input != null && input.InteractPressed)
                TryEnterInterior();
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

        if (!IsOriginalPlayer()) {
            UIEvents.OnPromptShow?.Invoke("Can't enter while controlling entity");
            return;
        }

        if (requiredKeys == 0)
            UIEvents.OnPromptShow?.Invoke("Press E to enter");
        else if (playerCurrentKeys >= requiredKeys)
            UIEvents.OnPromptShow?.Invoke($"Press E to enter (Access Level {requiredKeys})");
        else
            UIEvents.OnPromptShow?.Invoke($"Need {requiredKeys} master keys (You have {playerCurrentKeys})");
    }

    private void TryEnterInterior() {
        if (!IsOriginalPlayer())
            return;

        if (requiredKeys > 0 && playerCurrentKeys < requiredKeys)
            return;

        if (GameSceneManager.Instance != null && !string.IsNullOrEmpty(interiorSceneName)) {
            UIEvents.OnPromptHide?.Invoke();
            GameSceneManager.Instance.LoadInteriorScene(interiorSceneName, spawnPointID);
        }
    }

    private void OnDrawGizmosSelected() {
        Color gizmoColor = requiredKeys switch {
            0 => Color.cyan,
            1 => Color.yellow,
            2 => Color.red,
            _ => Color.magenta
        };

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);

        Vector3 forward = transform.forward;
        Vector3 arrowStart = transform.position - forward * 1f;
        Vector3 arrowEnd = transform.position;

        Gizmos.DrawLine(arrowStart, arrowEnd);
        Gizmos.DrawLine(arrowEnd, arrowEnd - forward * 0.3f + transform.right * 0.2f);
        Gizmos.DrawLine(arrowEnd, arrowEnd - forward * 0.3f - transform.right * 0.2f);

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