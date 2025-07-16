using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {
    const string BUTTON_HOVER_AUDIO = "ButtonHover";
    const string BUTTON_CLICK_AUDIO = "ButtonClick";

    [Header("Audio Settings")]
    [SerializeField] private bool playHoverSound = true;
    [SerializeField] private bool playClickSound = true;

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (playHoverSound && button != null && button.interactable) {
            AudioEvents.OnPlayAudio?.Invoke(BUTTON_HOVER_AUDIO, AudioType.UI);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (playClickSound && button != null && button.interactable) {
            AudioEvents.OnPlayAudio?.Invoke(BUTTON_CLICK_AUDIO, AudioType.UI);
        }
    }
}
