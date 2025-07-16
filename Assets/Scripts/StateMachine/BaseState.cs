using System;

public abstract class BaseState<EState> where EState : Enum {
    public EState StateKey { get; private set; }
    public virtual BaseStateDO<EState> StateData { get; protected set; }
    public virtual BaseStateContext<EState> Context { get; protected set; }

    protected BaseState(EState stateKey, BaseStateDO<EState> stateData, BaseStateContext<EState> context) {
        StateKey = stateKey;
        StateData = stateData;
        Context = context;
    }

    public void Enter() {
        if (Context._animator != null) Context._animator.SetInteger("State", Convert.ToInt32(StateKey));
        OnEnter();
    }

    public virtual void OnEnter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
    public abstract EState GetNextState();
}