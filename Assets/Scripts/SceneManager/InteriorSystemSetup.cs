using UnityEngine;
using UnityEngine.UI;

public class InteriorSystemSetup : MonoBehaviour {
    [Header("Auto Setup")]
    [SerializeField] private bool createLoadingUI = true;
    [SerializeField] private bool createInteriorManager = true;

    [Header("Loading UI Settings")]
    [SerializeField] private Canvas uiCanvas;

    private void Start() {
        if (createInteriorManager) {
            SetupInteriorManager();
        }

        if (createLoadingUI) {
            SetupLoadingUI();
        }
    }

    private void SetupInteriorManager() {
        // Check if InteriorSceneManager already exists
        if (InteriorSceneManager.Instance == null) {
            GameObject managerObj = new GameObject("InteriorSceneManager");
            managerObj.AddComponent<InteriorSceneManager>();
            Debug.Log("InteriorSceneManager created automatically.");
        }
    }

    private void SetupLoadingUI() {
        // Find or create canvas
        if (uiCanvas == null) {
            uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas == null) {
                GameObject canvasObj = new GameObject("UI Canvas");
                uiCanvas = canvasObj.AddComponent<Canvas>();
                uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                Debug.Log("UI Canvas created automatically.");
            }
        }

        // Check if LoadingUI already exists
        if (FindObjectOfType<LoadingUI>() == null) {
            CreateLoadingUI();
        }
    }

    private void CreateLoadingUI() {
        // Create main loading panel
        GameObject loadingPanel = new GameObject("LoadingPanel");
        loadingPanel.transform.SetParent(uiCanvas.transform, false);

        // Setup panel
        Image panelImage = loadingPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 1f); // Semi-transparent black

        RectTransform panelRect = loadingPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Create loading text
        GameObject loadingTextObj = new GameObject("LoadingText");
        loadingTextObj.transform.SetParent(loadingPanel.transform, false);

        Text loadingText = loadingTextObj.AddComponent<Text>();
        loadingText.text = "Loading...";
        loadingText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        loadingText.fontSize = 24;
        loadingText.color = Color.white;
        loadingText.alignment = TextAnchor.MiddleCenter;

        RectTransform textRect = loadingTextObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.6f);
        textRect.anchorMax = new Vector2(0.5f, 0.6f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(300, 50);

        // Create progress bar background
        GameObject progressBG = new GameObject("ProgressBarBackground");
        progressBG.transform.SetParent(loadingPanel.transform, false);

        Image bgImage = progressBG.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        RectTransform bgRect = progressBG.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.5f, 0.4f);
        bgRect.anchorMax = new Vector2(0.5f, 0.4f);
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.sizeDelta = new Vector2(400, 20);

        // Create progress bar
        GameObject progressBarObj = new GameObject("ProgressBar");
        progressBarObj.transform.SetParent(loadingPanel.transform, false);

        Slider progressBar = progressBarObj.AddComponent<Slider>();
        progressBar.value = 0f;
        progressBar.minValue = 0f;
        progressBar.maxValue = 1f;

        RectTransform sliderRect = progressBarObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, 0.4f);
        sliderRect.anchorMax = new Vector2(0.5f, 0.4f);
        sliderRect.anchoredPosition = Vector2.zero;
        sliderRect.sizeDelta = new Vector2(400, 20);

        // Create fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(progressBarObj.transform, false);

        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        // Create fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);

        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green fill

        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        progressBar.fillRect = fillRect;

        // Create progress percentage text
        GameObject percentTextObj = new GameObject("PercentText");
        percentTextObj.transform.SetParent(loadingPanel.transform, false);

        Text percentText = percentTextObj.AddComponent<Text>();
        percentText.text = "0%";
        percentText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        percentText.fontSize = 18;
        percentText.color = Color.white;
        percentText.alignment = TextAnchor.MiddleCenter;

        RectTransform percentRect = percentTextObj.GetComponent<RectTransform>();
        percentRect.anchorMin = new Vector2(0.5f, 0.3f);
        percentRect.anchorMax = new Vector2(0.5f, 0.3f);
        percentRect.anchoredPosition = Vector2.zero;
        percentRect.sizeDelta = new Vector2(100, 30);

        // Add LoadingUI component
        loadingPanel.AddComponent<LoadingUI>();

        loadingPanel.SetActive(false);
    }
}