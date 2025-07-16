using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingUI : MonoBehaviour {
    [Header("UI References")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text loadingText;
    [SerializeField] private Text progressText;
    [SerializeField] private Text titleText;

    [Header("Loading Settings")]
    [SerializeField]
    private string[] defaultLoadingMessages = {
        "Loading...",
        "Preparing Environment...",
        "Almost Ready..."
    };

    [SerializeField]
    private string[] interiorEntryMessages = {
        "Entering Interior...",
        "Preparing Interior Space...",
        "Loading Interior Assets...",
        "Almost Ready..."
    };

    [SerializeField]
    private string[] interiorExitMessages = {
        "Exiting Interior...",
        "Returning to Main Area...",
        "Restoring Environment...",
        "Almost Ready..."
    };

    [SerializeField] private float messageChangeInterval = 1f;

    private string[] currentMessages;

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
            if (text.name == "TitleText") titleText = text;
        }
    }

    public void ShowLoadingScreen() {
        if (loadingPanel != null) {
            loadingPanel.SetActive(true);
        }

        // Set default messages if none are set
        if (currentMessages == null) {
            currentMessages = defaultLoadingMessages;
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
            if (loadingText != null && currentMessages != null && currentMessages.Length > 0) {
                loadingText.text = currentMessages[messageIndex];
                messageIndex = (messageIndex + 1) % currentMessages.Length;
            }

            yield return new WaitForSeconds(messageChangeInterval);
        }
    }

    public void SetLoadingMessage(string message) {
        if (loadingText != null) {
            loadingText.text = message;
        }
    }

    public void SetLoadingType(string loadingType) {
        switch (loadingType.ToLower()) {
            case "interior_entry":
                currentMessages = interiorEntryMessages;
                break;
            case "interior_exit":
                currentMessages = interiorExitMessages;
                break;
            case "single":
            case "default":
            default:
                currentMessages = defaultLoadingMessages;
                break;
        }
    }
}