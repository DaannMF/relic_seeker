using UnityEngine;

public class Controllable : MonoBehaviour, IControllable {
    [SerializeField] public GameObject visualRepresentation;

    public void ControlEntity(PlayerController controller) {
        GameObject previusVisual = controller.transform.parent.GetComponent<Controllable>().visualRepresentation;
        previusVisual.transform.parent = controller.transform.parent;
        previusVisual.transform.localPosition = Vector3.zero;

        controller.transform.parent = transform;
        controller.transform.localPosition = Vector3.zero;
        visualRepresentation.transform.parent = controller.transform;
        visualRepresentation.transform.localPosition = Vector3.zero;
    }

    public void ShadeEntity() {
        Debug.Log("Controllable entity is being shaded.");
    }
}