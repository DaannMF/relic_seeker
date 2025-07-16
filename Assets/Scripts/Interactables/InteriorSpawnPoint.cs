using UnityEngine;

[System.Serializable]
public enum SpawnPointType
{
    Entrance,  // Where player spawns when entering from exterior
    Exit       // Where player spawns when returning from another interior
}

public class InteriorSpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    [SerializeField] private SpawnPointType pointType = SpawnPointType.Entrance;
    [SerializeField] private string spawnPointID = ""; // Optional ID for specific spawn points

    public SpawnPointType PointType => pointType;
    public string SpawnPointID => spawnPointID;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    private void OnDrawGizmos()
    {
        // Draw different colors based on spawn point type
        Gizmos.color = pointType == SpawnPointType.Entrance ? Color.green : Color.blue;

        // Draw arrow pointing forward
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;

        // Draw base circle
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        // Draw directional arrow
        Vector3 arrowHead = transform.position + forward * 1f;
        Gizmos.DrawLine(transform.position, arrowHead);
        Gizmos.DrawLine(arrowHead, arrowHead - forward * 0.3f + right * 0.2f);
        Gizmos.DrawLine(arrowHead, arrowHead - forward * 0.3f - right * 0.2f);

        // Draw label
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + up * 0.8f, 
            $"{pointType} Spawn\n{(string.IsNullOrEmpty(spawnPointID) ? "Default" : spawnPointID)}");
#endif
    }

    private void OnDrawGizmosSelected()
    {
        // Highlight when selected
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.7f);
    }
}