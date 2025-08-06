using UnityEngine;

[CreateAssetMenu(fileName = "DogIdleState", menuName = "States/Dog/Idle")]
public class DogIdleSO : BaseStateDO<EDogStates> {
    [Header("Idle Settings")]
    [SerializeField] private float idleTime = 2f;

    public float IdleTime => idleTime;

    public override BaseState<EDogStates> GetState(BaseStateContext<EDogStates> context) {
        return new DogIdleState(EDogStates.Idle, this, context);
    }
}