using UnityEngine;

public class Controllable : MonoBehaviour, IControllable {
    [SerializeField] public GameObject visualRepresentation;

    public void ControlEntity(PlayerController controller) {
        GameObject previusVisual = controller.transform.parent.GetComponent<Controllable>().visualRepresentation;
        previusVisual.transform.parent = controller.transform.parent;

        controller.transform.parent = transform;
        visualRepresentation.transform.parent = controller.transform;
        controller.ResetCameraPosition();
    }

    public void ShadeEntity() {
        Debug.Log("Controllable entity is being shaded.");
    }
}