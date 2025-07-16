using UnityEngine;
using UnityEngine.UI;

public class LoadingUISetup : MonoBehaviour {
    [Header("LoadingUI Setup")]
    [SerializeField] private bool createLoadingUI = true;
    [SerializeField] private Canvas targetCanvas;

    private void Start() {
        if (createLoadingUI) CreateLoadingUI();
    }

    private void CreateLoadingUI() {
        if (targetCanvas == null) {
            targetCanvas = FindObjectOfType<Canvas>();
        }

        if (targetCanvas == null) {
            Debug.LogError("[LoadingUISetup] No Canvas found! Please assign a Canvas or create one first.");
            return;
        }

        if (FindObjectOfType<LoadingUI>() != null) {
            Debug.Log("[LoadingUISetup] LoadingUI already exists, skipping creation");
            return;
        }

        GameObject loadingPanel = new GameObject("LoadingPanel");
        loadingPanel.transform.SetParent(targetCanvas.transform, false);

        RectTransform panelRect = loadingPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        Image backgroundImage = loadingPanel.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 1f);

        // Create Game Title
        GameObject titleText = new GameObject("TitleText");
        titleText.transform.SetParent(loadingPanel.transform, false);

        RectTransform titleRect = titleText.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.7f);
        titleRect.anchorMax = new Vector2(1f, 0.9f);
        titleRect.sizeDelta = Vector2.zero;
        titleRect.anchoredPosition = Vector2.zero;

        Text titleTextComponent = titleText.AddComponent<Text>();
        titleTextComponent.text = "RELIC SEEKER";
        titleTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleTextComponent.fontSize = 48;
        titleTextComponent.color = Color.white;
        titleTextComponent.alignment = TextAnchor.MiddleCenter;
        titleTextComponent.fontStyle = FontStyle.Bold;

        GameObject progressContainer = new GameObject("ProgressContainer");
        progressContainer.transform.SetParent(loadingPanel.transform, false);

        RectTransform containerRect = progressContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.2f, 0.3f);  // Moved up from 0.1f to 0.3f
        containerRect.anchorMax = new Vector2(0.8f, 0.5f);  // Moved up from 0.3f to 0.5f
        containerRect.sizeDelta = Vector2.zero;
        containerRect.anchoredPosition = Vector2.zero;

        GameObject progressBg = new GameObject("ProgressBackground");
        progressBg.transform.SetParent(progressContainer.transform, false);

        RectTransform bgRect = progressBg.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0.4f);
        bgRect.anchorMax = new Vector2(1f, 0.6f);
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;

        Image bgImage = progressBg.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        GameObject progressBar = new GameObject("ProgressBar");
        progressBar.transform.SetParent(progressBg.transform, false);

        RectTransform barRect = progressBar.AddComponent<RectTransform>();
        barRect.anchorMin = Vector2.zero;
        barRect.anchorMax = Vector2.one;
        barRect.sizeDelta = Vector2.zero;
        barRect.anchoredPosition = Vector2.zero;

        Slider slider = progressBg.AddComponent<Slider>();
        slider.fillRect = barRect;
        slider.value = 0f;

        Image fillImage = progressBar.AddComponent<Image>();
        fillImage.color = new Color(0f, 0.8f, 0.2f, 1f);

        GameObject loadingText = new GameObject("LoadingText");
        loadingText.transform.SetParent(progressContainer.transform, false);

        RectTransform textRect = loadingText.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0.7f);
        textRect.anchorMax = new Vector2(1f, 0.9f);
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        Text text = loadingText.AddComponent<Text>();
        text.text = "Loading...";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        GameObject percentText = new GameObject("PercentText");
        percentText.transform.SetParent(progressContainer.transform, false);

        RectTransform percentRect = percentText.AddComponent<RectTransform>();
        percentRect.anchorMin = new Vector2(0f, 0.2f);
        percentRect.anchorMax = new Vector2(1f, 0.4f);
        percentRect.sizeDelta = Vector2.zero;
        percentRect.anchoredPosition = Vector2.zero;

        Text percentTextComponent = percentText.AddComponent<Text>();
        percentTextComponent.text = "0%";
        percentTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        percentTextComponent.fontSize = 18;
        percentTextComponent.color = Color.white;
        percentTextComponent.alignment = TextAnchor.MiddleCenter;

        LoadingUI loadingUI = loadingPanel.AddComponent<LoadingUI>();

        var loadingUIType = typeof(LoadingUI);
        var loadingPanelField = loadingUIType.GetField("loadingPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var progressBarField = loadingUIType.GetField("progressBar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var loadingTextField = loadingUIType.GetField("loadingText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var progressTextField = loadingUIType.GetField("progressText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var titleTextField = loadingUIType.GetField("titleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        loadingPanelField?.SetValue(loadingUI, loadingPanel);
        progressBarField?.SetValue(loadingUI, slider);
        loadingTextField?.SetValue(loadingUI, text);
        progressTextField?.SetValue(loadingUI, percentTextComponent);
        titleTextField?.SetValue(loadingUI, titleTextComponent);

        loadingPanel.SetActive(false);

        Debug.Log("[LoadingUISetup] LoadingUI created and configured automatically");

        if (CanvasManager.Instance != null) {
            CanvasManager.Instance.SetLoadingUI(loadingUI);
            Debug.Log("[LoadingUISetup] LoadingUI assigned to CanvasManager");
        }

        if (GameSceneManager.Instance != null) {
            GameSceneManager.Instance.SetLoadingUI(loadingUI);
            Debug.Log("[LoadingUISetup] LoadingUI assigned to GameSceneManager");
        }
    }
}