using UnityEngine;

public class Controllable : MonoBehaviour, IControllable {
    [SerializeField] private GameObject visualRepresentation;
    [SerializeField] private GameObject currentController;
    [SerializeField] private bool isOriginalPlayer = false;
    [SerializeField] private Vector3 cameraPositionReference;

    [Header("UI Settings")]
    [SerializeField] private string controlPromptMessage = "Press E to control";

    [Header("Outline Settings")]
    [SerializeField] private OutlineController outlineController;

    private static Controllable originalPlayer;
    private static Controllable lastControlledEntity;
    private bool isShowingPrompt = false;

    private void Awake() {
        if (isOriginalPlayer) {
            originalPlayer = this;
        }

        if (!isOriginalPlayer) {
            foreach (Transform child in transform) {
                if (child.name != "Visual" && !child.GetComponent<PlayerController>()) {
                    currentController = child.gameObject;
                    break;
                }
            }
        }

        // Auto-asignar OutlineController si no está asignado
        if (outlineController == null) {
            outlineController = GetComponentInChildren<OutlineController>();
        }
    }

    public static void ReturnToOriginalPlayer(PlayerController controller) {
        if (originalPlayer != null) {
            if (lastControlledEntity != null && !lastControlledEntity.isOriginalPlayer) {
                lastControlledEntity.currentController.SetActive(true);
                lastControlledEntity = null;
            }

            originalPlayer.ControlEntity(controller);
        }
    }

    public void OnStartLooking() {
        if (!isShowingPrompt) {
            UIEvents.OnPromptShow?.Invoke(controlPromptMessage);
            isShowingPrompt = true;
        }

        // Also call outline for visual feedback
        OutlineEntity();
    }

    public void OnStopLooking() {
        if (isShowingPrompt) {
            UIEvents.OnPromptHide?.Invoke();
            isShowingPrompt = false;
        }

        // Ocultar outline cuando se deja de mirar
        if (outlineController != null) {
            outlineController.HideOutline();
        }
    }

    public void OnInteract(PlayerController controller) {
        // Hide prompt when interacting
        if (isShowingPrompt) {
            UIEvents.OnPromptHide?.Invoke();
            isShowingPrompt = false;
        }

        // Control the entity
        ControlEntity(controller);
    }

    public void ControlEntity(PlayerController playerController) {
        Controllable currentEntity = playerController.transform.parent.GetComponent<Controllable>();

        if (currentEntity != null && !currentEntity.isOriginalPlayer && currentEntity.currentController != null) {
            currentEntity.currentController.SetActive(true);
        }

        if (!isOriginalPlayer) {
            currentController.SetActive(false);
            lastControlledEntity = this;
        }

        playerController.transform.SetParent(transform);
        playerController.transform.localPosition = Vector3.zero;
        playerController.SetCameraPosition(cameraPositionReference);

        RefreshReferences(playerController);
    }

    public void RefreshReferences(PlayerController playerController) {
        var playerStateMachine = playerController.GetComponent<PlayerStateMachine>();
        if (playerStateMachine != null) {
            playerStateMachine.enabled = true;

            playerStateMachine.RefreshAnimatorReference();
        }

        playerController.RefreshReferences();
    }

    public void OutlineEntity() {
        if (outlineController != null) {
            outlineController.ShowOutline();
        }
    }
}