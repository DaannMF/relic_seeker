using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteriorSceneManager : MonoBehaviour {
    public static InteriorSceneManager Instance { get; private set; }

    [Header("Scene Management")]
    [SerializeField] private LoadingUI loadingUI;

    private string currentInteriorScene = "";
    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private PlayerController playerController;

    // Events
    public System.Action<string> OnInteriorEntered;
    public System.Action<string> OnInteriorExited;

    private void Awake() {
        // Singleton pattern
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        bool includeInactive = true;
        if (loadingUI == null) loadingUI = FindObjectOfType<LoadingUI>(includeInactive);
    }

    public void EnterInterior(string sceneName, string spawnPointID = "") {
        if (string.IsNullOrEmpty(sceneName)) {
            Debug.LogError("Scene name cannot be null or empty!");
            return;
        }

        // Find player
        if (playerController == null) {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (playerController == null) {
            Debug.LogError("PlayerController not found!");
            return;
        }

        // Store current position for return
        originalPlayerPosition = playerController.transform.parent.position;

        StartCoroutine(LoadInteriorScene(sceneName, spawnPointID));
    }

    public void ExitInterior() {
        if (string.IsNullOrEmpty(currentInteriorScene)) {
            Debug.LogWarning("No interior scene to exit from!");
            return;
        }

        StartCoroutine(UnloadInteriorScene());
    }

    private IEnumerator LoadInteriorScene(string sceneName, string spawnPointID) {
        // Show loading UI
        if (loadingUI != null) {
            loadingUI.ShowLoadingScreen();
        }

        // Disable player temporarily
        if (playerController != null) {
            playerController.enabled = false;
        }

        // Start loading the scene additively
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false;

        float fakeProgress = 0f;
        float startTime = Time.time;
        float minLoadTime = 6f;

        while (fakeProgress < 1f || (Time.time - startTime) < minLoadTime) {
            fakeProgress += Time.deltaTime * 0.8f;

            float realProgress = loadOperation.progress / 0.9f;
            float displayProgress = Mathf.Min(fakeProgress, realProgress);

            if (loadingUI != null) {
                loadingUI.UpdateProgress(displayProgress);
            }

            yield return null;
        }

        loadOperation.allowSceneActivation = true;
        yield return loadOperation;

        InteriorSpawnPoint spawnPoint = FindSpawnPoint(spawnPointID);

        if (spawnPoint != null) {
            playerController.transform.parent.position = spawnPoint.Position;
            playerController.transform.parent.rotation = spawnPoint.Rotation;
            playerController.SetIsInInterior(true);
        }
        else {
            Debug.LogWarning($"No suitable spawn point found in scene {sceneName}. Player will remain at current position.");
        }

        if (playerController != null) {
            playerController.enabled = true;
        }

        if (loadingUI != null) {
            loadingUI.HideLoadingScreen();
        }

        currentInteriorScene = sceneName;
        OnInteriorEntered?.Invoke(sceneName);

        Debug.Log($"Entered interior: {sceneName}");
    }

    private IEnumerator UnloadInteriorScene() {
        if (string.IsNullOrEmpty(currentInteriorScene)) {
            yield break;
        }

        if (loadingUI != null) {
            loadingUI.ShowLoadingScreen();
        }

        if (playerController != null) {
            playerController.enabled = false;
        }

        float progress = 0f;
        float startTime = Time.time;
        float minLoadTime = 3f;

        while (progress < 1f || (Time.time - startTime) < minLoadTime) {
            progress += Time.deltaTime * 1.5f;

            if (loadingUI != null) {
                loadingUI.UpdateProgress(progress);
            }

            yield return null;
        }

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentInteriorScene);
        yield return unloadOperation;

        if (playerController != null) {
            playerController.transform.parent.position = originalPlayerPosition;
            playerController.transform.parent.rotation = originalPlayerRotation;
            playerController.enabled = true;
            playerController.SetIsInInterior(false);
        }

        if (loadingUI != null) {
            loadingUI.HideLoadingScreen();
        }

        string exitedScene = currentInteriorScene;
        currentInteriorScene = "";
        OnInteriorExited?.Invoke(exitedScene);

        Debug.Log($"Exited interior: {exitedScene}");
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
}