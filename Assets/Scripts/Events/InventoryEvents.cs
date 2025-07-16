using UnityEngine;
using UnityEngine.Events;

public static class InventoryEvents
{
    [Header("Key Management")]
    public static UnityAction<int> OnAddKeys;
    public static UnityAction<int> OnRemoveKeys;
    public static UnityAction<int> OnSetKeys;

    [Header("Key Queries")]
    public static UnityAction<System.Action<int>> OnGetKeyCount;
    public static UnityAction<int, System.Action<bool>> OnCheckHasKeys;

    [Header("Key Events")]
    public static UnityAction<int> OnKeyCountChanged;
}