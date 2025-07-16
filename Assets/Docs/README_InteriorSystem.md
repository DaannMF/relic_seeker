# Interior Scene Loading System

A complete system for loading interior scenes additively with fake loading screens and automatic player positioning using spawn points.

## 📁 Files Overview

### Core Components

- **`InteriorSceneManager.cs`** - Main singleton manager for scene transitions
- **`LoadingUI.cs`** - Handles the loading screen UI with progress bar and rotating messages
- **`InteriorEntrance.cs`** - Trigger component for entering interior scenes
- **`InteriorExit.cs`** - Trigger component for exiting interior scenes
- **`InteriorSpawnPoint.cs`** - Marks spawn positions in interior scenes
- **`InteriorSystemSetup.cs`** - Auto-configuration helper

### Support Files

- **`README_InteriorSystem.md`** - This documentation
- Various `.meta` files for proper Unity organization

## 🚀 Quick Setup

### Automatic Setup (Recommended)

1. Add `InteriorSystemSetup` component to any GameObject in your main scene
2. Configure auto-setup options in inspector:
   - ✅ Create Interior Manager
   - ✅ Create Loading UI
3. The system will automatically create all necessary components

### Manual Setup

1. **Create Interior Scene Manager**:
   - Add `InteriorSceneManager` to any GameObject
   - Assign `LoadingUI` reference

2. **Create Loading UI**:
   - Create UI Canvas if needed
   - Add `LoadingUI` component
   - Configure UI references (panel, progress bar, texts)

## 🎯 How to Use

### 1. Setting Up Entrances

1. Create an entrance GameObject with a **Collider** (set as trigger)
2. Add `InteriorEntrance` component
3. Configure:
   - **Interior Scene Name**: Name of scene to load
   - **Spawn Point ID**: (Optional) Specific spawn point to use
   - **Interaction Prompt**: UI element to show "Press E"

### 2. Setting Up Interior Scenes

1. Create your interior scene
2. Add at least one **spawn point**:
   - Create empty GameObject
   - Add `InteriorSpawnPoint` component
   - Configure:
     - **Point Type**: Entrance (where player spawns when entering)
     - **Spawn Point ID**: (Optional) Unique identifier
3. Position and rotate the spawn point as desired

### 3. Setting Up Exits

1. In interior scene, create exit GameObject with **Collider** (trigger)
2. Add `InteriorExit` component
3. Configure:
   - **Auto Exit**: If true, exits automatically on touch
   - **Interaction Prompt**: UI for manual exit

## 🎮 Spawn Point System

### InteriorSpawnPoint Features

- **Visual Gizmos**: See spawn points in Scene view
  - 🟢 Green = Entrance spawn points
  - 🔵 Blue = Exit spawn points
- **Automatic Detection**: InteriorSceneManager finds spawn points automatically
- **Flexible Configuration**: Use IDs for specific spawn points

### Spawn Point Types

```csharp
public enum SpawnPointType
{
    Entrance,  // Where player spawns when entering from exterior
    Exit       // Where player spawns when returning from another interior
}
```

### How Spawn Point Selection Works

1. If **Spawn Point ID** is specified in entrance, finds exact match
2. Otherwise, looks for **Entrance** type spawn points
3. Falls back to first available spawn point
4. If no spawn points found, player stays at current position

## ⚙️ Technical Features

### Scene Loading

- **Additive Loading**: Main scene stays loaded
- **Async Operations**: Non-blocking scene loading
- **Fake Progress Bar**: Minimum 1-second loading time for polish
- **Automatic Position Management**: Saves/restores original player position

### Player Management

- **Temporary Disable**: Prevents movement during transitions
- **Position Restoration**: Returns to exact original position on exit
- **Rotation Preservation**: Maintains both position and rotation

### Event System

```csharp
// Subscribe to events
InteriorSceneManager.Instance.OnInteriorEntered += (sceneName) => { };
InteriorSceneManager.Instance.OnInteriorExited += (sceneName) => { };
```

## 🔧 Customization Options

### Loading UI Messages

Edit the `loadingMessages` array in `LoadingUI`:

```csharp
private string[] loadingMessages = {
    "Loading...",
    "Entering Interior...",
    "Preparing Environment...",
    "Almost Ready..."
};
```

### Loading Times

Adjust in `InteriorSceneManager`:

```csharp
float minLoadTime = 1f; // Entrance loading time
float minLoadTime = 0.5f; // Exit loading time (faster)
```

### Spawn Point Gizmos

InteriorSpawnPoint shows visual indicators:

- Circle with directional arrow
- Color-coded by type
- Inspector labels with details

## 🐛 Debugging Tips

### Common Issues

1. **"No spawn point found"** - Add InteriorSpawnPoint to interior scene
2. **Player not moving to spawn** - Check spawn point position/rotation
3. **Loading UI not showing** - Verify LoadingUI references are assigned
4. **Scene not loading** - Check scene name spelling and build settings

### Debug Features

- Console logs for entering/exiting scenes
- Gizmos show entrance/exit/spawn positions
- Warning messages for missing components

### Validation

- Script validates scene names exist
- Checks for required components
- Provides helpful error messages

## 📋 Example Workflow

### Creating a House Interior

1. **Main Scene Setup**:
   - Place house model
   - Add door with `InteriorEntrance`
   - Set scene name: "HouseInterior"

2. **Interior Scene Creation**:
   - Create "HouseInterior" scene
   - Add furniture, props
   - Place `InteriorSpawnPoint` near door (Type: Entrance)
   - Add exit trigger with `InteriorExit`

3. **Testing**:
   - Walk to door → "Press E" appears
   - Press E → Loading screen → Inside house
   - Walk to exit → Press E → Back outside

## 🎨 Visual Flow Diagram

```
[Player approaches entrance]
           ↓
[InteriorEntrance detects player]
           ↓
[Shows "Press E" prompt]
           ↓
[Player presses E]
           ↓
[InteriorSceneManager.EnterInterior()]
           ↓
[Loading screen shows]
           ↓
[Scene loads additively]
           ↓
[Finds InteriorSpawnPoint]
           ↓
[Moves player to spawn point]
           ↓
[Hide loading screen]
           ↓
[Player in interior scene]
           ↓
[Walk to exit → Press E]
           ↓
[InteriorSceneManager.ExitInterior()]
           ↓
[Return to original position]
```

## 📝 Script References

### InteriorSceneManager Methods

- `EnterInterior(sceneName, spawnPointID = "")` - Enter interior scene
- `ExitInterior()` - Exit current interior (returns to original position)
- `IsInInterior()` - Check if currently in interior
- `GetCurrentInteriorScene()` - Get current interior scene name

### InteriorSpawnPoint Properties

- `PointType` - Entrance or Exit type
- `SpawnPointID` - Optional unique identifier
- `Position` - World position for spawning
- `Rotation` - World rotation for spawning

This system provides a professional, polished experience for interior scene transitions with minimal setup required!
