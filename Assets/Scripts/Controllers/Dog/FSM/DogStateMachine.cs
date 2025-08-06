using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class DogStateMachine : BaseStateMachine<EDogStates> {
    [Header("State Data")]
    [SerializeField] private List<BaseStateDO<EDogStates>> _statesData;

    private DogStateContext _context;
    private Animator _animator;
    private DogController _dogController;

    void Awake() {
        _animator = transform.parent.GetComponentInChildren<Animator>();
        _dogController = GetComponentInChildren<DogController>();

        ValidateRequiredComponents();

        _context = new DogStateContext(_animator, _dogController);
        Initialize();
    }

    private void ValidateRequiredComponents() {
        Assert.IsNotNull(_animator, "Animator is not assigned in DogStateMachine.");
        Assert.IsNotNull(_dogController, "DogController is not assigned in DogStateMachine.");
    }

    private void Initialize() {
        foreach (var stateData in _statesData)
            AddState(stateData.GetState(_context));

        _currentState = _states[EDogStates.Idle];
    }
}