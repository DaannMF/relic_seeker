# Game Controller & State Management System

Sistema completamente basado en **eventos centralizados** en la carpeta Events. Los eventos están segmentados por tipos en clases estáticas especializadas, permitiendo acceso desde cualquier script.

## 🎯 Arquitectura Event-Based Centralizada

### 📁 **Eventos Centralizados** (`Assets/Scripts/Events/`)

#### 🎮 **GameEvents.cs** - Gestión de Estados del Juego

```csharp
public static class GameEvents {
    // INPUT EVENTS - Scripts invocan estos para cambiar estados
    public static UnityAction OnPauseRequested;
    public static UnityAction OnResumeRequested;
    public static UnityAction OnTogglePauseRequested;
    public static UnityAction OnStartGameRequested;
    public static UnityAction OnReturnToMenuRequested;
    public static UnityAction OnGameWonTriggered;
    public static UnityAction OnGameLostTriggered;
    public static UnityAction<GameState> OnSetGameStateRequested;

    // OUTPUT EVENTS - Scripts escuchan estos para reaccionar
    public static UnityAction<GameState> OnGameStateChanged;
    public static UnityAction OnGamePaused;
    public static UnityAction OnGameResumed;
    public static UnityAction OnGameWon;
    public static UnityAction OnGameLost;
    public static UnityAction OnReturnToMainMenu;
}
```

#### 🎬 **SceneEvents.cs** - Gestión de Escenas

```csharp
public static class SceneEvents {
    public static System.Action<string> OnSceneLoaded;
    public static System.Action<string> OnSceneUnloaded;
    public static System.Action<string> OnInteriorEntered;
    public static System.Action<string> OnInteriorExited;
}
```

#### 🎨 **UIEvents.cs** - Gestión de UI

```csharp
public static class UIEvents {
    public static UnityAction<string> OnPromptShow;
    public static UnityAction OnPromptHide;
}
```

### 🎮 **GameController** - Solo Lógica Interna

- **Función**: Se suscribe a INPUT events de GameEvents y dispara OUTPUT events
- **Acceso**: **NO** métodos públicos estáticos
- **Comunicación**: **EXCLUSIVAMENTE** vía GameEvents

## 🔄 Estados del Juego

```csharp
// Definido en GameEvents.cs
public enum GameState {
    MainMenu,    // En el menú principal
    Playing,     // Jugando activamente
    Paused,      // Juego pausado (menú visible)
    Won,         // Condición de victoria
    Lost         // Condición de derrota
}
```

## 🎯 Flujo Event-Based Completo

### Secuencia con Eventos Centralizados

```
MainMenu Scene (GameState.MainMenu)
    ↓ Play Button
    GameEvents.OnStartGameRequested?.Invoke()
    ↓ GameController escucha y dispara
    GameEvents.OnGameStateChanged?.Invoke(GameState.Playing)
    ↓ CanvasManager escucha y oculta MainMenuPanel

MainGame Scene (GameState.Playing)
    ↓ Esc Key (PlayerInput)
    GameEvents.OnTogglePauseRequested?.Invoke()
    ↓ GameController escucha y dispara
    GameEvents.OnGameStateChanged?.Invoke(GameState.Paused)
    ↓ CanvasManager escucha y muestra MainMenuPanel

Paused State
    ↓ Resume Button (MainMenuController)
    GameEvents.OnResumeRequested?.Invoke()
    ↓ GameController escucha y dispara
    GameEvents.OnGameStateChanged?.Invoke(GameState.Playing)
    ↓ CanvasManager escucha y oculta MainMenuPanel
```

## 🛠️ Implementación por Componente

### 🎮 **GameController** - Manager de Estados

```csharp
public class GameController : MonoBehaviour {
    private void SubscribeToInputEvents() {
        // Escucha INPUT events de GameEvents
        GameEvents.OnPauseRequested += HandlePauseRequested;
        GameEvents.OnStartGameRequested += HandleStartGameRequested;
        // ...
    }

    private void HandlePauseRequested() {
        if (currentState == GameState.Playing) {
            SetGameState(GameState.Paused);
        }
    }

    private void SetGameState(GameState newState) {
        // Dispara OUTPUT events de GameEvents
        GameEvents.OnGameStateChanged?.Invoke(newState);
        GameEvents.OnGamePaused?.Invoke();
    }
}
```

### 🎨 **CanvasManager** - UI Manager

```csharp
private void SubscribeToEvents() {
    // Escucha OUTPUT events de GameEvents
    GameEvents.OnGameStateChanged += HandleGameStateChanged;
    GameEvents.OnGamePaused += ShowMainMenu;
    GameEvents.OnReturnToMainMenu += ShowMainMenu;
}

private void HandleGameStateChanged(GameState newState) {
    switch (newState) {
        case GameState.Playing:
            HideMainMenu();
            break;
        case GameState.Paused:
        case GameState.MainMenu:
            ShowMainMenu();
            break;
    }
}
```

### 🎮 **MainMenuController** - UI Controller

```csharp
private GameState currentGameState = GameState.MainMenu;

private void SubscribeToEvents() {
    // Escucha OUTPUT events para tracking de estado
    GameEvents.OnGameStateChanged += HandleGameStateChanged;
}

public void OnPlayButtonClicked() {
    if (currentGameState == GameState.Paused) {
        // Invoca INPUT events de GameEvents
        GameEvents.OnResumeRequested?.Invoke();
    }
}
```

### 🎛️ **GameSceneManager** - Scene Manager

```csharp
public void LoadMainGameScene(string sceneName) {
    LoadSingleScene(sceneName);
    // Invoca INPUT events de GameEvents
    GameEvents.OnStartGameRequested?.Invoke();
}

public void ReturnToMainMenu(string menuSceneName = "MainMenu") {
    LoadSingleScene(menuSceneName);
    // Invoca INPUT events de GameEvents
    GameEvents.OnReturnToMenuRequested?.Invoke();
}
```

### 🎮 **PlayerInput** - Input Handler

```csharp
private GameState currentGameState = GameState.MainMenu;

private void SubscribeToEvents() {
    // Escucha OUTPUT events para tracking de estado
    GameEvents.OnGameStateChanged += HandleGameStateChanged;
}

private void Update() {
    PausePressed = Input.GetKeyDown(KeyCode.Escape);
    
    if (PausePressed && currentGameState != GameState.MainMenu) {
        // Invoca INPUT events de GameEvents
        GameEvents.OnTogglePauseRequested?.Invoke();
    }
}
```

## 🎮 Sistema de Eventos Desde Cualquier Lugar

### ✅ **Invocar Estados de Juego** (desde cualquier script)

```csharp
// Pausar juego
GameEvents.OnPauseRequested?.Invoke();

// Reanudar juego
GameEvents.OnResumeRequested?.Invoke();

// Iniciar juego
GameEvents.OnStartGameRequested?.Invoke();

// Ganar/Perder
GameEvents.OnGameWonTriggered?.Invoke();
GameEvents.OnGameLostTriggered?.Invoke();

// Volver al menú
GameEvents.OnReturnToMenuRequested?.Invoke();
```

### ✅ **Escuchar Estados de Juego** (desde cualquier script)

```csharp
private void Start() {
    GameEvents.OnGameStateChanged += HandleGameStateChanged;
    GameEvents.OnGamePaused += OnGamePaused;
    GameEvents.OnGameWon += OnGameWon;
}

private void OnDestroy() {
    GameEvents.OnGameStateChanged -= HandleGameStateChanged;
    GameEvents.OnGamePaused -= OnGamePaused;
    GameEvents.OnGameWon -= OnGameWon;
}
```

### ✅ **Usar Otros Eventos** (según necesidad)

```csharp
// Scene Events
SceneEvents.OnSceneLoaded += HandleSceneLoaded;
SceneEvents.OnInteriorEntered += HandleInteriorEntered;

// UI Events
UIEvents.OnPromptShow += HandlePromptShow;
UIEvents.OnPromptHide += HandlePromptHide;
```

## 🛠️ Configuración Manual

### 1. MainMenu Scene Setup

#### GameController GameObject

```
Hierarchy:
├── GameController
│   └── GameController.cs (solo se suscribe a GameEvents)
```

#### CanvasManager Setup

```
Inspector:
[UI References]
├── Loading UI: [Assign LoadingUI component]
└── Main Menu Panel: [Auto-detected "MainMenu" GameObject]
```

### 2. MainMenuPanel UI Setup

#### MainMenuController Configuration

```
Inspector:
[UI Elements]
├── Play Button: [Assign Play Button]
├── Quit Button: [Assign Quit Button]
└── Return To Menu Button: [Assign new Return Button]

[Play Button Texts]
├── Play Button Text: [Assign TextMeshPro component]
├── Play Text: "PLAY"
└── Resume Text: "RESUME"
```

## 🎯 Funcionalidades Implementadas

### ✅ **Sistema de Eventos Centralizados**

- **GameEvents**: Estados del juego
- **SceneEvents**: Gestión de escenas  
- **UIEvents**: Gestión de UI
- **Acceso global**: Desde cualquier script

### ✅ **GameController Event-Based**

- **NO métodos públicos estáticos**
- **Solo se suscribe a INPUT events**
- **Solo dispara OUTPUT events**

### ✅ **Dynamic UI System**

- **MainMenuPanel**: Auto-hide/show según estado
- **Play Button**: "PLAY" ↔ "RESUME"
- **Return to Menu**: Solo visible en juego
- **Quit Button**: Solo visible en MainMenu

### ✅ **Global Pause System**

- **Esc Key**: `GameEvents.OnTogglePauseRequested`
- **State Tracking**: Cada componente mantiene su estado
- **UI Response**: Automática vía eventos

## 🚀 Ventajas del Sistema Centralizado

### 🔒 **Organización Clara**

- **Eventos por categoría**: Game, Scene, UI
- **Acceso universal**: Cualquier script puede usar eventos
- **Sin dependencias**: No hay referencias cruzadas

### 🎯 **Escalabilidad Total**

- **Nuevos eventos**: Solo agregar a la clase correspondiente
- **Nuevos listeners**: Sin modificar código existente
- **Modularidad**: Componentes completamente independientes

### 🚀 **Mantenimiento Simple**

- **Búsqueda fácil**: Todos los eventos en Events/
- **Tipado fuerte**: GameState, SceneEvents, etc.
- **Debugging claro**: Flujo de eventos trazeable

## 🔧 Próximos Pasos

1. **Verificar compilación** - Unity debe reconocer GameEvents
2. **Crear GameController** en MainMenu scene  
3. **Configurar UI** - MainMenuPanel y botones
4. **Probar flujo completo** - Sin referencias directas al GameController

¡Sistema completamente event-based con eventos centralizados! 🚀
