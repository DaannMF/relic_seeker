using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWalk_Data", menuName = "FSM/Player/Walk", order = 1)]
public class WalkSO : BaseStateDO<EPlayerStates> {
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxAngleMovement = 30f;

    public float MoveSpeed => moveSpeed;
    public float MaxAngleMovement => maxAngleMovement;

    public override BaseState<EPlayerStates> GetState(BaseStateContext<EPlayerStates> context) {
        return new WalkState(context as PlayerStateContext, this);
    }
}