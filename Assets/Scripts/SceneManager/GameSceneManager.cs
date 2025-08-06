using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum SceneLoadType {
    Single,
    Additive
}

public class GameSceneManager : MonoBehaviour {
    [Header("Required References")]
    [SerializeField] private CanvasManager canvasManager;

    [Header("Optional References")]
    [SerializeField] private GameObject mainSceneEnvironment;

    private string currentMainScene = "";
    private string currentInteriorScene = "";
    private bool isInInterior = false;

    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private PlayerController playerController;
    private Transform originalPlayerParent;

    private LoadingUI loadingUI;

    public static GameSceneManager Instance { get; private set; }

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
        loadingUI = canvasManager.GetComponentInChildren<LoadingUI>();

        ValidateReferences();

        currentMainScene = SceneManager.GetActiveScene().name;

        if (currentMainScene.Contains("MainMenu")) {
            GameEvents.OnSetGameStateRequested?.Invoke(GameState.MainMenu);
        }
    }

    private void ValidateReferences() {
        if (canvasManager == null) {
            Debug.LogWarning("[GameSceneManager] CanvasManager reference not assigned! Loading screens might not work properly.");
        }
    }

    public void LoadInteriorScene(string sceneName, string spawnPointID = "") {
        if (mainSceneEnvironment == null) {
            AutoDetectMainEnvironment();
        }

        if (mainSceneEnvironment == null) {
            Debug.LogWarning("[GameSceneManager] No main scene environment found! Performance optimization won't work. Consider tagging your environment GameObject with 'Environment'.");
        }

        StartCoroutine(LoadSceneCoroutine(sceneName, SceneLoadType.Additive, spawnPointID));
    }

    public void LoadSingleScene(string sceneName) {
        Debug.Log($"[GameSceneManager] LoadSingleScene called with: {sceneName}");

        // Check if scene exists in build settings
#if UNITY_EDITOR
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
        if (sceneIndex == -1) {
            Debug.LogError($"[GameSceneManager] Scene '{sceneName}' not found in Build Settings! Please add it to Build Settings.");
            return;
        }
#endif

        // Store current timeScale and temporarily restore it for scene loading
        float originalTimeScale = Time.timeScale;
        if (Time.timeScale == 0f) {
            Debug.Log("[GameSceneManager] Time is paused, temporarily restoring timeScale for scene loading");
            Time.timeScale = 1f;
        }

        StartCoroutine(LoadSceneCoroutineWithTimeScale(sceneName, SceneLoadType.Single, originalTimeScale));
    }

    public void ExitInterior() {
        if (!isInInterior) {
            Debug.LogWarning("[GameSceneManager] Not currently in an interior scene!");
            return;
        }

        StartCoroutine(ExitInteriorCoroutine());
    }

    private IEnumerator LoadSceneCoroutineWithTimeScale(string sceneName, SceneLoadType loadType, float originalTimeScale, string spawnPointID = "") {
        Debug.Log($"[GameSceneManager] Starting LoadSceneCoroutineWithTimeScale with originalTimeScale: {originalTimeScale}");

        // Ensure time is running for the coroutine to work properly
        if (Time.timeScale == 0f) {
            Time.timeScale = 1f;
        }

        yield return StartCoroutine(LoadSceneCoroutine(sceneName, loadType, spawnPointID));

        // Restore original timeScale after scene loading is complete
        Debug.Log($"[GameSceneManager] Scene loading complete, restoring timeScale to: {originalTimeScale}");
        Time.timeScale = originalTimeScale;
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, SceneLoadType loadType, string spawnPointID = "") {
        string loadTypeString = loadType == SceneLoadType.Single ? "Single" : "Additive";
        string actionString = loadType == SceneLoadType.Single ? "Loading scene" : "Entering interior";
        Debug.Log($"[GameSceneManager] {actionString} ({loadTypeString}): {sceneName}");

        // Set appropriate loading messages before showing loading screen
        string loadingTypeString = loadType == SceneLoadType.Single ? "single" : "interior_entry";

        if (loadingUI != null) {
            loadingUI.SetLoadingType(loadingTypeString);
            loadingUI.ShowLoadingScreen();
        }
        else if (canvasManager != null) {
            canvasManager.ShowLoading(loadingTypeString);
        }

        if (loadType == SceneLoadType.Additive && playerController != null) {
            playerController.enabled = false;
        }

        if (loadType == SceneLoadType.Additive && mainSceneEnvironment != null) {
            // Preserve player by moving it out of environment before deactivating
            if (playerController == null) {
                playerController = FindObjectOfType<PlayerController>();
            }

            if (playerController != null) {
                originalPlayerPosition = playerController.transform.parent.position;
                originalPlayerRotation = playerController.transform.parent.rotation;

                Transform playerParent = playerController.transform.parent; // This is the Controllable GameObject
                if (IsChildOfEnvironment(playerParent, mainSceneEnvironment.transform)) {
                    Debug.Log($"[GameSceneManager] Moving player out of environment to preserve it during interior entry");
                    originalPlayerParent = playerParent.parent;
                    playerParent.SetParent(null);
                }
            }

            Debug.Log($"[GameSceneManager] Deactivating environment for interior entry: {mainSceneEnvironment.name}");
            mainSceneEnvironment.SetActive(false);
        }

        LoadSceneMode sceneMode = loadType == SceneLoadType.Additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, sceneMode);
        loadOperation.allowSceneActivation = false;

        yield return StartCoroutine(FakeLoadingCoroutine(loadOperation));

        loadOperation.allowSceneActivation = true;
        yield return loadOperation;

        if (loadType == SceneLoadType.Additive) {
            yield return StartCoroutine(SetupInteriorScene(sceneName, spawnPointID));
        }
        else {
            yield return StartCoroutine(SetupMainScene(sceneName));
        }

        if (loadingUI != null) {
            loadingUI.HideLoadingScreen();
        }
        else if (canvasManager != null) {
            canvasManager.HideLoading();
        }

        string completedAction = loadType == SceneLoadType.Single ? "Scene loaded" : "Interior entered";
        Debug.Log($"[GameSceneManager] {completedAction} successfully: {sceneName}");
    }

    private IEnumerator FakeLoadingCoroutine(AsyncOperation loadOperation, bool isExit = false) {
        float fakeProgress = 0f;
        float loadingTime = isExit ? 3f : 6f; // Shorter loading for exits
        float elapsedTime = 0f;

        while (elapsedTime < loadingTime || loadOperation.progress < 0.9f) {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time to work during pause
            fakeProgress = Mathf.Clamp01(elapsedTime / loadingTime);

            float realProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            float combinedProgress = Mathf.Max(fakeProgress, realProgress);

            if (loadingUI != null) {
                loadingUI.UpdateProgress(combinedProgress);
            }
            else if (canvasManager != null) {
                canvasManager.UpdateLoadingProgress(combinedProgress);
            }

            yield return null;
        }

        if (loadingUI != null) {
            loadingUI.UpdateProgress(1f);
        }
        else if (canvasManager != null) {
            canvasManager.UpdateLoadingProgress(1f);
        }

        yield return new WaitForSecondsRealtime(0.5f); // Use realtime to work during pause
    }

    private IEnumerator SetupInteriorScene(string sceneName, string spawnPointID) {
        yield return null;

        currentInteriorScene = sceneName;
        isInInterior = true;

        if (playerController == null) {
            // First try to find active PlayerController
            playerController = FindObjectOfType<PlayerController>();

            // If not found, look for inactive ones too
            if (playerController == null) {
                PlayerController[] allPlayers = FindObjectsOfType<PlayerController>(true);
                foreach (var player in allPlayers) {
                    if (player.transform.parent.gameObject.activeInHierarchy) {
                        playerController = player;
                        break;
                    }
                }
            }
        }

        if (playerController != null) {
            // Ensure player's parent (Controllable) is active
            if (!playerController.transform.parent.gameObject.activeInHierarchy) {
                playerController.transform.parent.gameObject.SetActive(true);
            }

            InteriorSpawnPoint spawnPoint = FindSpawnPoint(spawnPointID);
            if (spawnPoint != null) {
                Vector3 spawnPosition = spawnPoint.transform.position;
                Quaternion spawnRotation = spawnPoint.transform.rotation;

                playerController.transform.parent.position = spawnPosition;
                playerController.transform.parent.rotation = spawnRotation;
            }

            // SYNC: Update PlayerController's IsInInterior state
            playerController.SetIsInInterior(true);

            playerController.enabled = true;

            // Final validation
            if (playerController.enabled && playerController.transform.parent.gameObject.activeInHierarchy) {
                Debug.Log($"[GameSceneManager] Player successfully set up in interior: {sceneName}");
            }
            else {
                Debug.LogError($"[GameSceneManager] Player setup failed! Enabled: {playerController.enabled}, Parent Active: {playerController.transform.parent.gameObject.activeInHierarchy}");
            }
        }
        else {
            Debug.LogError("[GameSceneManager] PlayerController not found! Make sure player exists and is active.");
        }
    }

    private IEnumerator ExitInteriorCoroutine() {
        // Set exit loading messages before showing loading screen
        if (loadingUI != null) {
            loadingUI.SetLoadingType("interior_exit");
            loadingUI.ShowLoadingScreen();
        }
        else if (canvasManager != null) {
            canvasManager.ShowLoading("interior_exit");
        }

        if (playerController != null) {
            playerController.enabled = false;
        }

        Debug.Log($"[GameSceneManager] Exiting interior: {currentInteriorScene}");

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentInteriorScene);
        unloadOperation.allowSceneActivation = false;

        // Use fake loading for exit transition
        yield return StartCoroutine(FakeLoadingCoroutine(unloadOperation, true)); // true = is exit

        unloadOperation.allowSceneActivation = true;
        yield return unloadOperation;

        // First reactivate environment and restore player hierarchy
        if (mainSceneEnvironment != null) {
            Debug.Log($"[GameSceneManager] Reactivating environment: {mainSceneEnvironment.name}");
            mainSceneEnvironment.SetActive(true);

            // Wait for environment to be fully active and physics to settle
            yield return new WaitForFixedUpdate();

            // Restore player to original parent if it was moved
            if (playerController != null && originalPlayerParent != null) {
                Transform playerParent = playerController.transform.parent; // Controllable GameObject
                if (playerParent.parent == null) { // Controllable is at root level
                    Debug.Log($"[GameSceneManager] Restoring player to original parent in environment");
                    playerParent.SetParent(originalPlayerParent);
                    originalPlayerParent = null;
                }
            }
        }

        // Then restore player position and enable
        if (playerController != null) {
            // Ensure player is at a safe height above ground
            Vector3 safePosition = originalPlayerPosition;
            safePosition.y += 2f; // Add 2 units above original position as safety margin

            playerController.transform.parent.position = safePosition;
            playerController.transform.parent.rotation = originalPlayerRotation;

            // SYNC: Update PlayerController's IsInInterior state
            playerController.SetIsInInterior(false);

            // Wait one more frame before enabling player
            yield return null;
            playerController.enabled = true;

            Debug.Log($"[GameSceneManager] Player restored to position: {safePosition}");
        }

        isInInterior = false;
        string exitedScene = currentInteriorScene;
        currentInteriorScene = "";

        if (loadingUI != null) {
            loadingUI.HideLoadingScreen();
        }
        else if (canvasManager != null) {
            canvasManager.HideLoading();
        }

        Debug.Log($"[GameSceneManager] Interior exit completed successfully: {exitedScene}");
    }

    private IEnumerator SetupMainScene(string sceneName) {
        yield return null;

        currentMainScene = sceneName;
        isInInterior = false;
        currentInteriorScene = "";

        // SYNC: Update PlayerController's IsInInterior state if available
        if (playerController == null) {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (playerController != null) {
            playerController.SetIsInInterior(false);
        }

        if (mainSceneEnvironment == null) {
            AutoDetectMainEnvironment();
        }
    }

    private InteriorSpawnPoint FindSpawnPoint(string spawnPointID) {
        InteriorSpawnPoint[] allSpawnPoints = FindObjectsOfType<InteriorSpawnPoint>();

        if (allSpawnPoints.Length == 0) {
            return null;
        }

        if (!string.IsNullOrEmpty(spawnPointID)) {
            foreach (var spawnPoint in allSpawnPoints) {
                if (spawnPoint.SpawnPointID == spawnPointID) {
                    return spawnPoint;
                }
            }
        }

        foreach (var spawnPoint in allSpawnPoints) {
            if (spawnPoint.PointType == SpawnPointType.Entrance) {
                return spawnPoint;
            }
        }

        return allSpawnPoints[0];
    }

    public void LoadMainGameScene(string sceneName) {
        GameEvents.OnStartGameRequested?.Invoke();
        LoadSingleScene(sceneName);
    }

    public void ReturnToMainMenu(string menuSceneName = "MainMenu") {
        LoadSingleScene(menuSceneName);
        GameEvents.OnReturnToMenuRequested?.Invoke();
    }

    public void LoadCreditsScene(string creditsSceneName = "Credits") {
        Debug.Log($"[GameSceneManager] LoadCreditsScene called with scene: {creditsSceneName}");
        Debug.Log($"[GameSceneManager] Current timeScale: {Time.timeScale}");

        // Load scene first, then clean up after it's loaded
        StartCoroutine(LoadCreditsSceneCoroutine(creditsSceneName));
    }

    public void SetLoadingUI(LoadingUI loading) {
        loadingUI = loading;
    }

    public void AutoDetectMainEnvironment() {
        GameObject envObject = GameObject.FindWithTag("Environment");
        if (envObject != null) {
            mainSceneEnvironment = envObject;
        }
    }

    private bool IsChildOfEnvironment(Transform target, Transform environment) {
        if (target == null || environment == null) return false;

        Transform current = target;
        while (current != null) {
            if (current == environment) {
                return true;
            }
            current = current.parent;
        }
        return false;
    }

    private IEnumerator LoadCreditsSceneCoroutine(string creditsSceneName) {
        Debug.Log($"[GameSceneManager] Starting LoadCreditsSceneCoroutine for: {creditsSceneName}");

        // Store current timeScale and temporarily restore it for scene loading
        float originalTimeScale = Time.timeScale;
        if (Time.timeScale == 0f) {
            Debug.Log("[GameSceneManager] Time is paused, temporarily restoring timeScale for scene loading");
            Time.timeScale = 1f;
        }

        // Check if scene exists in build settings
#if UNITY_EDITOR
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(creditsSceneName);
        if (sceneIndex == -1) {
            Debug.LogError($"[GameSceneManager] Scene '{creditsSceneName}' not found in Build Settings! Please add it to Build Settings.");
            yield break;
        }
#endif

        // Load the credits scene
        yield return StartCoroutine(LoadSceneCoroutine(creditsSceneName, SceneLoadType.Single));

        // Wait a bit to ensure the scene is fully loaded and active
        yield return new WaitForSecondsRealtime(0.5f);

        // Now clean up DontDestroyOnLoad objects after the scene is loaded
        Debug.Log("[GameSceneManager] Scene loaded successfully, now cleaning up DontDestroyOnLoad objects...");
        CleanupDontDestroyOnLoadObjects();

        // Finally, destroy this GameSceneManager as well
        Debug.Log("[GameSceneManager] Cleanup complete, destroying GameSceneManager...");
        Destroy(gameObject);
    }

    private void CleanupDontDestroyOnLoadObjects() {
        Debug.Log("[GameSceneManager] Cleaning up DontDestroyOnLoad objects...");

        // Find all root objects in the DontDestroyOnLoad scene
        GameObject[] dontDestroyObjects = FindObjectsOfType<GameObject>();
        int destroyedCount = 0;

        foreach (GameObject obj in dontDestroyObjects) {
            // Only consider root objects (no parent)
            if (obj.transform.parent == null && obj.scene.name == "DontDestroyOnLoad") {
                // Skip essential Unity objects (like the event system)
                if (ShouldDestroyDontDestroyOnLoadObject(obj)) {
                    Debug.Log($"[GameSceneManager] Destroying DontDestroyOnLoad object: {obj.name}");
                    Destroy(obj);
                    destroyedCount++;
                }
            }
        }

        Debug.Log($"[GameSceneManager] Cleaned up {destroyedCount} DontDestroyOnLoad objects");
    }

    private bool ShouldDestroyDontDestroyOnLoadObject(GameObject obj) {
        // List of objects we want to keep (essential Unity systems)
        string[] keepObjects = {
            "EventSystem",
            "Main Camera", // In case there's a main camera marked as DontDestroyOnLoad
        };

        // List of game objects we specifically want to destroy
        string[] destroyObjects = {
            "GameController",
            "InteriorSceneManager",
            "CanvasManager",
            "KeyInventoryManager",
            "AudioManager"
            // Note: GameSceneManager will destroy itself after cleanup
        };

        // Don't destroy essential Unity objects
        foreach (string keepName in keepObjects) {
            if (obj.name.Contains(keepName)) {
                Debug.Log($"[GameSceneManager] Keeping essential object: {obj.name}");
                return false;
            }
        }

        // Specifically destroy our game managers
        foreach (string destroyName in destroyObjects) {
            if (obj.name.Contains(destroyName)) {
                Debug.Log($"[GameSceneManager] Will destroy game manager: {obj.name}");
                return true;
            }
        }

        // For any other objects, log them but destroy them (safer approach)
        Debug.Log($"[GameSceneManager] Unknown DontDestroyOnLoad object, will destroy: {obj.name}");
        return true;
    }
}