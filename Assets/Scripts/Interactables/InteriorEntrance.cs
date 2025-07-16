using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteriorEntrance : MonoBehaviour {
    [Header("Interior Settings")]
    [SerializeField] private string interiorSceneName;
    [SerializeField] private string spawnPointID = ""; // Optional specific spawn point ID

    private bool playerInRange = false;
    private PlayerController currentPlayer;

    private void Start() {
        // Ensure the collider is set as trigger
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
            UIEvents.OnPromptShow?.Invoke("Press E to enter");
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
                EnterInterior();
            }
        }
    }

    private void EnterInterior() {
        if (GameSceneManager.Instance != null && !string.IsNullOrEmpty(interiorSceneName)) {
            UIEvents.OnPromptHide.Invoke();
            GameSceneManager.Instance.LoadInteriorScene(interiorSceneName, spawnPointID);
        }
        else {
            Debug.LogWarning("GameSceneManager not found or scene name is empty!");
        }
    }

    private void OnDrawGizmosSelected() {
        // Draw entrance indicator
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);

        // Draw arrow pointing into the entrance
        Vector3 forward = transform.forward;
        Vector3 arrowStart = transform.position - forward * 1f;
        Vector3 arrowEnd = transform.position;

        Gizmos.DrawLine(arrowStart, arrowEnd);
        Gizmos.DrawLine(arrowEnd, arrowEnd - forward * 0.3f + transform.right * 0.2f);
        Gizmos.DrawLine(arrowEnd, arrowEnd - forward * 0.3f - transform.right * 0.2f);

        // Label
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f,
            $"Interior Entrance\nScene: {interiorSceneName}\nSpawn ID: {(string.IsNullOrEmpty(spawnPointID) ? "Default" : spawnPointID)}");
#endif
    }
}