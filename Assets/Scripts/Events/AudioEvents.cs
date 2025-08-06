using UnityEngine;
using UnityEngine.Events;

public enum AudioType {
    Music,
    UI,
    SFX
}

public static class AudioEvents {
    public static UnityAction<string, AudioType> OnPlayAudio;
    public static UnityAction<string, AudioType, Vector3> OnPlayAudio3D;
    public static UnityAction<string> OnStopAudio;
    public static UnityAction OnStopAllAudio;
    public static UnityAction<string, AudioType, bool> OnPlayAudioLoop;
    public static UnityAction<string, AudioType, Vector3, bool> OnPlayAudio3DLoop;

    public static UnityAction<float> OnSetMasterVolume;
    public static UnityAction<float> OnSetMusicVolume;
    public static UnityAction<float> OnSetUIVolume;
    public static UnityAction<float> OnSetSFXVolume;

    public static UnityAction<string, float> OnSetAudioVolume;
    public static UnityAction<string, float> OnFadeAudio;
}