using UnityEngine;

[CreateAssetMenu(fileName = "DogWalkState", menuName = "States/Dog/Walk")]
public class DogWalkSO : BaseStateDO<EDogStates> {
    [Header("Walk Settings")]
    [SerializeField] private float walkSpeed = 300f;
    [SerializeField] private float walkTime = 4f;

    public float WalkSpeed => walkSpeed;
    public float WalkTime => walkTime;

    public override BaseState<EDogStates> GetState(BaseStateContext<EDogStates> context) {
        return new DogWalkState(EDogStates.Walk, this, context);
    }
}