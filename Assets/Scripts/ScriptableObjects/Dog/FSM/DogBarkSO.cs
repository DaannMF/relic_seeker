using UnityEngine;

[CreateAssetMenu(fileName = "DogBarkState", menuName = "States/Dog/Bark")]
public class DogBarkSO : BaseStateDO<EDogStates> {
    [Header("Bark Settings")]
    [SerializeField] private float barkDuration = 3f;
    [SerializeField] private float barkInterval = 0.8f;

    public float BarkDuration => barkDuration;
    public float BarkInterval => barkInterval;

    public override BaseState<EDogStates> GetState(BaseStateContext<EDogStates> context) {
        return new DogBarkState(EDogStates.Bark, this, context);
    }
}