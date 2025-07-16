using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingUI : MonoBehaviour {
    [Header("UI References")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text loadingText;
    [SerializeField] private Text progressText;

    [Header("Loading Settings")]
    [SerializeField]
    private string[] loadingMessages = {
        "Loading...",
        "Entering Interior...",
        "Preparing Environment...",
        "Almost Ready..."
    };
    [SerializeField] private float messageChangeInterval = 1f;

    private Coroutine loadingMessageCoroutine;

    private void Awake() {
        bool includeInactive = true;
        if (loadingPanel == null) loadingPanel = FindObjectOfType<LoadingUI>(includeInactive).gameObject;
        if (loadingPanel != null) loadingPanel.SetActive(false);
        else Debug.LogError("LoadingUI not found");
    }

    private void OnEnable() {
        if (progressBar == null) progressBar = GetComponentInChildren<Slider>();
        Text[] texts = GetComponentsInChildren<Text>();
        foreach (Text text in texts) {
            if (text.name == "LoadingText") loadingText = text;
            if (text.name == "ProgressText") progressText = text;
        }
    }

    public void ShowLoadingScreen() {
        if (loadingPanel != null) {
            loadingPanel.SetActive(true);
        }

        // Reset progress
        UpdateProgress(0f);

        // Start cycling through loading messages
        if (loadingMessageCoroutine != null) {
            StopCoroutine(loadingMessageCoroutine);
        }

        loadingMessageCoroutine = StartCoroutine(CycleLoadingMessages());
    }

    public void HideLoadingScreen() {
        if (loadingPanel != null) {
            loadingPanel.SetActive(false);
        }

        // Stop loading message cycling
        if (loadingMessageCoroutine != null) {
            StopCoroutine(loadingMessageCoroutine);
            loadingMessageCoroutine = null;
        }
    }

    public void UpdateProgress(float progress) {
        // Clamp progress between 0 and 1
        progress = Mathf.Clamp01(progress);

        // Update progress bar
        if (progressBar != null) {
            progressBar.value = progress;
        }

        // Update progress text
        if (progressText != null) {
            progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }
    }

    private IEnumerator CycleLoadingMessages() {
        int messageIndex = 0;

        while (true) {
            if (loadingText != null && loadingMessages.Length > 0) {
                loadingText.text = loadingMessages[messageIndex];
                messageIndex = (messageIndex + 1) % loadingMessages.Length;
            }

            yield return new WaitForSeconds(messageChangeInterval);
        }
    }

    public void SetLoadingMessage(string message) {
        if (loadingText != null) {
            loadingText.text = message;
        }
    }
}