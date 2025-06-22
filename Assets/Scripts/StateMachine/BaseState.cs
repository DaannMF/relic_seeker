using System;
using UnityEngine;

public abstract class BaseState<EState> where EState : Enum {
    public EState StateKey { get; private set; }
    public BaseStateDO<EState> StateData { get; private set; }

    protected BaseState(EState stateKey, BaseStateDO<EState> stateData) {
        StateKey = stateKey;
        StateData = stateData;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
    public abstract EState GetNextState();
    public virtual void OnTriggerEnter(Collider other) { }
    public virtual void OnTriggerStay(Collider other) { }
    public virtual void OnTriggerExit(Collider other) { }
}