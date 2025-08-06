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

        currentMainScene = SceneManager.GetActiveScene().name;

        if (currentMainScene.Contains("MainMenu"))
            GameEvents.OnSetGameStateRequested?.Invoke(GameState.MainMenu);
    }

    public void LoadInteriorScene(string sceneName, string spawnPointID = "") {
        if (mainSceneEnvironment == null)
            AutoDetectMainEnvironment();

        StartCoroutine(LoadSceneCoroutine(sceneName, SceneLoadType.Additive, spawnPointID));
    }

    public void LoadSingleScene(string sceneName) {
        float originalTimeScale = Time.timeScale;
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;

        StartCoroutine(LoadSceneCoroutineWithTimeScale(sceneName, SceneLoadType.Single, originalTimeScale));
    }

    public void ExitInterior() {
        if (!isInInterior) return;

        StartCoroutine(ExitInteriorCoroutine());
    }

    private IEnumerator LoadSceneCoroutineWithTimeScale(string sceneName, SceneLoadType loadType, float originalTimeScale, string spawnPointID = "") {
        if (Time.timeScale == 0f) Time.timeScale = 1f;

        yield return StartCoroutine(LoadSceneCoroutine(sceneName, loadType, spawnPointID));

        Time.timeScale = originalTimeScale;
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, SceneLoadType loadType, string spawnPointID = "") {
        string loadingTypeString = loadType == SceneLoadType.Single ? "single" : "interior_entry";

        if (loadingUI != null) {
            loadingUI.SetLoadingType(loadingTypeString);
            loadingUI.ShowLoadingScreen();
        }
        else if (canvasManager != null)
            canvasManager.ShowLoading(loadingTypeString);

        if (loadType == SceneLoadType.Additive && playerController != null)
            playerController.enabled = false;

        if (loadType == SceneLoadType.Additive && mainSceneEnvironment != null) {
            if (playerController == null)
                playerController = FindObjectOfType<PlayerController>();

            if (playerController != null) {
                originalPlayerPosition = playerController.transform.parent.position;
                originalPlayerRotation = playerController.transform.parent.rotation;

                Transform playerParent = playerController.transform.parent;
                if (IsChildOfEnvironment(playerParent, mainSceneEnvironment.transform)) {
                    originalPlayerParent = playerParent.parent;
                    playerParent.SetParent(null);
                }
            }

            mainSceneEnvironment.SetActive(false);
        }

        LoadSceneMode sceneMode = loadType == SceneLoadType.Additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, sceneMode);
        loadOperation.allowSceneActivation = false;

        yield return StartCoroutine(FakeLoadingCoroutine(loadOperation));

        loadOperation.allowSceneActivation = true;
        yield return loadOperation;

        if (loadType == SceneLoadType.Additive)
            yield return StartCoroutine(SetupInteriorScene(sceneName, spawnPointID));
        else
            yield return StartCoroutine(SetupMainScene(sceneName));

        if (loadingUI != null)
            loadingUI.HideLoadingScreen();
        else if (canvasManager != null)
            canvasManager.HideLoading();
    }

    private IEnumerator FakeLoadingCoroutine(AsyncOperation loadOperation, bool isExit = false) {
        float fakeProgress;
        float loadingTime = isExit ? 3f : 6f;
        float elapsedTime = 0f;

        while (elapsedTime < loadingTime || loadOperation.progress < 0.9f) {
            elapsedTime += Time.unscaledDeltaTime;
            fakeProgress = Mathf.Clamp01(elapsedTime / loadingTime);

            float realProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            float combinedProgress = Mathf.Max(fakeProgress, realProgress);

            if (loadingUI != null)
                loadingUI.UpdateProgress(combinedProgress);
            else if (canvasManager != null)
                canvasManager.UpdateLoadingProgress(combinedProgress);

            yield return null;
        }

        if (loadingUI != null)
            loadingUI.UpdateProgress(1f);
        else if (canvasManager != null)
            canvasManager.UpdateLoadingProgress(1f);

        yield return new WaitForSecondsRealtime(0.5f);
    }

    private IEnumerator SetupInteriorScene(string sceneName, string spawnPointID) {
        yield return null;

        currentInteriorScene = sceneName;
        isInInterior = true;

        if (playerController == null) {
            playerController = FindObjectOfType<PlayerController>();

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
            if (!playerController.transform.parent.gameObject.activeInHierarchy)
                playerController.transform.parent.gameObject.SetActive(true);

            InteriorSpawnPoint spawnPoint = FindSpawnPoint(spawnPointID);
            if (spawnPoint != null) {
                Vector3 spawnPosition = spawnPoint.transform.position;
                Quaternion spawnRotation = spawnPoint.transform.rotation;

                playerController.transform.parent.position = spawnPosition;
                playerController.transform.parent.rotation = spawnRotation;
            }

            playerController.SetIsInInterior(true);

            playerController.enabled = true;
        }
    }

    private IEnumerator ExitInteriorCoroutine() {
        if (loadingUI != null) {
            loadingUI.SetLoadingType("interior_exit");
            loadingUI.ShowLoadingScreen();
        }
        else if (canvasManager != null)
            canvasManager.ShowLoading("interior_exit");

        if (playerController != null)
            playerController.enabled = false;

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentInteriorScene);
        unloadOperation.allowSceneActivation = false;

        yield return StartCoroutine(FakeLoadingCoroutine(unloadOperation, true));

        unloadOperation.allowSceneActivation = true;
        yield return unloadOperation;

        if (mainSceneEnvironment != null) {
            mainSceneEnvironment.SetActive(true);

            yield return new WaitForFixedUpdate();

            if (playerController != null && originalPlayerParent != null) {
                Transform playerParent = playerController.transform.parent;
                if (playerParent.parent == null) {
                    playerParent.SetParent(originalPlayerParent);
                    originalPlayerParent = null;
                }
            }
        }

        if (playerController != null) {
            Vector3 safePosition = originalPlayerPosition;
            safePosition.y += 2f;

            playerController.transform.parent.position = safePosition;
            playerController.transform.parent.rotation = originalPlayerRotation;

            playerController.SetIsInInterior(false);

            yield return null;
            playerController.enabled = true;
        }

        isInInterior = false;
        currentInteriorScene = "";

        if (loadingUI != null)
            loadingUI.HideLoadingScreen();
        else if (canvasManager != null)
            canvasManager.HideLoading();
    }

    private IEnumerator SetupMainScene(string sceneName) {
        yield return null;

        currentMainScene = sceneName;
        isInInterior = false;
        currentInteriorScene = "";


        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (playerController != null)
            playerController.SetIsInInterior(false);

        if (mainSceneEnvironment == null)
            AutoDetectMainEnvironment();
    }

    private InteriorSpawnPoint FindSpawnPoint(string spawnPointID) {
        InteriorSpawnPoint[] allSpawnPoints = FindObjectsOfType<InteriorSpawnPoint>();

        if (allSpawnPoints.Length == 0)
            return null;

        if (!string.IsNullOrEmpty(spawnPointID))
            foreach (var spawnPoint in allSpawnPoints)
                if (spawnPoint.SpawnPointID == spawnPointID)
                    return spawnPoint;

        foreach (var spawnPoint in allSpawnPoints)
            if (spawnPoint.PointType == SpawnPointType.Entrance)
                return spawnPoint;

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
        StartCoroutine(LoadCreditsSceneCoroutine(creditsSceneName));
    }

    public void SetLoadingUI(LoadingUI loading) {
        loadingUI = loading;
    }

    public void AutoDetectMainEnvironment() {
        GameObject envObject = GameObject.FindWithTag("Environment");
        if (envObject != null)
            mainSceneEnvironment = envObject;
    }

    private bool IsChildOfEnvironment(Transform target, Transform environment) {
        if (target == null || environment == null) return false;

        Transform current = target;
        while (current != null) {
            if (current == environment)
                return true;

            current = current.parent;
        }
        return false;
    }

    private IEnumerator LoadCreditsSceneCoroutine(string creditsSceneName) {
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;

        yield return StartCoroutine(LoadSceneCoroutine(creditsSceneName, SceneLoadType.Single));
        yield return new WaitForSecondsRealtime(0.5f);

        CleanupDontDestroyOnLoadObjects();
        Destroy(gameObject);
    }

    private void CleanupDontDestroyOnLoadObjects() {
        GameObject[] dontDestroyObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in dontDestroyObjects)
            if (obj.transform.parent == null && obj.scene.name == "DontDestroyOnLoad")
                if (ShouldDestroyDontDestroyOnLoadObject(obj))
                    Destroy(obj);
    }

    private bool ShouldDestroyDontDestroyOnLoadObject(GameObject obj) {
        string[] keepObjects = {
            "EventSystem",
            "Main Camera"
        };

        string[] destroyObjects = {
            "GameController",
            "InteriorSceneManager",
            "CanvasManager",
            "KeyInventoryManager",
            "AudioManager"
        };

        foreach (string keepName in keepObjects)
            if (obj.name.Contains(keepName))
                return false;

        foreach (string destroyName in destroyObjects)
            if (obj.name.Contains(destroyName))
                return true;

        return true;
    }
}