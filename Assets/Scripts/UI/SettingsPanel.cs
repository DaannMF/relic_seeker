using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour {
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string UI_VOLUME_KEY = "UIVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Start() {
        InitializeSliders();
        SetupSliderEvents();
    }

    private void OnDestroy() {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);

        if (uiVolumeSlider != null)
            uiVolumeSlider.onValueChanged.RemoveListener(OnUIVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }

    private void InitializeSliders() {
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);

        if (uiVolumeSlider != null)
            uiVolumeSlider.value = PlayerPrefs.GetFloat(UI_VOLUME_KEY, 1f);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }

    private void SetupSliderEvents() {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (uiVolumeSlider != null)
            uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnMasterVolumeChanged(float value) {
        AudioEvents.OnSetMasterVolume?.Invoke(value);
    }

    private void OnMusicVolumeChanged(float value) {
        AudioEvents.OnSetMusicVolume?.Invoke(value);
    }

    private void OnUIVolumeChanged(float value) {
        AudioEvents.OnSetUIVolume?.Invoke(value);
    }

    private void OnSFXVolumeChanged(float value) {
        AudioEvents.OnSetSFXVolume?.Invoke(value);
    }
}