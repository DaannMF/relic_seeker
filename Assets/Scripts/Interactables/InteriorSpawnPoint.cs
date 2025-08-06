using UnityEngine;

[System.Serializable]
public enum SpawnPointType {
    Entrance,
    Exit
}

public class InteriorSpawnPoint : MonoBehaviour {
    [Header("Spawn Point Settings")]
    [SerializeField] private SpawnPointType pointType = SpawnPointType.Entrance;
    [SerializeField] private string spawnPointID = "";

    public SpawnPointType PointType => pointType;
    public string SpawnPointID => spawnPointID;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    private void OnDrawGizmos() {
        Gizmos.color = pointType == SpawnPointType.Entrance ? Color.green : Color.blue;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;

        Gizmos.DrawWireSphere(transform.position, 0.5f);

        Vector3 arrowHead = transform.position + forward * 1f;
        Gizmos.DrawLine(transform.position, arrowHead);
        Gizmos.DrawLine(arrowHead, arrowHead - forward * 0.3f + right * 0.2f);
        Gizmos.DrawLine(arrowHead, arrowHead - forward * 0.3f - right * 0.2f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + up * 0.8f,
            $"{pointType} Spawn\n{(string.IsNullOrEmpty(spawnPointID) ? "Default" : spawnPointID)}");
#endif
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.7f);
    }
}