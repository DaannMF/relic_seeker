using UnityEngine;

public class PlayerController : MonoBehaviour {
    private PlayerStateMachine stateMachine;

    private void Awake() {
        if (stateMachine == null)
            stateMachine = GetComponent<PlayerStateMachine>();

        if (stateMachine == null)
            stateMachine = gameObject.AddComponent<PlayerStateMachine>();
    }

    public void OnControlledBy(Controllable controllable) {
        Debug.Log($"Player is being controlled by {controllable.name}");
    }
}
