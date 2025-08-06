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

    [Header("Audio")]
    [SerializeField] private string creditsMusic = "Credits_Music";

    [Header("Scene Management")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isScrolling = false;

    private void Start() {
        Time.timeScale = 1f;

        if (creditsText != null)
            creditsText.text = CreditsText.CREDITS_TEXT;

        StartCoroutine(PlayCreditsSequence());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            StopAllCoroutines();
            ReturnToMainScene();
        }
    }

    private void OnDestroy() {
        if (!string.IsNullOrEmpty(creditsMusic))
            AudioEvents.OnStopAudio?.Invoke(creditsMusic);
    }

    private IEnumerator PlayCreditsSequence() {
        if (!string.IsNullOrEmpty(creditsMusic))
            AudioEvents.OnPlayAudioLoop?.Invoke(creditsMusic, AudioType.Music, true);

        isScrolling = true;

        yield return StartCoroutine(ScrollCredits());
        yield return new WaitForSeconds(endWaitTime);

        ReturnToMainScene();
    }

    private IEnumerator ScrollCredits() {
        if (scrollRect == null || contentParent == null) yield break;

        yield return null;

        scrollRect.verticalNormalizedPosition = 1f;

        yield return null;

        float currentPosition = 1f;
        float scrollIncrement = scrollSpeed / 1000f;

        while (currentPosition > 0f && isScrolling) {
            currentPosition -= scrollIncrement * Time.deltaTime;
            currentPosition = Mathf.Clamp01(currentPosition);

            scrollRect.verticalNormalizedPosition = currentPosition;

            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
        isScrolling = false;
    }

    private void ReturnToMainScene() {
        if (!string.IsNullOrEmpty(creditsMusic))
            AudioEvents.OnStopAudio?.Invoke(creditsMusic);

        GameEvents.OnSetGameStateRequested?.Invoke(GameState.MainMenu);

        if (GameSceneManager.Instance != null)
            GameSceneManager.Instance.ReturnToMainMenu(mainMenuSceneName);
        else
            SceneManager.LoadScene(mainMenuSceneName);
    }
}