using UnityEngine;

[CreateAssetMenu(fileName = "PlayerJump_Data", menuName = "FSM/Player/Jump", order = 3)]
public class JumpSO : BaseStateDO<EPlayerStates> {
    [Header("Jump Force Settings")]
    [SerializeField] private float jumpForce = 400f;
    [SerializeField] private ForceMode jumpForceMode = ForceMode.Impulse;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float maxFallSpeed = 15f;

    [Header("Air Movement")]
    [SerializeField] private float airMoveForce = 100f;
    [SerializeField] private float maxAirSpeed = 5f;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayerMask = 1;

    // Properties to access the settings
    public float JumpForce => jumpForce;
    public ForceMode JumpForceMode => jumpForceMode;

    public float Gravity => gravity;
    public float MaxFallSpeed => maxFallSpeed;

    public float AirMoveForce => airMoveForce;
    public float MaxAirSpeed => maxAirSpeed;

    public float GroundCheckDistance => groundCheckDistance;
    public LayerMask GroundLayerMask => groundLayerMask;

    public override BaseState<EPlayerStates> GetState(BaseStateContext<EPlayerStates> context) {
        return new JumpState(context as PlayerStateContext, this);
    }
}