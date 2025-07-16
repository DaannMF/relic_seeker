using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Key : MonoBehaviour {
    [Header("Key Settings")]
    [SerializeField] private int keyValue = 1;
    [SerializeField] private bool autoPickup = true;
    [SerializeField] private GameObject visualRepresentation;

    [Header("Audio")]
    [SerializeField] private string pickupAudioClip = "Key_Pickup";

    private bool isCollected = false;
    private bool playerInRange = false;
    private PlayerController currentPlayer;

    private void Start() {
        // Ensure the collider is set as trigger
        Collider col = GetComponent<Collider>();
        if (col != null) {
            col.isTrigger = true;
        }

        // If no visual representation assigned, try to find one
        if (visualRepresentation == null) {
            // Look for a child object named "Visual" or use the first child
            Transform visual = transform.Find("Visual");
            if (visual != null) {
                visualRepresentation = visual.gameObject;
            }
            else if (transform.childCount > 0) {
                visualRepresentation = transform.GetChild(0).gameObject;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (isCollected) return;

        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = true;
            currentPlayer = player;

            if (autoPickup) {
                CollectKey();
            }
            else {
                UIEvents.OnPromptShow?.Invoke("Press E to pick up key");
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = false;
            currentPlayer = null;

            if (!autoPickup) {
                UIEvents.OnPromptHide?.Invoke();
            }
        }
    }

    private void Update() {
        if (!autoPickup && playerInRange && currentPlayer != null && !isCollected) {
            PlayerInput input = currentPlayer.GetComponent<PlayerInput>();
            if (input != null && input.InteractPressed) {
                CollectKey();
            }
        }
    }

    private void CollectKey() {
        if (isCollected) return;

        isCollected = true;
        Debug.Log($"[Key] Collecting key with value: {keyValue}");

        // Add keys to inventory
        InventoryEvents.OnAddKeys?.Invoke(keyValue);
        Debug.Log($"[Key] Invoked OnAddKeys event with value: {keyValue}");

        // Play pickup sound
        if (!string.IsNullOrEmpty(pickupAudioClip)) {
            AudioEvents.OnPlayAudio?.Invoke(pickupAudioClip, AudioType.SFX);
        }

        // Hide prompt if showing
        if (!autoPickup) {
            UIEvents.OnPromptHide?.Invoke();
        }

        // Hide visual representation
        if (visualRepresentation != null) {
            visualRepresentation.SetActive(false);
        }

        // Disable the key (or destroy it after a delay)
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        // Draw key indicator
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        // Draw key value indicator
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.up * 1f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.2f,
            $"Key\nValue: {keyValue}\nAuto Pickup: {autoPickup}");
#endif
    }
}