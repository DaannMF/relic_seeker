using UnityEngine;

[CreateAssetMenu(fileName = "CreatureIdleState", menuName = "States/Creature/Idle")]
public class CreatureIdleSO : BaseStateDO<ECreatureStates> {
    [Header("Idle Settings")]
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private float lookChance = 0.3f;

    public float IdleTime => idleTime;
    public float LookChance => lookChance;

    public override BaseState<ECreatureStates> GetState(BaseStateContext<ECreatureStates> context) {
        return new CreatureIdleState(ECreatureStates.Idle, this, context);
    }
}