using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour {
    [Header("UI References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentParent;
    [SerializeField] private TextMeshProUGUI creditsText;

    [Header("Credits Settings")]
    [SerializeField] private float scrollSpeed = 50f;
    [SerializeField] private float endWaitTime = 3f;
    [SerializeField] private bool debugScrolling = false;

    [Header("Audio")]
    [SerializeField] private string creditsMusic = "Credits_Music";

    [Header("Scene Management")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isScrolling = false;

    private void Start() {
        Time.timeScale = 1f;

        if (creditsText != null) {
            creditsText.text = CreditsText.CREDITS_TEXT;
        }

        StartCoroutine(PlayCreditsSequence());
    }

    private void Update() {
        // Allow player to skip credits with Escape at any time
        if (Input.GetKeyDown(KeyCode.Escape)) {
            StopAllCoroutines();
            ReturnToMainScene();
        }
    }

    private IEnumerator PlayCreditsSequence() {
        // Start credits music
        if (!string.IsNullOrEmpty(creditsMusic)) {
            AudioEvents.OnPlayAudioLoop?.Invoke(creditsMusic, AudioType.Music, true);
        }

        // Start scrolling immediately
        isScrolling = true;
        yield return StartCoroutine(ScrollCredits());

        // Wait at the end
        yield return new WaitForSeconds(endWaitTime);

        // Mark as completed and return to main scene
        ReturnToMainScene();
    }

    private IEnumerator ScrollCredits() {
        if (scrollRect == null || contentParent == null) yield break;

        // Wait a frame to ensure content is properly sized
        yield return null;

        if (debugScrolling) {
            Debug.Log($"[Credits] Content Height: {contentParent.rect.height}");
            Debug.Log($"[Credits] Viewport Height: {scrollRect.viewport.rect.height}");
        }

        // Start at the top to show the bottom of the content (verticalNormalizedPosition = 1)
        scrollRect.verticalNormalizedPosition = 1f;

        if (debugScrolling) {
            Debug.Log("[Credits] Starting scroll from top (position = 1) to show bottom content");
        }

        // Wait another frame to ensure the position is set
        yield return null;

        float currentPosition = 1f;
        float scrollIncrement = scrollSpeed / 1000f;

        if (debugScrolling) {
            Debug.Log($"[Credits] Scroll increment per second: {scrollIncrement}");
        }

        while (currentPosition > 0f && isScrolling) {
            currentPosition -= scrollIncrement * Time.deltaTime;
            currentPosition = Mathf.Clamp01(currentPosition);

            scrollRect.verticalNormalizedPosition = currentPosition;

            if (debugScrolling && Time.frameCount % 60 == 0) { // Log every 60 frames
                Debug.Log($"[Credits] Current scroll position: {currentPosition:F3}");
            }

            yield return null;
        }

        // Ensure we've scrolled all the way to show the top content
        scrollRect.verticalNormalizedPosition = 0f;
        isScrolling = false;

        if (debugScrolling) {
            Debug.Log("[Credits] Scrolling completed (position = 0)");
        }
    }

    private void ReturnToMainScene() {
        Debug.Log("[CreditsManager] Returning to main menu...");

        // Stop credits music
        if (!string.IsNullOrEmpty(creditsMusic)) {
            AudioEvents.OnStopAudio?.Invoke(creditsMusic);
        }

        // Reset game state to main menu
        GameEvents.OnSetGameStateRequested?.Invoke(GameState.MainMenu);

        // Use GameSceneManager to return to main menu properly
        if (GameSceneManager.Instance != null) {
            GameSceneManager.Instance.ReturnToMainMenu(mainMenuSceneName);
        }
        else {
            // Fallback to direct scene loading
            Debug.LogWarning("[CreditsManager] GameSceneManager not found, using direct scene loading");
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private void OnDestroy() {
        // Ensure music stops if object is destroyed
        if (!string.IsNullOrEmpty(creditsMusic)) {
            AudioEvents.OnStopAudio?.Invoke(creditsMusic);
        }
    }
}