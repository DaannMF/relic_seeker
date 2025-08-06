using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Lever : MonoBehaviour {
    [Header("Lever Settings")]
    [SerializeField] private bool isActivated = false;
    [SerializeField] private bool canBeDeactivated = true;
    [SerializeField] private GameObject leverVisual;
    [SerializeField] private Vector3 activatedRotation = new Vector3(0, 0, -45);
    [SerializeField] private Vector3 deactivatedRotation = new Vector3(0, 0, 45);

    [Header("Connected Objects")]
    [SerializeField] private Fence[] connectedFences;

    [Header("Audio")]
    [SerializeField] private string activateAudioClip = "Lever_Activate";
    [SerializeField] private string deactivateAudioClip = "Lever_Deactivate";

    [Header("Animation")]
    [SerializeField] private float rotationSpeed = 2f;

    [Header("Outline Settings")]
    [SerializeField] private OutlineController outlineController;

    private bool playerInRange = false;
    private PlayerController currentPlayer;
    private bool isAnimating = false;

    private void Start() {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        if (leverVisual == null) {
            Transform visual = transform.Find("Visual");
            if (visual != null)
                leverVisual = visual.gameObject;
            else if (transform.childCount > 0)
                leverVisual = transform.GetChild(0).gameObject;
        }

        if (leverVisual != null)
            leverVisual.transform.localRotation = Quaternion.Euler(isActivated ? activatedRotation : deactivatedRotation);

        if (outlineController == null)
            outlineController = GetComponentInChildren<OutlineController>();

        UpdateConnectedFences();
    }

    private void OnTriggerEnter(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = true;
            currentPlayer = player;
            UpdatePrompt();
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
        if (playerInRange && currentPlayer != null && !isAnimating) {
            PlayerInput input = currentPlayer.GetComponent<PlayerInput>();
            if (input != null && input.InteractPressed)
                ToggleLever();
        }
    }

    private void UpdatePrompt() {
        if (!playerInRange) return;

        if (isActivated && canBeDeactivated)
            UIEvents.OnPromptShow?.Invoke("Press E to deactivate lever");
        else if (!isActivated)
            UIEvents.OnPromptShow?.Invoke("Press E to activate lever");
        else
            UIEvents.OnPromptShow?.Invoke("Lever is permanently activated");
    }

    private void ToggleLever() {
        if (isAnimating) return;

        if (isActivated && !canBeDeactivated) return;

        isActivated = !isActivated;
        StartCoroutine(AnimateLever());
        UpdateConnectedFences();
        UpdatePrompt();

        string audioClip = isActivated ? activateAudioClip : deactivateAudioClip;
        if (!string.IsNullOrEmpty(audioClip))
            AudioEvents.OnPlayAudio?.Invoke(audioClip, AudioType.SFX);
    }

    private System.Collections.IEnumerator AnimateLever() {
        if (leverVisual == null) yield break;

        isAnimating = true;

        Vector3 targetRotation = isActivated ? activatedRotation : deactivatedRotation;
        Quaternion startRotation = leverVisual.transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);

        float elapsed = 0f;
        float duration = 1f / rotationSpeed;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            leverVisual.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, progress);
            yield return null;
        }

        leverVisual.transform.localRotation = endRotation;
        isAnimating = false;
    }

    private void UpdateConnectedFences() {
        if (connectedFences == null) return;

        foreach (Fence fence in connectedFences) {
            if (fence != null) {
                if (isActivated)
                    fence.OpenFence();
                else
                    fence.CloseFence();
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = isActivated ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);

        if (connectedFences != null) {
            Gizmos.color = Color.yellow;
            foreach (Fence fence in connectedFences)
                if (fence != null)
                    Gizmos.DrawLine(transform.position, fence.transform.position);
        }

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f,
            $"Lever\nActivated: {isActivated}\nCan Deactivate: {canBeDeactivated}\nConnected Fences: {(connectedFences?.Length ?? 0)}");
#endif
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