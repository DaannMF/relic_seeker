using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Relic : MonoBehaviour {
    [Header("Relic Settings")]
    [SerializeField] private GameObject visualRepresentation;
    [SerializeField] private bool autoCollect = false;

    [Header("Audio")]
    [SerializeField] private string collectAudioClip = "Relic_Collect";
    [SerializeField] private string ambientAudioClip = "Relic_Ambient";

    [Header("Outline Settings")]
    [SerializeField] private OutlineController outlineController;

    [Header("Effects")]
    [SerializeField] private ParticleSystem collectEffect;
    [SerializeField] private Light relicLight;
    [SerializeField] private float lightIntensityMin = 0.5f;
    [SerializeField] private float lightIntensityMax = 2f;
    [SerializeField] private float lightPulseSpeed = 2f;

    private bool isCollected = false;
    private bool playerInRange = false;
    private PlayerController currentPlayer;
    private float lightTimer = 0f;

    private void Start() {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        if (visualRepresentation == null) {
            Transform visual = transform.Find("Visual");
            if (visual != null)
                visualRepresentation = visual.gameObject;
            else if (transform.childCount > 0)
                visualRepresentation = transform.GetChild(0).gameObject;
        }

        if (outlineController == null)
            outlineController = GetComponentInChildren<OutlineController>();

        if (collectEffect == null)
            collectEffect = GetComponentInChildren<ParticleSystem>();

        if (relicLight == null)
            relicLight = GetComponentInChildren<Light>();

        if (!string.IsNullOrEmpty(ambientAudioClip))
            AudioEvents.OnPlayAudioLoop?.Invoke(ambientAudioClip, AudioType.SFX, true);
    }

    private void OnDestroy() {
        if (!string.IsNullOrEmpty(ambientAudioClip))
            AudioEvents.OnStopAudio?.Invoke(ambientAudioClip);
    }

    private void Update() {
        if (isCollected) return;

        HandleLightPulse();

        if (!autoCollect && playerInRange && currentPlayer != null) {
            PlayerInput input = currentPlayer.GetComponent<PlayerInput>();
            if (input != null && input.InteractPressed)
                CollectRelic();
        }
    }

    private void HandleLightPulse() {
        if (relicLight == null) return;

        lightTimer += Time.deltaTime * lightPulseSpeed;
        float intensity = Mathf.Lerp(lightIntensityMin, lightIntensityMax,
            (Mathf.Sin(lightTimer) + 1f) * 0.5f);
        relicLight.intensity = intensity;
    }

    private void OnTriggerEnter(Collider other) {
        if (isCollected) return;

        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            if (!IsOriginalPlayer(player))
                return;

            playerInRange = true;
            currentPlayer = player;

            if (autoCollect)
                CollectRelic();
            else
                UIEvents.OnPromptShow?.Invoke("Press E to collect the Sacred Relic");

            ShowOutline();
        }
    }

    private void OnTriggerExit(Collider other) {
        PlayerController player = other.transform.parent.GetComponentInChildren<PlayerController>();
        if (player != null) {
            playerInRange = false;
            currentPlayer = null;

            if (!autoCollect)
                UIEvents.OnPromptHide?.Invoke();

            HideOutline();
        }
    }

    private void CollectRelic() {
        if (isCollected) return;

        isCollected = true;

        if (!autoCollect)
            UIEvents.OnPromptHide?.Invoke();

        HideOutline();

        if (!string.IsNullOrEmpty(ambientAudioClip))
            AudioEvents.OnStopAudio?.Invoke(ambientAudioClip);

        if (!string.IsNullOrEmpty(collectAudioClip))
            AudioEvents.OnPlayAudio?.Invoke(collectAudioClip, AudioType.SFX);

        if (collectEffect != null) {
            collectEffect.Play();
            StartCoroutine(WaitForParticleSystem());
        }
    }

    private IEnumerator WaitForParticleSystem() {
        while (collectEffect.isPlaying) {
            yield return null;
        }

        if (visualRepresentation != null)
            visualRepresentation.SetActive(false);

        if (relicLight != null)
            relicLight.enabled = false;

        GameEvents.OnGameWonTriggered?.Invoke();

        Destroy(gameObject, 3f);
    }

    private bool IsOriginalPlayer(PlayerController player) {
        if (player == null) return false;

        Controllable controllable = player.transform.parent.GetComponent<Controllable>();
        return controllable != null && controllable.IsOriginalPlayer();
    }

    private void ShowOutline() {
        if (outlineController != null && !isCollected)
            outlineController.ShowOutline();
    }

    private void HideOutline() {
        if (outlineController != null)
            outlineController.HideOutline();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = isCollected ? Color.gray : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);

        Gizmos.color = Color.yellow;
        for (int i = 0; i < 8; i++) {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Gizmos.DrawRay(transform.position, direction * 2f);
        }

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 3f,
            $"SACRED RELIC\nAuto Collect: {autoCollect}\nStatus: {(isCollected ? "COLLECTED" : "AVAILABLE")}\n*** GAME WIN CONDITION ***");
#endif
    }
}