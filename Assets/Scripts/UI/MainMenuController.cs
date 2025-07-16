using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button returnToMenuButton;

    [Header("Play Button Texts")]
    [SerializeField] private TextMeshProUGUI playButtonText;
    [SerializeField] private string playText = "PLAY";
    [SerializeField] private string resumeText = "RESUME";

    [Header("Scene Settings")]
    [SerializeField] private string mainGameSceneName = "MainGame";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private GameState currentGameState = GameState.MainMenu;

    private void Start() {
        SetupUI();
        SubscribeToEvents();
        UpdateUI();
    }

    private void OnDestroy() {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents() {
        GameEvents.OnGameStateChanged += HandleGameStateChanged;
    }

    private void UnsubscribeFromEvents() {
        GameEvents.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState) {
        currentGameState = newState;
        UpdateUI();
    }

    private void SetupUI() {
        if (playButtonText == null && playButton != null) {
            playButtonText = playButton.GetComponentInChildren<TextMeshProUGUI>();
        }

#if UNITY_WEBGL
        if (quitButton != null) {
            quitButton.gameObject.SetActive(false);
        }
#endif

        if (playButton != null) {
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        if (quitButton != null) {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        if (returnToMenuButton != null) {
            returnToMenuButton.onClick.AddListener(OnReturnToMenuButtonClicked);
        }
    }

    private void UpdateUI() {
        bool isInMainMenu = currentGameState == GameState.MainMenu;
        bool isInGame = currentGameState == GameState.Playing || currentGameState == GameState.Paused;

        if (playButtonText != null) {
            if (isInMainMenu)
                playButtonText.text = playText;

            else if (currentGameState == GameState.Paused)
                playButtonText.text = resumeText;
        }

        if (returnToMenuButton != null)
            returnToMenuButton.gameObject.SetActive(isInGame);


        if (quitButton != null)
            quitButton.gameObject.SetActive(!isInGame);
    }

    public void OnPlayButtonClicked() {
        if (currentGameState == GameState.MainMenu) {
            if (GameSceneManager.Instance != null) {
                GameSceneManager.Instance.LoadMainGameScene(mainGameSceneName);
            }
        }
        else if (currentGameState == GameState.Paused) {
            GameEvents.OnResumeRequested?.Invoke();
        }
    }

    public void OnReturnToMenuButtonClicked() {
        if (GameSceneManager.Instance != null) {
            GameSceneManager.Instance.ReturnToMainMenu(mainMenuSceneName);
        }
    }

    public void OnQuitButtonClicked() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}