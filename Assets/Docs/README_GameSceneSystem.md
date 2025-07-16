# Game Scene Management System

Un sistema completo y genérico para la gestión de escenas en todo el juego, con soporte tanto para transiciones principales (MainMenu ↔ MainGame) como para interiores aditivos con optimización de rendimiento.

## 📁 Estructura del Sistema

### 🎯 Managers Principales (DontDestroyOnLoad)

- **`GameSceneManager.cs`** - Manager principal para todas las transiciones de escenas
- **`CanvasManager.cs`** - Manager de UI persistente a través de todas las escenas
- **`LoadingUI.cs`** - Componente de pantalla de carga unificada

### 🚪 Componentes de Interacción

- **`InteriorEntrance.cs`** - Trigger para entrar a interiores (carga aditiva)
- **`InteriorExit.cs`** - Trigger para salir de interiores
- **`InteriorSpawnPoint.cs`** - Puntos de spawn automáticos en interiores

### ⚙️ Setup y Configuración

- **`LoadingUISetup.cs`** - Setup automático solo para LoadingUI
- **`MainMenuController.cs`** - Ejemplo de controller para menú principal

## 🚀 Configuración Manual

### Setup de Managers (Manual)

1. **Crear GameSceneManager**:
   - Crear GameObject vacío llamado "GameSceneManager"
   - Añadir componente `GameSceneManager`
   - Asignar referencias en inspector:
     - Loading UI → Referencia al LoadingUI
     - Main Scene Environment → GameObject padre del environment
     - Canvas Manager → (Opcional) Referencia al CanvasManager

2. **Crear CanvasManager**:
   - En tu Canvas principal, añadir componente `CanvasManager`
   - Asignar referencias en inspector:
     - Loading UI → Referencia al LoadingUI
   - Marcar Canvas como DontDestroyOnLoad si es necesario

3. **Crear LoadingUI** (Opcional - Setup Automático):
   - Añadir `LoadingUISetup` a cualquier GameObject
   - Configurar Canvas target
   - El script creará automáticamente el LoadingUI completo

### Configuración Requerida

#### GameSceneManager Inspector

```
[Required References]
├── Loading UI: [Assign LoadingUI component]
├── Main Scene Environment: [Assign main environment GameObject]  
└── Canvas Manager: [Optional - Assign CanvasManager]

[Debug]
└── Enable Debug Logs: ✅
```

#### CanvasManager Inspector

```
[UI References]
└── Loading UI: [Assign LoadingUI component]

[Debug]
└── Enable Debug Logs: ✅
```

## 🎮 Flujo del Juego

### Secuencia Completa

```
MainMenu (Single)
    ↓ Play Button
MainGame (Single) 
    ↓ Interior Entrance
Interior (Additive + Environment Disabled)
    ↓ Interior Exit  
MainGame (Environment Re-enabled)
    ↓ Menu Button
MainMenu (Single)
```

## 🔧 Tipos de Carga de Escenas

### SceneLoadType.Single

- **Uso**: MainMenu ↔ MainGame
- **Comportamiento**: Reemplaza la escena actual completamente
- **Ejemplos**: Menú principal, escena principal del juego

### SceneLoadType.Additive  

- **Uso**: Interiores desde escena principal
- **Comportamiento**: Carga aditivamente + desactiva environment principal
- **Optimización**: Mejora rendimiento manteniendo ambas escenas

## 📍 Sistema de Spawn Points

### Configuración de InteriorSpawnPoint

```csharp
// En escena interior
GameObject spawnPoint = new GameObject("PlayerSpawn");
InteriorSpawnPoint spawn = spawnPoint.AddComponent<InteriorSpawnPoint>();
spawn.PointType = SpawnPointType.Entrance; // Verde en editor
// Posicionar donde debe aparecer el player
```

### Selección Automática

1. **ID específico** → Busca spawn point con ese ID
2. **Tipo Entrance** → Busca puntos de entrada por defecto  
3. **Primer disponible** → Fallback al primero encontrado
4. **Sin spawn points** → Player se queda en posición actual

## ⚡ Características del Sistema

### 🎯 Unified Loading (6 segundos)

- **Consistencia**: Todas las cargas usan el mismo tiempo base
- **Fake Progress**: Progreso mínimo garantizado para pulimento
- **Real Progress**: Combina progreso real con fake progress

### 🎮 Optimización de Rendimiento

- **Environment Disable**: Desactiva environment principal en interiores
- **Memory Management**: No descarga escena principal, solo desactiva
- **Smooth Transitions**: Player se desactiva temporalmente durante transiciones

### 🔄 Gestión de Estado

- **Player Position**: Guarda/restaura posición original automáticamente
- **Scene Tracking**: Mantiene registro de escena principal y interior actual
- **Event System**: Eventos para hooks personalizados

## 💻 Uso Programático

### Desde Main Menu

```csharp
public class MainMenuController : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        // Carga escena principal (Single mode)
        GameSceneManager.Instance.LoadMainGameScene("MainGame");
    }

    public void OnReturnToMenu()
    {
        // Regresa al menú principal
        GameSceneManager.Instance.ReturnToMainMenu("MainMenu");
    }
}
```

### Cargar Interior

```csharp
public class InteriorEntrance : MonoBehaviour
{
    private void EnterInterior()
    {
        // Carga interior aditivamente
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
        // Retorna a escena principal automáticamente
        GameSceneManager.Instance.ExitInterior();
    }
}
```

## 🛠️ Configuración Avanzada

### Environment Management

```csharp
// Asignar environment manualmente en inspector
// O mediante código:
GameSceneManager.Instance.SetMainSceneEnvironment(environmentGameObject);
```

### Event Subscriptions

```csharp
void Start()
{
    // Suscribirse a eventos de escenas
    GameSceneManager.Instance.OnSceneLoaded += OnSceneLoaded;
    GameSceneManager.Instance.OnInteriorEntered += OnInteriorEntered;
    GameSceneManager.Instance.OnInteriorExited += OnInteriorExited;
    GameSceneManager.Instance.OnSceneUnloaded += OnSceneUnloaded;
}

void OnSceneLoaded(string sceneName)
{
    Debug.Log($"Scene loaded: {sceneName}");
}
```

### Custom Loading Messages

```csharp
// En LoadingUI.cs
private string[] loadingMessages = {
    "Loading...",
    "Entering Interior...", 
    "Preparing Environment...",
    "Almost Ready..."
};
```

## 🐛 Debugging y Solución de Problemas

### Logs del Sistema

- `[GameSceneManager] Scene {name} loaded successfully`
- `[CanvasManager] LoadingUI reference updated`  
- `[LoadingUISetup] LoadingUI created and configured automatically`

### Problemas Comunes

1. **"LoadingUI reference not assigned"**
   - ✅ Asignar LoadingUI en inspector de GameSceneManager
   - ✅ Asignar LoadingUI en inspector de CanvasManager

2. **"Main Scene Environment not assigned"**
   - ✅ Asignar GameObject environment en inspector
   - ✅ Performance optimization para interiores no funcionará sin esto

3. **"No spawn point found"**
   - ✅ Añadir `InteriorSpawnPoint` en escena interior
   - ✅ Configurar como tipo "Entrance"

4. **Loading UI no aparece**
   - ✅ Verificar referencias asignadas en inspector
   - ✅ Comprobar que Canvas sea persistente

### Validation Checklist

- [ ] GameSceneManager configurado con todas las referencias
- [ ] CanvasManager configurado con LoadingUI reference
- [ ] Environment GameObject asignado manualmente  
- [ ] InteriorSpawnPoints en escenas interiores
- [ ] Nombres de escenas correctos en Build Settings

## 📋 Ejemplo de Implementación Completa

### 1. MainMenu Scene Setup

```
Hierarchy:
├── UI Canvas (DontDestroyOnLoad)
│   ├── CanvasManager (LoadingUI: assigned)
│   ├── LoadingUI (created by LoadingUISetup)
│   └── MenuUI
│       ├── PlayButton → OnPlayButtonClicked()
│       └── QuitButton → OnQuitButtonClicked()
├── GameSceneManager (all references assigned)
└── LoadingUISetup (optional - creates LoadingUI)
```

### 2. MainGame Scene Setup

```
Hierarchy:
├── Environment (assigned to GameSceneManager)
│   ├── Terrain
│   ├── Buildings  
│   ├── Props
│   └── Lighting
└── Interactables
    └── HouseEntrance (InteriorEntrance)
        ├── Collider (isTrigger: true)
        └── interiorSceneName: "HouseInterior"
```

### 3. HouseInterior Scene Setup

```
Hierarchy:
├── HouseContent
│   ├── Furniture
│   ├── Lighting
│   └── Props
├── PlayerSpawn (InteriorSpawnPoint: Entrance)
└── HouseExit (InteriorExit)
    └── Collider (isTrigger: true)
```

## 🎨 Ventajas del Sistema Manual

### 🚀 Control Total

- **Referencias explícitas** en inspector
- **Sin auto-detección** que pueda fallar
- **Configuración visible** y modificable
- **Debug claro** con warnings específicos

### 🎮 Flexibilidad

- **Asignación manual** de todos los componentes
- **LoadingUISetup opcional** para creación automática
- **Managers independientes** configurables por separado
- **Performance optimizada** con environment manual

### 🔧 Mantenimiento

- **Referencias claras** en inspector
- **Validation automática** con warnings
- **Setup modular** sin dependencias ocultas
- **Error handling** robusto con mensajes claros

## 🔄 Flujo de Setup Recomendado

### Paso 1: Crear Managers

1. Crear GameObject "GameSceneManager" con componente
2. Crear Canvas con componente "CanvasManager"  
3. (Opcional) Añadir "LoadingUISetup" para crear LoadingUI

### Paso 2: Asignar Referencias

1. En GameSceneManager: Asignar LoadingUI, Environment, CanvasManager
2. En CanvasManager: Asignar LoadingUI
3. Verificar que no hay warnings en console

### Paso 3: Configurar Escenas

1. Añadir InteriorSpawnPoints en interiores
2. Configurar InteriorEntrance/Exit con nombres de escenas
3. Asignar tags o referencias de environment

¡Sistema manual completo y robusto listo para usar! 🚀
