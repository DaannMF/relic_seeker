using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class PlayerStateMachine : BaseStateMachine<EPlayerStates> {
    [SerializeField] private List<BaseStateDO<EPlayerStates>> _statesData;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Animator _animator;

    [Header("References")]
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform cameraTransform;

    private PlayerStateContext _context;
    private PlayerController _playerController;

    void Awake() {
        if (_input == null) _input = GetComponent<PlayerInput>();
        if (_animator == null) _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();

        ValidateRequiredComponents();

        _context = new PlayerStateContext(_input, _animator, _playerController);
        Initialize();
    }

    private void ValidateRequiredComponents() {
        Assert.IsNotNull(_input, "PlayerInput is not assigned in PlayerStateMachine.");
        Assert.IsNotNull(_animator, "Animator is not assigned in PlayerStateMachine.");
        Assert.IsNotNull(_playerController, "PlayerController is not assigned in PlayerStateMachine.");
        Assert.IsNotNull(pivot, "Pivot Transform is not assigned in PlayerStateMachine.");
        Assert.IsNotNull(cameraTransform, "Camera Transform is not assigned in PlayerStateMachine.");
    }

    private void Initialize() {
        foreach (var stateData in _statesData)
            AddState(stateData.GetState(_context));

        _currentState = _states[EPlayerStates.Idle];
    }
}