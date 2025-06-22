
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerIdle_Data", menuName = "FSM/Player/Idle", order = 1)]
public class IdleSO : BaseStateDO<EPlayerStates> {
    public override BaseState<EPlayerStates> GetState() {
        return new IdleSate(this);
    }
}