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
        Debug.Log("[KeyCounterUI] Starting initialization");

        // Subscribe to key count changes
        InventoryEvents.OnKeyCountChanged += OnKeyCountChanged;
        Debug.Log("[KeyCounterUI] Subscribed to OnKeyCountChanged");

        // Get initial key count
        InventoryEvents.OnGetKeyCount?.Invoke(OnInitialKeyCountReceived);
        Debug.Log("[KeyCounterUI] Requested initial key count");

        // Auto-detect UI elements if not assigned
        AutoDetectUIElements();
    }

    private void OnDestroy() {
        // Unsubscribe from events
        InventoryEvents.OnKeyCountChanged -= OnKeyCountChanged;
        Debug.Log("[KeyCounterUI] Unsubscribed from events");
    }

    private void AutoDetectUIElements() {
        // Auto-detect text component if not assigned
        if (keyCountText == null) {
            keyCountText = GetComponentInChildren<TextMeshProUGUI>();
            if (keyCountText == null) {
                Text legacyText = GetComponentInChildren<Text>();
                if (legacyText != null) {
                    Debug.LogWarning("[KeyCounterUI] Using legacy Text component. Consider upgrading to TextMeshPro.");
                }
            }
        }

        // Auto-detect image component if not assigned
        if (keyIcon == null) {
            keyIcon = GetComponentInChildren<Image>();
        }

        // Use this GameObject as panel if not assigned
        if (keyCounterPanel == null) {
            keyCounterPanel = gameObject;
        }

        Debug.Log($"[KeyCounterUI] UI Elements - Text: {(keyCountText != null ? "Found" : "Missing")}, Panel: {(keyCounterPanel != null ? "Found" : "Missing")}");
    }

    private void OnInitialKeyCountReceived(int keyCount) {
        Debug.Log($"[KeyCounterUI] Received initial key count: {keyCount}");
        currentKeyCount = keyCount;
        UpdateDisplay();
    }

    private void OnKeyCountChanged(int newKeyCount) {
        Debug.Log($"[KeyCounterUI] Key count changed: {currentKeyCount} → {newKeyCount}");
        currentKeyCount = newKeyCount;
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        Debug.Log($"[KeyCounterUI] Updating display with {currentKeyCount} keys");

        // Update text
        if (keyCountText != null) {
            string newText = string.Format(keyCountFormat, currentKeyCount);
            keyCountText.text = newText;
            Debug.Log($"[KeyCounterUI] Updated text to: '{newText}'");
        }
        else {
            Debug.LogWarning("[KeyCounterUI] keyCountText is null - cannot update display");
        }

        // Handle visibility
        if (hideWhenZeroKeys && keyCounterPanel != null) {
            keyCounterPanel.SetActive(currentKeyCount > 0);
            Debug.Log($"[KeyCounterUI] Panel visibility: {keyCounterPanel.activeSelf}");
        }
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