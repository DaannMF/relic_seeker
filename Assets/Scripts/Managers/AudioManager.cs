using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources Pool")]
    [SerializeField] private int poolSize = 20;

    [Header("Audio Clips")]
    [SerializeField] private AudioClipSO audioClipCollection;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float uiVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string UI_VOLUME_KEY = "UIVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private Queue<AudioSource> audioSourcePool;
    private List<AudioSource> activeAudioSources;
    private Dictionary<string, AudioSource> namedAudioSources;

    public float MasterVolume => masterVolume;
    public float MusicVolume => musicVolume;
    public float UIVolume => uiVolume;
    public float SFXVolume => sfxVolume;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioManager();
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        SubscribeToEvents();
    }

    private void OnDestroy() {
        UnsubscribeFromEvents();
    }

    private void InitializeAudioManager() {
        LoadVolumeSettings();

        audioSourcePool = new Queue<AudioSource>();
        activeAudioSources = new List<AudioSource>();
        namedAudioSources = new Dictionary<string, AudioSource>();

        for (int i = 0; i < poolSize; i++)
            CreateAudioSource();
    }

    private AudioSource CreateAudioSource() {
        GameObject audioGO = new GameObject($"AudioSource_{audioSourcePool.Count}");
        audioGO.transform.SetParent(transform);
        AudioSource audioSource = audioGO.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSourcePool.Enqueue(audioSource);
        return audioSource;
    }

    private void SubscribeToEvents() {
        AudioEvents.OnPlayAudio += PlayAudio;
        AudioEvents.OnPlayAudio3D += PlayAudio3D;
        AudioEvents.OnStopAudio += StopAudio;
        AudioEvents.OnStopAllAudio += StopAllAudio;
        AudioEvents.OnPlayAudioLoop += PlayAudioLoop;
        AudioEvents.OnPlayAudio3DLoop += PlayAudio3DLoop;

        AudioEvents.OnSetMasterVolume += SetMasterVolume;
        AudioEvents.OnSetMusicVolume += SetMusicVolume;
        AudioEvents.OnSetUIVolume += SetUIVolume;
        AudioEvents.OnSetSFXVolume += SetSFXVolume;

        AudioEvents.OnSetAudioVolume += SetAudioVolume;
        AudioEvents.OnFadeAudio += FadeAudio;
    }

    private void UnsubscribeFromEvents() {
        AudioEvents.OnPlayAudio -= PlayAudio;
        AudioEvents.OnPlayAudio3D -= PlayAudio3D;
        AudioEvents.OnStopAudio -= StopAudio;
        AudioEvents.OnStopAllAudio -= StopAllAudio;
        AudioEvents.OnPlayAudioLoop -= PlayAudioLoop;
        AudioEvents.OnPlayAudio3DLoop -= PlayAudio3DLoop;

        AudioEvents.OnSetMasterVolume -= SetMasterVolume;
        AudioEvents.OnSetMusicVolume -= SetMusicVolume;
        AudioEvents.OnSetUIVolume -= SetUIVolume;
        AudioEvents.OnSetSFXVolume -= SetSFXVolume;

        AudioEvents.OnSetAudioVolume -= SetAudioVolume;
        AudioEvents.OnFadeAudio -= FadeAudio;
    }

    private AudioSource GetAudioSource() {
        if (audioSourcePool.Count > 0)
            return audioSourcePool.Dequeue();
        else
            return CreateAudioSource();
    }

    private void ReturnAudioSource(AudioSource audioSource) {
        if (audioSource != null) {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
            audioSource.transform.position = transform.position;

            if (activeAudioSources.Contains(audioSource))
                activeAudioSources.Remove(audioSource);

            audioSourcePool.Enqueue(audioSource);
        }
    }

    private void PlayAudio(string clipName, AudioType audioType) {
        AudioClipData clipData = audioClipCollection.GetAudioClip(clipName);
        if (clipData?.clip != null)
            PlayAudioInternal(clipData, audioType, Vector3.zero, false, false);
    }

    private void PlayAudio3D(string clipName, AudioType audioType, Vector3 position) {
        AudioClipData clipData = audioClipCollection.GetAudioClip(clipName);
        if (clipData?.clip != null)
            PlayAudioInternal(clipData, audioType, position, true, false);
    }

    private void PlayAudioLoop(string clipName, AudioType audioType, bool loop) {
        AudioClipData clipData = audioClipCollection.GetAudioClip(clipName);
        if (clipData?.clip != null)
            PlayAudioInternal(clipData, audioType, Vector3.zero, false, loop);
    }

    private void PlayAudio3DLoop(string clipName, AudioType audioType, Vector3 position, bool loop) {
        AudioClipData clipData = audioClipCollection.GetAudioClip(clipName);
        if (clipData?.clip != null)
            PlayAudioInternal(clipData, audioType, position, true, loop);
    }

    private void PlayAudioInternal(AudioClipData clipData, AudioType audioType, Vector3 position, bool is3D, bool forceLoop) {
        AudioSource audioSource = GetAudioSource();

        audioSource.clip = clipData.clip;
        audioSource.volume = GetVolumeMultiplier(audioType) * masterVolume;
        audioSource.loop = forceLoop || clipData.loop;

        if (is3D || clipData.is3D) {
            audioSource.spatialBlend = 1f;
            audioSource.transform.position = position;
        }
        else
            audioSource.spatialBlend = 0f;

        activeAudioSources.Add(audioSource);
        namedAudioSources[clipData.clipName] = audioSource;

        audioSource.Play();

        if (!audioSource.loop)
            StartCoroutine(ReturnAudioSourceAfterClip(audioSource, clipData.clip.length));
    }

    private IEnumerator ReturnAudioSourceAfterClip(AudioSource audioSource, float clipLength) {
        yield return new WaitForSeconds(clipLength);

        if (namedAudioSources.ContainsValue(audioSource)) {
            string keyToRemove = null;
            foreach (var kvp in namedAudioSources) {
                if (kvp.Value == audioSource) {
                    keyToRemove = kvp.Key;
                    break;
                }
            }
            if (keyToRemove != null)
                namedAudioSources.Remove(keyToRemove);
        }

        ReturnAudioSource(audioSource);
    }

    private float GetVolumeMultiplier(AudioType audioType) {
        switch (audioType) {
            case AudioType.Music:
                return musicVolume;
            case AudioType.UI:
                return uiVolume;
            case AudioType.SFX:
                return sfxVolume;
            default:
                return 1f;
        }
    }

    private void StopAudio(string clipName) {
        if (namedAudioSources.ContainsKey(clipName)) {
            AudioSource audioSource = namedAudioSources[clipName];
            namedAudioSources.Remove(clipName);
            ReturnAudioSource(audioSource);
        }
    }

    private void StopAllAudio() {
        foreach (var audioSource in activeAudioSources.ToArray())
            ReturnAudioSource(audioSource);

        namedAudioSources.Clear();
    }

    private void SetMasterVolume(float volume) {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllAudioVolumes();
        SaveVolumeSettings();
    }

    private void SetMusicVolume(float volume) {
        musicVolume = Mathf.Clamp01(volume);
        UpdateAudioVolumesByType(AudioType.Music);
        SaveVolumeSettings();
    }

    private void SetUIVolume(float volume) {
        uiVolume = Mathf.Clamp01(volume);
        UpdateAudioVolumesByType(AudioType.UI);
        SaveVolumeSettings();
    }

    private void SetSFXVolume(float volume) {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateAudioVolumesByType(AudioType.SFX);
        SaveVolumeSettings();
    }

    private void UpdateAllAudioVolumes() {
        foreach (var audioSource in activeAudioSources) {
            if (audioSource.clip != null) {
                AudioClipData clipData = audioClipCollection.GetAudioClip(audioSource.clip.name);
                if (clipData != null)
                    audioSource.volume = GetVolumeMultiplier(clipData.audioType) * masterVolume;
            }
        }
    }

    private void UpdateAudioVolumesByType(AudioType audioType) {
        foreach (var audioSource in activeAudioSources) {
            if (audioSource.clip != null) {
                AudioClipData clipData = audioClipCollection.GetAudioClip(audioSource.clip.name);
                if (clipData != null && clipData.audioType == audioType)
                    audioSource.volume = GetVolumeMultiplier(audioType) * masterVolume;
            }
        }
    }

    private void SetAudioVolume(string clipName, float volume) {
        if (namedAudioSources.ContainsKey(clipName))
            namedAudioSources[clipName].volume = volume * masterVolume;
    }

    private void FadeAudio(string clipName, float targetVolume) {
        if (namedAudioSources.ContainsKey(clipName))
            StartCoroutine(FadeAudioCoroutine(namedAudioSources[clipName], targetVolume, 1f));
    }

    private IEnumerator FadeAudioCoroutine(AudioSource audioSource, float targetVolume, float fadeTime) {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime) {
            elapsedTime += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume * masterVolume, elapsedTime / fadeTime);
            yield return null;
        }

        audioSource.volume = targetVolume * masterVolume;
    }

    private void LoadVolumeSettings() {
        masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        uiVolume = PlayerPrefs.GetFloat(UI_VOLUME_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }

    private void SaveVolumeSettings() {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(UI_VOLUME_KEY, uiVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }
}