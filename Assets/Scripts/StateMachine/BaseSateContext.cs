using System;
using UnityEngine;

public abstract class BaseStateContext<EState> where EState : Enum {
    public Animator _animator;
}