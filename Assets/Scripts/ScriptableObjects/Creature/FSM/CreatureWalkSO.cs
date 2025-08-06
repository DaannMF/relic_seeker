using UnityEngine;

[CreateAssetMenu(fileName = "CreatureWalkState", menuName = "States/Creature/Walk")]
public class CreatureWalkSO : BaseStateDO<ECreatureStates> {
    [Header("Walk Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float walkTime = 5f;

    public float WalkSpeed => walkSpeed;
    public float WalkTime => walkTime;

    public override BaseState<ECreatureStates> GetState(BaseStateContext<ECreatureStates> context) {
        return new CreatureWalkState(ECreatureStates.Walk, this, context);
    }
}