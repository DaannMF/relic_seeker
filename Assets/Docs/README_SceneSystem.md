# Game Scene Management System

Sistema completo para gestión de escenas que maneja el flujo MainMenu → MainGame → Interiores con optimización automática de performance.

## 🎯 Flujo del Juego

### Secuencia Completa

```
MainMenu Scene (DontDestroyOnLoad managers created)
    ↓ Play Button
MainGame Scene (Environment auto-detected)
    ↓ Interior Entrance  
Interior Scene (Environment disabled for performance)
    ↓ Interior Exit
MainGame Scene (Environment re-enabled)
```

## 📁 Componentes del Sistema

### 🚀 Managers Persistentes (DontDestroyOnLoad)

- **`GameSceneManager`** - Gestión de todas las transiciones de escenas
- **`CanvasManager`** - UI persistente con LoadingUI
- **`SceneEvents`** - Eventos centralizados para hooks personalizados

### 🚪 Interacción con Interiores

- **`InteriorEntrance`** - Triggers para entrar a interiores
- **`InteriorExit`** - Triggers para salir de interiores  
- **`InteriorSpawnPoint`** - Puntos de spawn automáticos

### ⚙️ Setup

- **`LoadingUISetup`** - (Opcional) Creación automática de LoadingUI

## 🔧 Configuración Manual

### GameSceneManager Inspector

```
[Required References]
└── Canvas Manager: [ASIGNAR CanvasManager component]

[Optional References]  
└── Main Scene Environment: [Auto-detectado por tag "Environment"]
```

### CanvasManager Inspector

```
[UI References]
└── Loading UI: [ASIGNAR LoadingUI component]
```

## 🌟 Sistema de Auto-Detección de Environment

### ✨ **Problema Solucionado:**

- **Antes**: Tenías que asignar el environment en MainMenu (donde no existe)
- **Ahora**: Se auto-detecta cuando cargas MainGame scene

### 🔍 **Cómo Funciona:**

1. **MainMenu Scene**: GameSceneManager no requiere environment
2. **MainGame Scene Load**: Auto-detecta GameObject con tag "Environment"
3. **Interior Load**: Usa environment para optimización (disable/enable)

### 🏷️ **Setup del Environment:**

```csharp
// En MainGame scene, asignar tag "Environment" al GameObject padre:
Environment (Tag: "Environment")
├── Terrain
├── Buildings  
├── Props
├── Lighting
└── Player Spawn Areas
```

### 📋 **Métodos Disponibles:**

```csharp
// Auto-detectar por tag (llamado automáticamente)
GameSceneManager.Instance.AutoDetectMainEnvironment();

// Asignar manualmente si es necesario
GameSceneManager.Instance.SetMainSceneEnvironment(environmentGameObject);
```

## 🎮 Tipos de Carga

### SceneLoadType.Single

- **MainMenu → MainGame**: Reemplaza escena completamente
- **MainGame → MainMenu**: Vuelve al menú principal
- **Cualquier → Cualquier**: Transición completa entre escenas principales

### SceneLoadType.Additive

- **MainGame → Interior**: Carga aditiva + environment disabled
- **Optimización**: Mantiene MainGame cargada pero invisible
- **Performance**: Solo desactiva GameObjects, no descarga escena

## 💻 Uso en Código

### Desde MainMenu

```csharp
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string mainGameSceneName = "MainGame";
    
    public void OnPlayButtonClicked()
    {
        // Carga MainGame scene (Single mode)
        GameSceneManager.Instance.LoadMainGameScene(mainGameSceneName);
        // Environment se auto-detecta automáticamente
    }
}
```

### Entrar a Interior

```csharp
public class InteriorEntrance : MonoBehaviour
{
    [SerializeField] private string interiorSceneName = "HouseInterior";
    [SerializeField] private string spawnPointID = ""; // Opcional
    
    private void EnterInterior()
    {
        // Auto-detecta environment si no está asignado
        // Desactiva environment automáticamente
        GameSceneManager.Instance.LoadInteriorScene(interiorSceneName, spawnPointID);
    }
}
```

### Salir de Interior

```csharp
public class InteriorExit : MonoBehaviour
{
    private void ExitInterior()
    {
        // Re-activa environment automáticamente
        // Retorna player a posición original
        GameSceneManager.Instance.ExitInterior();
    }
}
```

## 🔔 Sistema de Eventos

### Suscripción a Eventos

```csharp
void Start()
{
    SceneEvents.OnSceneLoaded += OnSceneLoaded;
    SceneEvents.OnInteriorEntered += OnInteriorEntered;
    SceneEvents.OnInteriorExited += OnInteriorExited; 
    SceneEvents.OnSceneUnloaded += OnSceneUnloaded;
}

void OnSceneLoaded(string sceneName)
{
    Debug.Log($"Scene loaded: {sceneName}");
    
    if (sceneName == "MainGame")
    {
        // MainGame scene cargada, environment auto-detectado
    }
}

void OnInteriorEntered(string interiorName)
{
    // Interior cargado, environment desactivado automáticamente
    // Player movido a spawn point automáticamente
}
```

## 🐛 Debugging y Logs

### Logs Automáticos del Sistema

```
[GameSceneManager] Auto-detected main environment: Environment
[GameSceneManager] Loading scene: MainGame with mode: Single
[GameSceneManager] Scene MainGame loaded successfully
[GameSceneManager] Loading scene: HouseInterior with mode: Additive
[GameSceneManager] Main scene environment disabled for performance
[GameSceneManager] Player moved to spawn point: PlayerSpawn
[GameSceneManager] Main scene environment re-enabled
```

### Warnings Útiles

```
[GameSceneManager] No main scene environment found! Performance optimization won't work. Consider tagging your environment GameObject with 'Environment'.
[GameSceneManager] CanvasManager reference not assigned! Loading screens might not work properly.
```

## 📋 Setup Paso a Paso

### 1. MainMenu Scene Setup

```
Hierarchy:
├── UI Canvas (DontDestroyOnLoad)
│   ├── CanvasManager (LoadingUI: assigned)
│   ├── LoadingUI
│   └── Menu UI (Play/Quit buttons)
├── GameSceneManager (CanvasManager: assigned)
└── (Optional) LoadingUISetup
```

### 2. MainGame Scene Setup

```
Hierarchy:
├── Environment (Tag: "Environment") ← AUTO-DETECTADO
│   ├── Terrain
│   ├── Buildings
│   ├── Lighting
│   └── Props
├── Player
└── Interactables
    └── HouseEntrance (InteriorEntrance)
```

### 3. Interior Scene Setup

```
Hierarchy:
├── Interior Content
│   ├── Furniture
│   └── Lighting
├── PlayerSpawn (InteriorSpawnPoint: Type = Entrance)
└── Exit (InteriorExit)
```

## ✅ Checklist de Validación

### MainMenu Scene

- [ ] GameSceneManager con CanvasManager asignado
- [ ] CanvasManager con LoadingUI asignado
- [ ] MainMenuController configurado con scene name

### MainGame Scene

- [ ] GameObject principal con tag "Environment"
- [ ] InteriorEntrance configurados con scene names
- [ ] Player configurado correctamente

### Interior Scenes

- [ ] Al menos un InteriorSpawnPoint (Type: Entrance)
- [ ] InteriorExit configurado
- [ ] Nombres de escenas en Build Settings

## 🎯 Ventajas del Sistema Mejorado

### 🚀 **Flujo Natural:**

- **MainMenu**: No requiere references que no existen
- **MainGame**: Auto-detecta environment automáticamente
- **Interiores**: Optimización transparente sin configuración manual

### ⚡ **Performance Automática:**

- **Environment Disable**: Desactiva automáticamente en interiores
- **Memory Efficient**: Mantiene escenas cargadas, solo desactiva GameObjects
- **Seamless Transitions**: Player no pierde estado entre transiciones

### 🔧 **Mantenimiento Fácil:**

- **Tag-Based**: Solo necesitas tag "Environment"
- **Auto-Detection**: No asignación manual de referencias complejas
- **Clear Warnings**: Mensajes específicos para debugging

¡Sistema robusto que se adapta al flujo natural del juego! 🚀
