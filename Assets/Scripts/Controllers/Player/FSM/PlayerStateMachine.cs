using UnityEngine;
using System.Collections.Generic;

public class PlayerStateMachine : BaseStateMachine<EPlayerStates> {
    [SerializeField] private List<BaseStateDO<EPlayerStates>> _stateData;

    void Awake() {
        foreach (var stateData in _stateData) {
            var state = stateData.GetState();
            AddState(state);
        }
    }
}