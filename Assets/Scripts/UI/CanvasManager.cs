using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour {
    [SerializeField] private TMP_Text interactionPrompt;

    private void Awake() {
        SubscribeToEvents();
    }

    private void Start() {
        interactionPrompt.text = "";
    }

    private void OnDestroy() {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents() {
        UIEvents.OnPromptShow += OnPromptShow;
        UIEvents.OnPromptHide += OnPromptHide;
    }

    private void UnsubscribeFromEvents() {
        UIEvents.OnPromptShow -= OnPromptShow;
        UIEvents.OnPromptHide -= OnPromptHide;
    }

    private void OnPromptShow(string promptText) {
        interactionPrompt.text = promptText;
    }

    private void OnPromptHide() {
        interactionPrompt.text = "";
    }
}