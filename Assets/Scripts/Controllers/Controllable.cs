using UnityEngine;

public class Controllable : MonoBehaviour, IControllable {
    [SerializeField] private GameObject visualRepresentation;
    [SerializeField] private GameObject currentController;
    [SerializeField] private bool isOriginalPlayer = false;
    [SerializeField] private Vector3 cameraPositionReference;

    private static Controllable originalPlayer;
    private static Controllable lastControlledEntity;

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
        // if (visualRepresentation == null) return;

        // var _outline = visualRepresentation.GetComponent<Outline>();
        // if (_outline != null) return;

        // _outline = visualRepresentation.AddComponent<Outline>();
        // _outline.OutlineColor = Color.red;
        // _outline.OutlineWidth = 10f;
        // _outline.OutlineMode = Outline.Mode.OutlineVisible;
    }
}