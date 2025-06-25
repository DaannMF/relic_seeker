using System;
using UnityEngine;

public abstract class BaseStateDO<EState> : ScriptableObject where EState : Enum {
    public abstract BaseState<EState> GetState(BaseStateContext<EState> context);
}