using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRun_Data", menuName = "FSM/Player/Run", order = 1)]
public class RunSO : BaseStateDO<EPlayerStates>
{
    [Header("Run Settings")]
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float maxAngleMovement = 25f;
    [SerializeField] private float staminaDrain = 10f;

    // Properties to access the settings
    public float RunSpeed => runSpeed;
    public float MaxAngleMovement => maxAngleMovement;
    public float StaminaDrain => staminaDrain;

    public override BaseState<EPlayerStates> GetState(BaseStateContext<EPlayerStates> context)
    {
        return new RunState(context as PlayerStateContext, this);
    }
}