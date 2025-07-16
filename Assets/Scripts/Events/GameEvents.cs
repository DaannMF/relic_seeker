using UnityEngine.Events;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    Won,
    Lost
}

public static class GameEvents
{
    // INPUT EVENTS - Other scripts invoke these to request game state changes
    public static UnityAction OnPauseRequested;
    public static UnityAction OnResumeRequested;
    public static UnityAction OnTogglePauseRequested;
    public static UnityAction OnStartGameRequested;
    public static UnityAction OnReturnToMenuRequested;
    public static UnityAction OnGameWonTriggered;
    public static UnityAction OnGameLostTriggered;
    public static UnityAction<GameState> OnSetGameStateRequested;

    // OUTPUT EVENTS - Other scripts listen to these for game state changes
    public static UnityAction<GameState> OnGameStateChanged;
    public static UnityAction OnGamePaused;
    public static UnityAction OnGameResumed;
    public static UnityAction OnGameWon;
    public static UnityAction OnGameLost;
    public static UnityAction OnReturnToMainMenu;
}