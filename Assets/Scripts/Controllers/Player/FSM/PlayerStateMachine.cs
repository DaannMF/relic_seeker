using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class PlayerStateMachine : BaseStateMachine<EPlayerStates> {
    [SerializeField] private List<BaseStateDO<EPlayerStates>> _statesData;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Animator _animator;


    private PlayerStateContext _context;
    private PlayerController _playerController;

    void Awake() {
        if (_input == null) _input = GetComponent<PlayerInput>();
        if (_animator == null) _animator = transform.parent.GetComponentInChildren<Animator>();
        _playerController = GetComponent<PlayerController>();

        ValidateRequiredComponents();

        _context = new PlayerStateContext(_input, _animator, _playerController);
        Initialize();
    }

    private void ValidateRequiredComponents() {
        Assert.IsNotNull(_input, "PlayerInput is not assigned in PlayerStateMachine.");
        Assert.IsNotNull(_animator, "Animator is not assigned in PlayerStateMachine.");
        Assert.IsNotNull(_playerController, "PlayerController is not assigned in PlayerStateMachine.");
    }

    private void Initialize() {
        foreach (var stateData in _statesData)
            AddState(stateData.GetState(_context));

        _currentState = _states[EPlayerStates.Idle];
    }

    // Method to refresh Animator reference when controlling different entities
    public void RefreshAnimatorReference() {
        _animator = transform.parent.GetComponentInChildren<Animator>();
        if (_context != null) {
            _context._animator = _animator;
        }

        ValidateRequiredComponents();
    }
}