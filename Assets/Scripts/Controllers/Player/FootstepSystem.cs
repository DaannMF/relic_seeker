using UnityEngine;

public class FootstepSystem : MonoBehaviour {
    [Header("Footstep Settings")]
    [SerializeField] private float walkFootstepInterval = 0.5f;
    [SerializeField] private float runFootstepInterval = 0.3f;
    [SerializeField] private Transform audioSourcePosition;

    [Header("Audio Clips")]
    [SerializeField] private string grassFootstepAudio = "Footstep_Grass";
    [SerializeField] private string dirtFootstepAudio = "Footstep_Dirt";
    [SerializeField] private string normalFootstepAudio = "Footstep_Normal";

    private float footstepTimer;
    private Terrain currentTerrain;
    private PlayerController playerController;

    public enum SurfaceType {
        Grass,
        Dirt,
        Normal
    }

    private void Awake() {
        playerController = GetComponent<PlayerController>();
        currentTerrain = Terrain.activeTerrain;
    }

    public void UpdateFootsteps(float interval, bool isMoving) {
        if (!isMoving) {
            footstepTimer = 0f;
            return;
        }

        footstepTimer += Time.deltaTime;

        if (footstepTimer >= interval) {
            PlayFootstepSound();
            footstepTimer = 0f;
        }
    }

    private void PlayFootstepSound() {
        SurfaceType surfaceType = DetectSurfaceType();
        string audioClipName = GetAudioClipForSurface(surfaceType);

        if (!string.IsNullOrEmpty(audioClipName)) {
            AudioEvents.OnPlayAudio3D?.Invoke(audioClipName, AudioType.SFX, audioSourcePosition.position);
        }
    }

    private SurfaceType DetectSurfaceType() {
        if (currentTerrain == null || currentTerrain.terrainData == null || playerController.IsInInterior) {
            return SurfaceType.Normal;
        }

        Vector3 playerPosition = playerController.Rb.position;
        Vector3 terrainPosition = currentTerrain.transform.position;
        TerrainData terrainData = currentTerrain.terrainData;

        Vector3 relativePosition = playerPosition - terrainPosition;
        Vector3 normalizedPosition = new Vector3(
            relativePosition.x / terrainData.size.x,
            0,
            relativePosition.z / terrainData.size.z
        );

        normalizedPosition.x = Mathf.Clamp01(normalizedPosition.x);
        normalizedPosition.z = Mathf.Clamp01(normalizedPosition.z);

        int mapX = Mathf.RoundToInt(normalizedPosition.x * (terrainData.alphamapWidth - 1));
        int mapZ = Mathf.RoundToInt(normalizedPosition.z * (terrainData.alphamapHeight - 1));

        float[,,] alphaMap = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        if (alphaMap.GetLength(2) < 2) {
            return SurfaceType.Normal;
        }

        float grassStrength = alphaMap[0, 0, 0];
        float dirtStrength = alphaMap[0, 0, 1];

        const float threshold = 0.5f;

        if (grassStrength > threshold && grassStrength > dirtStrength)
            return SurfaceType.Grass;
        else if (dirtStrength > threshold && dirtStrength > grassStrength)
            return SurfaceType.Dirt;
        else
            return SurfaceType.Normal;
    }

    private string GetAudioClipForSurface(SurfaceType surfaceType) {
        return surfaceType switch {
            SurfaceType.Grass => grassFootstepAudio,
            SurfaceType.Dirt => dirtFootstepAudio,
            SurfaceType.Normal => normalFootstepAudio,
            _ => normalFootstepAudio
        };
    }

    public float GetWalkInterval() => walkFootstepInterval;
    public float GetRunInterval() => runFootstepInterval;
}