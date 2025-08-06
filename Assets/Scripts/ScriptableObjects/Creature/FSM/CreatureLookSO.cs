using UnityEngine;

[CreateAssetMenu(fileName = "CreatureLookState", menuName = "States/Creature/Look")]
public class CreatureLookSO : BaseStateDO<ECreatureStates> {
    [Header("Look Settings")]
    [SerializeField] private float lookTime = 1.5f;

    public float LookTime => lookTime;

    public override BaseState<ECreatureStates> GetState(BaseStateContext<ECreatureStates> context) {
        return new CreatureLookState(ECreatureStates.Look, this, context);
    }
}