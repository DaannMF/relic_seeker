using System;
using UnityEngine;
using System.Collections.Generic;

public abstract class BaseStateMachine<EState> : MonoBehaviour where EState : Enum {
    protected Dictionary<EState, BaseState<EState>> _states = new();
    protected BaseState<EState> _currentState;
    protected bool _isTransitioning = false;

    void Start() {
        _currentState.Enter();
    }

    public void AddState(BaseState<EState> state) {
        if (!_states.ContainsKey(state.StateKey))
            _states.Add(state.StateKey, state);
    }

    void Update() {
        EState nextState = _currentState.GetNextState();

        if (_isTransitioning) return;

        if (nextState.Equals(_currentState.StateKey))
            _currentState.Update();
        else
            TransitionToState(nextState);

    }
    void FixedUpdate() {
        _currentState.FixedUpdate();
    }

    void TransitionToState(EState nextState) {
        _isTransitioning = true;
        _currentState.Exit();
        _currentState = _states[nextState];
        _currentState.Enter();
        _isTransitioning = false;
    }
}