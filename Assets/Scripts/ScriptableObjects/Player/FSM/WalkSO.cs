
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWalk_Data", menuName = "FSM/Player/Walk", order = 1)]
public class WalkSO : BaseStateDO<EPlayerStates> {
    public override BaseState<EPlayerStates> GetState() {
        return new WalkState(this);
    }
}