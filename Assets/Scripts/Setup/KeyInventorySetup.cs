using UnityEngine;

public class KeyInventorySetup : MonoBehaviour
{
    [Header("Auto-Setup Settings")]
    [SerializeField] private bool createKeyInventoryManager = true;

    private void Awake()
    {
        if (createKeyInventoryManager)
        {
            EnsureKeyInventoryManagerExists();
        }
    }

    private void EnsureKeyInventoryManagerExists()
    {
        // Check if KeyInventoryManager already exists
        if (KeyInventoryManager.Instance == null)
        {
            // Create KeyInventoryManager GameObject
            GameObject keyManagerObj = new GameObject("KeyInventoryManager");
            keyManagerObj.AddComponent<KeyInventoryManager>();

            Debug.Log("[KeyInventorySetup] Created KeyInventoryManager automatically");
        }
    }

    [ContextMenu("Create Key Inventory Manager")]
    public void CreateKeyInventoryManager()
    {
        EnsureKeyInventoryManagerExists();
    }
}