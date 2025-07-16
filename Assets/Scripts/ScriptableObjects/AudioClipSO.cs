using UnityEngine;

[System.Serializable]
public class AudioClipData {
    public string clipName;
    public AudioClip clip;
    public AudioType audioType;
    public bool loop = false;
    public bool is3D = false;
}

[CreateAssetMenu(fileName = "AudioClipSO", menuName = "Audio/Audio Clip Collection")]
public class AudioClipSO : ScriptableObject {
    [Header("Audio Clips Collection")]
    public AudioClipData[] audioClips;

    public AudioClipData GetAudioClip(string clipName) {
        foreach (var clipData in audioClips)
            if (clipData.clipName == clipName)
                return clipData;

        return null;
    }
}