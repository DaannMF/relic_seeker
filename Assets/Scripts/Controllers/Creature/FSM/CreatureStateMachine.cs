using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class CreatureStateMachine : BaseStateMachine<ECreatureStates> {
    [Header("State Data")]
    [SerializeField] private List<BaseStateDO<ECreatureStates>> _statesData;

    private CreatureStateContext _context;
    private Animator _animator;
    private CreatureController _creatureController;

    void Awake() {
        _animator = transform.parent.GetComponentInChildren<Animator>();
        _creatureController = GetComponentInChildren<CreatureController>();

        ValidateRequiredComponents();

        _context = new CreatureStateContext(_animator, _creatureController);
        Initialize();
    }

    private void ValidateRequiredComponents() {
        Assert.IsNotNull(_animator, "Animator is not assigned in CreatureStateMachine.");
        Assert.IsNotNull(_creatureController, "CreatureController is not assigned in CreatureStateMachine.");
    }

    private void Initialize() {
        foreach (var stateData in _statesData)
            AddState(stateData.GetState(_context));

        _currentState = _states[ECreatureStates.Idle];
    }
}