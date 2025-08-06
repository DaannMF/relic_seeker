using UnityEngine;

public class Fence : MonoBehaviour {
    [Header("Fence Settings")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private GameObject fenceVisual;
    [SerializeField] private Collider fenceCollider;

    [Header("Animation Settings")]
    [SerializeField] private FenceAnimationType animationType = FenceAnimationType.Slide;
    [SerializeField] private Vector3 openPosition = Vector3.up * 3f;
    [SerializeField] private Vector3 openRotation = Vector3.zero;
    [SerializeField] private float animationSpeed = 2f;

    [Header("Audio")]
    [SerializeField] private string openAudioClip = "Fence_Open";
    [SerializeField] private string closeAudioClip = "Fence_Close";

    private Vector3 closedPosition;
    private Vector3 closedRotation;
    private bool isAnimating = false;

    public enum FenceAnimationType {
        Slide,
        Rotate,
        Scale
    }

    private void Start() {
        if (fenceVisual != null) {
            closedPosition = fenceVisual.transform.localPosition;
            closedRotation = fenceVisual.transform.localEulerAngles;
        }

        if (fenceVisual == null) {
            Transform visual = transform.Find("Visual");
            if (visual != null) {
                fenceVisual = visual.gameObject;
                closedPosition = fenceVisual.transform.localPosition;
                closedRotation = fenceVisual.transform.localEulerAngles;
            }
        }

        if (fenceCollider == null) {
            fenceCollider = GetComponent<Collider>();
            if (fenceCollider == null && fenceVisual != null)
                fenceCollider = fenceVisual.GetComponent<Collider>();
        }

        SetFenceState(isOpen, false);
    }

    public void OpenFence() {
        if (isOpen || isAnimating) return;

        isOpen = true;
        StartCoroutine(AnimateFence(true));

        if (!string.IsNullOrEmpty(openAudioClip))
            AudioEvents.OnPlayAudio?.Invoke(openAudioClip, AudioType.SFX);
    }

    public void CloseFence() {
        if (!isOpen || isAnimating) return;

        isOpen = false;
        StartCoroutine(AnimateFence(false));

        if (!string.IsNullOrEmpty(closeAudioClip))
            AudioEvents.OnPlayAudio?.Invoke(closeAudioClip, AudioType.SFX);
    }

    public void SetFenceState(bool open, bool animate = true) {
        isOpen = open;

        if (animate && Application.isPlaying)
            StartCoroutine(AnimateFence(open));
        else
            ApplyFenceState(open);
    }

    private void ApplyFenceState(bool open) {
        if (fenceVisual == null) return;

        if (open) {
            switch (animationType) {
                case FenceAnimationType.Slide:
                    fenceVisual.transform.localPosition = closedPosition + openPosition;
                    break;
                case FenceAnimationType.Rotate:
                    fenceVisual.transform.localEulerAngles = closedRotation + openRotation;
                    break;
                case FenceAnimationType.Scale:
                    fenceVisual.transform.localScale = Vector3.zero;
                    break;
            }
        }
        else {
            switch (animationType) {
                case FenceAnimationType.Slide:
                    fenceVisual.transform.localPosition = closedPosition;
                    break;
                case FenceAnimationType.Rotate:
                    fenceVisual.transform.localEulerAngles = closedRotation;
                    break;
                case FenceAnimationType.Scale:
                    fenceVisual.transform.localScale = Vector3.one;
                    break;
            }
        }

        if (fenceCollider != null)
            fenceCollider.enabled = !open;
    }

    private System.Collections.IEnumerator AnimateFence(bool opening) {
        if (fenceVisual == null) yield break;

        isAnimating = true;
        float duration = 1f / animationSpeed;
        float elapsed = 0f;

        Vector3 startPos = fenceVisual.transform.localPosition;
        Vector3 startRot = fenceVisual.transform.localEulerAngles;
        Vector3 startScale = fenceVisual.transform.localScale;

        Vector3 targetPos, targetRot, targetScale;

        if (opening) {
            switch (animationType) {
                case FenceAnimationType.Slide:
                    targetPos = closedPosition + openPosition;
                    targetRot = startRot;
                    targetScale = startScale;
                    break;
                case FenceAnimationType.Rotate:
                    targetPos = startPos;
                    targetRot = closedRotation + openRotation;
                    targetScale = startScale;
                    break;
                case FenceAnimationType.Scale:
                    targetPos = startPos;
                    targetRot = startRot;
                    targetScale = Vector3.zero;
                    break;
                default:
                    targetPos = startPos;
                    targetRot = startRot;
                    targetScale = startScale;
                    break;
            }
        }
        else {
            targetPos = closedPosition;
            targetRot = closedRotation;
            targetScale = Vector3.one;
        }

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            fenceVisual.transform.localPosition = Vector3.Lerp(startPos, targetPos, progress);
            fenceVisual.transform.localEulerAngles = Vector3.Lerp(startRot, targetRot, progress);
            fenceVisual.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            yield return null;
        }

        fenceVisual.transform.localPosition = targetPos;
        fenceVisual.transform.localEulerAngles = targetRot;
        fenceVisual.transform.localScale = targetScale;

        if (fenceCollider != null)
            fenceCollider.enabled = !opening;

        isAnimating = false;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = isOpen ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);

        if (fenceVisual != null) {
            Gizmos.color = Color.yellow;
            Vector3 currentPos = Application.isPlaying ? fenceVisual.transform.position : transform.position;

            switch (animationType) {
                case FenceAnimationType.Slide:
                    Vector3 targetPos = currentPos + openPosition;
                    Gizmos.DrawLine(currentPos, targetPos);
                    Gizmos.DrawWireCube(targetPos, Vector3.one * 0.5f);
                    break;
                case FenceAnimationType.Rotate:
                    Gizmos.DrawWireSphere(currentPos, 1f);
                    break;
                case FenceAnimationType.Scale:
                    Gizmos.DrawWireSphere(currentPos, 0.5f);
                    break;
            }
        }

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 3f,
            $"Fence\nType: {animationType}\nState: {(isOpen ? "Open" : "Closed")}\nAnimating: {isAnimating}");
#endif
    }
}