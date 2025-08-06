using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Key : MonoBehaviour {
    [Header("Key Settings")]
    [SerializeField] private int keyValue = 1;
    [SerializeField] private bool autoPickup = true;
    [SerializeField] private GameObject visualRepresentation;

    [Header("Audio")]
    [SerializeField] private string pickupAudioClip = "Key_Pickup";

    [Header("Outline Settings")]
    [SerializeField] private OutlineController outlineController;

    private bool isCollected = false;
    private bool playerInRange = false;
    private PlayerController currentPlayer;

    private void Start() {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        if (visualRepresentation == null) {
            Transform visual = transform.Find("Visual");
            if (visual != null)
                visualRepresentation = visual.gameObject;
            else if (transform.childCount > 0)
                visualRepresentation = transform.GetChild(0).gameObject;
        }

        if (outlineController == null)
            outlineController = GetComponentInChildren<OutlineController>();
    }

    private void OnTriggerEnter(Collider other) {
        if (isCollected) return;

        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = true;
            currentPlayer = player;

            if (autoPickup)
                CollectKey();
            else
                UIEvents.OnPromptShow?.Invoke("Press E to pick up key");

            ShowOutline();
        }
    }

    private void OnTriggerExit(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = false;
            currentPlayer = null;

            if (!autoPickup)
                UIEvents.OnPromptHide?.Invoke();

            HideOutline();
        }
    }

    private void Update() {
        if (!autoPickup && playerInRange && currentPlayer != null && !isCollected) {
            PlayerInput input = currentPlayer.GetComponent<PlayerInput>();
            if (input != null && input.InteractPressed)
                CollectKey();
        }
    }

    private void CollectKey() {
        if (isCollected) return;

        isCollected = true;

        InventoryEvents.OnAddKeys?.Invoke(keyValue);

        if (!string.IsNullOrEmpty(pickupAudioClip))
            AudioEvents.OnPlayAudio?.Invoke(pickupAudioClip, AudioType.SFX);

        if (!autoPickup)
            UIEvents.OnPromptHide?.Invoke();

        if (visualRepresentation != null)
            visualRepresentation.SetActive(false);

        HideOutline();

        Destroy(gameObject);
    }

    private void ShowOutline() {
        if (outlineController != null && !isCollected)
            outlineController.ShowOutline();
    }

    private void HideOutline() {
        if (outlineController != null)
            outlineController.HideOutline();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.up * 1f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.2f,
            $"Key\nValue: {keyValue}\nAuto Pickup: {autoPickup}");
#endif
    }
}