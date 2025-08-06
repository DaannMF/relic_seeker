using UnityEngine;

public class GameController : MonoBehaviour {
    private static GameState currentState = GameState.MainMenu;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        EnsureRequiredManagers();
        SubscribeToInputEvents();
    }

    private void Start() {
        SetGameState(GameState.MainMenu);
        CheckForMainMenuReset();
    }

    private void OnDestroy() {
        UnsubscribeFromInputEvents();
    }

    private void EnsureRequiredManagers() {
        if (KeyInventoryManager.Instance == null) {
            GameObject keyManagerObj = new GameObject("KeyInventoryManager");
            keyManagerObj.AddComponent<KeyInventoryManager>();
        }
    }

    private void CheckForMainMenuReset() {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentScene.Contains("MainMenu"))
            ResetGameProgress();
    }

    private void ResetGameProgress() {
        InventoryEvents.OnSetKeys?.Invoke(0);
    }

    private void SubscribeToInputEvents() {
        GameEvents.OnPauseRequested += HandlePauseRequested;
        GameEvents.OnResumeRequested += HandleResumeRequested;
        GameEvents.OnTogglePauseRequested += HandleTogglePauseRequested;
        GameEvents.OnStartGameRequested += HandleStartGameRequested;
        GameEvents.OnReturnToMenuRequested += HandleReturnToMenuRequested;
        GameEvents.OnGameWonTriggered += HandleGameWonTriggered;
        GameEvents.OnGameLostTriggered += HandleGameLostTriggered;
        GameEvents.OnSetGameStateRequested += HandleSetGameStateRequested;
    }

    private void UnsubscribeFromInputEvents() {
        GameEvents.OnPauseRequested -= HandlePauseRequested;
        GameEvents.OnResumeRequested -= HandleResumeRequested;
        GameEvents.OnTogglePauseRequested -= HandleTogglePauseRequested;
        GameEvents.OnStartGameRequested -= HandleStartGameRequested;
        GameEvents.OnReturnToMenuRequested -= HandleReturnToMenuRequested;
        GameEvents.OnGameWonTriggered -= HandleGameWonTriggered;
        GameEvents.OnGameLostTriggered -= HandleGameLostTriggered;
        GameEvents.OnSetGameStateRequested -= HandleSetGameStateRequested;
    }

    private void HandlePauseRequested() {
        if (currentState == GameState.Playing)
            SetGameState(GameState.Paused);
    }

    private void HandleResumeRequested() {
        if (currentState == GameState.Paused)
            SetGameState(GameState.Playing);
    }

    private void HandleTogglePauseRequested() {
        if (currentState == GameState.Playing)
            SetGameState(GameState.Paused);
        else if (currentState == GameState.Paused)
            SetGameState(GameState.Playing);
    }

    private void HandleStartGameRequested() {
        SetGameState(GameState.Playing);
    }

    private void HandleReturnToMenuRequested() {
        ResetGameProgress();
        SetGameState(GameState.MainMenu);
    }

    private void HandleGameWonTriggered() {
        SetGameState(GameState.Won);
        LoadCredits();
    }

    private void LoadCredits() {
        if (GameSceneManager.Instance != null)
            GameSceneManager.Instance.LoadCreditsScene("Credits");
    }

    private void HandleGameLostTriggered() {
        SetGameState(GameState.Lost);
    }

    private void HandleSetGameStateRequested(GameState newState) {
        SetGameState(newState);
    }

    private void SetGameState(GameState newState) {
        if (currentState == newState) return;

        GameState previousState = currentState;
        currentState = newState;

        GameEvents.OnGameStateChanged?.Invoke(newState);

        switch (newState) {
            case GameState.Paused:
                Time.timeScale = 0f;
                GameEvents.OnGamePaused?.Invoke();
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                if (previousState == GameState.Paused) {
                    GameEvents.OnGameResumed?.Invoke();
                }
                break;
            case GameState.Won:
                Time.timeScale = 0f;
                GameEvents.OnGameWon?.Invoke();
                break;
            case GameState.Lost:
                Time.timeScale = 0f;
                GameEvents.OnGameLost?.Invoke();
                break;
            case GameState.MainMenu:
                Time.timeScale = 1f;
                GameEvents.OnReturnToMainMenu?.Invoke();
                break;
        }
    }
}