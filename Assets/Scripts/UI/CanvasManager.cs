using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour {
    public static CanvasManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private LoadingUI loadingUI;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Texture2D cursorTexture;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start() {
        SubscribeToEvents();
        AutoDetectUIElements();
        EnsureKeyCounterUIExists();
    }

    private void OnDestroy() {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents() {
        GameEvents.OnGameStateChanged += HandleGameStateChanged;
        GameEvents.OnGamePaused += ShowMainMenu;
        GameEvents.OnReturnToMainMenu += ShowMainMenu;
        UIEvents.OnPromptShow += ShowPrompt;
        UIEvents.OnPromptHide += HidePrompt;
    }

    private void UnsubscribeFromEvents() {
        GameEvents.OnGameStateChanged -= HandleGameStateChanged;
        GameEvents.OnGamePaused -= ShowMainMenu;
        GameEvents.OnReturnToMainMenu -= ShowMainMenu;
        UIEvents.OnPromptShow -= ShowPrompt;
        UIEvents.OnPromptHide -= HidePrompt;
    }

    private void HandleGameStateChanged(GameState newState) {
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log($"Cursor visible: {Cursor.visible}, lockState: {Cursor.lockState} ");

        switch (newState) {
            case GameState.MainMenu:
                ShowMainMenu();
                break;
            case GameState.Playing:
                HideMainMenu();
                break;
            case GameState.Paused:
                ShowMainMenu();
                break;
            case GameState.Won:
            case GameState.Lost:
                break;
        }
    }

    public void ShowMainMenu() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        if (mainMenuPanel != null) {
            mainMenuPanel.SetActive(true);
        }
        if (hudPanel != null) {
            hudPanel.SetActive(false);
        }
    }

    public void HideMainMenu() {
        Cursor.visible = false;
        if (mainMenuPanel != null) {
            mainMenuPanel.SetActive(false);
        }
        if (hudPanel != null) {
            hudPanel.SetActive(true);
        }
    }

    public void SetLoadingUI(LoadingUI loading) {
        loadingUI = loading;
    }

    public void ShowLoading() {
        if (loadingUI != null) {
            loadingUI.ShowLoadingScreen();
        }
        else {
            Debug.LogWarning("[CanvasManager] LoadingUI not assigned!");
        }
    }

    public void ShowLoading(string loadingType) {
        if (loadingUI != null) {
            loadingUI.SetLoadingType(loadingType);
            loadingUI.ShowLoadingScreen();
        }
        else {
            Debug.LogWarning("[CanvasManager] LoadingUI not assigned!");
        }
    }

    public void HideLoading() {
        if (loadingUI != null) {
            loadingUI.HideLoadingScreen();
        }
        else {
            Debug.LogWarning("[CanvasManager] LoadingUI not assigned!");
        }
    }

    public void UpdateLoadingProgress(float progress) {
        if (loadingUI != null) {
            loadingUI.UpdateProgress(progress);
        }
        else {
            Debug.LogWarning("[CanvasManager] LoadingUI not assigned!");
        }
    }

    public void ShowPrompt(string message) {
        if (promptText != null && hudPanel != null && hudPanel.activeInHierarchy) {
            promptText.text = message;
        }
    }

    public void HidePrompt() {
        if (promptText != null) {
            promptText.text = "";
        }
    }

    private void AutoDetectUIElements() {
        if (mainMenuPanel == null) {
            mainMenuPanel = transform.Find("MainMenu")?.gameObject;
        }

        if (hudPanel == null) {
            hudPanel = transform.Find("HUD")?.gameObject;
        }
    }

    private void EnsureKeyCounterUIExists() {
        // Look for KeyCounter GameObject
        Transform keyCounterTransform = FindChildRecursive(transform, "KeyCounter");

        if (keyCounterTransform != null) {
            // Check if it already has KeyCounterUI component
            KeyCounterUI existingKeyCounter = keyCounterTransform.GetComponent<KeyCounterUI>();

            if (existingKeyCounter == null) {
                // Add KeyCounterUI component
                keyCounterTransform.gameObject.AddComponent<KeyCounterUI>();
                Debug.Log("[CanvasManager] Added KeyCounterUI component to KeyCounter GameObject");
            }
        }
        else {
            Debug.LogWarning("[CanvasManager] KeyCounter GameObject not found in hierarchy");
        }
    }

    private Transform FindChildRecursive(Transform parent, string childName) {
        foreach (Transform child in parent) {
            if (child.name == childName) {
                return child;
            }

            Transform result = FindChildRecursive(child, childName);
            if (result != null) {
                return result;
            }
        }
        return null;
    }
}