using UnityEngine;

[RequireComponent(typeof(CreatureController), typeof(CreatureStateMachine))]
public class SlimeAI : MonoBehaviour {
    [Header("Animation Settings")]
    [SerializeField] private bool useRandomAnimationSpeed = true;
    [SerializeField] private float minAnimationSpeed = 0.8f;
    [SerializeField] private float maxAnimationSpeed = 1.2f;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip[] slimeSounds;
    [SerializeField] private float soundChance = 0.1f;
    [SerializeField] private float minSoundInterval = 5f;
    [SerializeField] private float maxSoundInterval = 15f;



    private CreatureController creatureController;
    private CreatureStateMachine stateMachine;
    private Animator animator;
    private AudioSource audioSource;
    private float nextSoundTime;

    private void Awake() {
        creatureController = GetComponent<CreatureController>();
        stateMachine = GetComponent<CreatureStateMachine>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null && slimeSounds != null && slimeSounds.Length > 0) {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.volume = 0.5f;
        }

        InitializeSlime();
    }

    private void Start() {
        if (useRandomAnimationSpeed && animator != null) {
            float randomSpeed = Random.Range(minAnimationSpeed, maxAnimationSpeed);
            animator.speed = randomSpeed;
        }

        ScheduleNextSound();
    }

    private void Update() {
        HandleSounds();
    }

    private void InitializeSlime() {
        // Slime-specific initialization can go here
        // For now, we rely on the StateMachine initialization
    }

    private void HandleSounds() {
        if (slimeSounds == null || slimeSounds.Length == 0 || audioSource == null) return;

        if (Time.time >= nextSoundTime) {
            if (Random.Range(0f, 1f) <= soundChance) {
                PlayRandomSound();
            }
            ScheduleNextSound();
        }
    }

    private void PlayRandomSound() {
        if (slimeSounds.Length > 0) {
            AudioClip randomSound = slimeSounds[Random.Range(0, slimeSounds.Length)];
            audioSource.PlayOneShot(randomSound);
        }
    }

    private void ScheduleNextSound() {
        nextSoundTime = Time.time + Random.Range(minSoundInterval, maxSoundInterval);
    }

    public void OnSlimeStateChanged(ECreatureStates newState) {
        // State change notifications can be handled here if needed
    }
}