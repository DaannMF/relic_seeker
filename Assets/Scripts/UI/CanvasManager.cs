using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour {
    public static CanvasManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private LoadingUI loadingUI;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        SubscribeToEvents();

        if (mainMenuPanel == null) {
            mainMenuPanel = transform.Find("MainMenu")?.gameObject;
        }

        if (hudPanel == null) {
            hudPanel = transform.Find("HUD")?.gameObject;
        }
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
                ShowMainMenu();
                break;
        }
    }

    public void ShowMainMenu() {
        if (mainMenuPanel != null) {
            mainMenuPanel.SetActive(true);
        }
        if (hudPanel != null) {
            hudPanel.SetActive(false);
        }
    }

    public void HideMainMenu() {
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
}