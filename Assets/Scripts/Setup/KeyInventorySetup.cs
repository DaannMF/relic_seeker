using UnityEngine;

public class KeyInventorySetup : MonoBehaviour {
    [Header("Auto-Setup Settings")]
    [SerializeField] private bool createKeyInventoryManager = true;

    private void Awake() {
        if (createKeyInventoryManager)
            EnsureKeyInventoryManagerExists();
    }

    private void EnsureKeyInventoryManagerExists() {
        if (KeyInventoryManager.Instance == null) {
            GameObject keyManagerObj = new GameObject("KeyInventoryManager");
            keyManagerObj.AddComponent<KeyInventoryManager>();
        }
    }
}