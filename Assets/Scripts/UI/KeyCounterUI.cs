using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyCounterUI : MonoBehaviour {
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI keyCountText;
    [SerializeField] private Image keyIcon;
    [SerializeField] private GameObject keyCounterPanel;

    [Header("Display Settings")]
    [SerializeField] private string keyCountFormat = "Master Keys: {0}";
    [SerializeField] private bool hideWhenZeroKeys = false;

    private int currentKeyCount = 0;

    private void Start() {
        InventoryEvents.OnKeyCountChanged += OnKeyCountChanged;

        InventoryEvents.OnGetKeyCount?.Invoke(OnInitialKeyCountReceived);

        AutoDetectUIElements();
    }

    private void OnDestroy() {
        InventoryEvents.OnKeyCountChanged -= OnKeyCountChanged;
    }

    private void AutoDetectUIElements() {
        if (keyCountText == null)
            keyCountText = GetComponentInChildren<TextMeshProUGUI>();

        if (keyIcon == null)
            keyIcon = GetComponentInChildren<Image>();

        if (keyCounterPanel == null)
            keyCounterPanel = gameObject;
    }

    private void OnInitialKeyCountReceived(int keyCount) {
        currentKeyCount = keyCount;
        UpdateDisplay();
    }

    private void OnKeyCountChanged(int newKeyCount) {
        currentKeyCount = newKeyCount;
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        if (keyCountText != null) {
            string newText = string.Format(keyCountFormat, currentKeyCount);
            keyCountText.text = newText;
        }

        if (hideWhenZeroKeys && keyCounterPanel != null)
            keyCounterPanel.SetActive(currentKeyCount > 0);
    }

    public void SetKeyCountFormat(string format) {
        keyCountFormat = format;
        UpdateDisplay();
    }

    public void SetHideWhenZero(bool hide) {
        hideWhenZeroKeys = hide;
        UpdateDisplay();
    }
}