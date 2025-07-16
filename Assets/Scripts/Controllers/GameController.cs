using UnityEngine;

public class GameController : MonoBehaviour {
    private static GameState currentState = GameState.MainMenu;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        SubscribeToInputEvents();
    }

    private void Start() {
        SetGameState(GameState.MainMenu);
    }

    private void OnDestroy() {
        UnsubscribeFromInputEvents();
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
        if (currentState == GameState.Playing) {
            SetGameState(GameState.Paused);
        }
    }

    private void HandleResumeRequested() {
        if (currentState == GameState.Paused) {
            SetGameState(GameState.Playing);
        }
    }

    private void HandleTogglePauseRequested() {
        if (currentState == GameState.Playing) {
            SetGameState(GameState.Paused);
        }
        else if (currentState == GameState.Paused) {
            SetGameState(GameState.Playing);
        }
    }

    private void HandleStartGameRequested() {
        SetGameState(GameState.Playing);
    }

    private void HandleReturnToMenuRequested() {
        SetGameState(GameState.MainMenu);
    }

    private void HandleGameWonTriggered() {
        SetGameState(GameState.Won);
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
                Time.timeScale = 0f; // Pause the game
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GameEvents.OnGamePaused?.Invoke();
                break;
            case GameState.Playing:
                Time.timeScale = 1f; // Resume the game
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (previousState == GameState.Paused) {
                    GameEvents.OnGameResumed?.Invoke();
                }
                break;
            case GameState.Won:
                Time.timeScale = 0f; // Pause on game over
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GameEvents.OnGameWon?.Invoke();
                break;
            case GameState.Lost:
                Time.timeScale = 0f; // Pause on game over
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GameEvents.OnGameLost?.Invoke();
                break;
            case GameState.MainMenu:
                Time.timeScale = 1f; // Ensure normal time in menu
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GameEvents.OnReturnToMainMenu?.Invoke();
                break;
        }
    }
}