using UnityEngine;

public class KeyInventoryManager : MonoBehaviour {
    public static KeyInventoryManager Instance { get; private set; }

    [Header("Key Inventory")]
    [SerializeField] private int currentKeys = 0;

    private const string KEYS_COUNT_KEY = "PlayerKeysCount";

    public int CurrentKeys => currentKeys;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadKeysFromPrefs();
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

    private void SubscribeToEvents() {
        InventoryEvents.OnAddKeys += AddKeys;
        InventoryEvents.OnRemoveKeys += RemoveKeys;
        InventoryEvents.OnSetKeys += SetKeys;
        InventoryEvents.OnGetKeyCount += GetKeyCount;
        InventoryEvents.OnCheckHasKeys += CheckHasKeys;
    }

    private void UnsubscribeFromEvents() {
        InventoryEvents.OnAddKeys -= AddKeys;
        InventoryEvents.OnRemoveKeys -= RemoveKeys;
        InventoryEvents.OnSetKeys -= SetKeys;
        InventoryEvents.OnGetKeyCount -= GetKeyCount;
        InventoryEvents.OnCheckHasKeys -= CheckHasKeys;
    }

    private void LoadKeysFromPrefs() {
        currentKeys = PlayerPrefs.GetInt(KEYS_COUNT_KEY, 0);
    }

    private void SaveKeysToPrefs() {
        PlayerPrefs.SetInt(KEYS_COUNT_KEY, currentKeys);
        PlayerPrefs.Save();
    }

    private void AddKeys(int amount) {
        if (amount > 0) {
            int oldKeys = currentKeys;
            currentKeys += amount;
            SaveKeysToPrefs();
            InventoryEvents.OnKeyCountChanged?.Invoke(currentKeys);
        }
    }

    private void RemoveKeys(int amount) {
        if (amount > 0) {
            int oldKeys = currentKeys;
            currentKeys = Mathf.Max(0, currentKeys - amount);
            SaveKeysToPrefs();
            InventoryEvents.OnKeyCountChanged?.Invoke(currentKeys);
        }
    }

    private void SetKeys(int amount) {
        int oldKeys = currentKeys;
        currentKeys = Mathf.Max(0, amount);
        SaveKeysToPrefs();
        InventoryEvents.OnKeyCountChanged?.Invoke(currentKeys);
    }

    private void GetKeyCount(System.Action<int> callback) {
        callback?.Invoke(currentKeys);
    }

    private void CheckHasKeys(int requiredKeys, System.Action<bool> callback) {
        bool hasEnough = currentKeys >= requiredKeys;
        Debug.Log($"[KeyInventoryManager] CheckHasKeys: need {requiredKeys}, have {currentKeys}, result: {hasEnough}");
        callback?.Invoke(hasEnough);
    }
}