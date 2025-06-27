using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class PlayerStateMachine : BaseStateMachine<EPlayerStates> {
    [SerializeField] private List<BaseStateDO<EPlayerStates>> _statesData;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Animator _animator;

    [Header("References")]
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform cameraTransform;

    [Header("Rotation")]
    [SerializeField] private float maxAngle = 45f;

    [Header("Controllable")]
    [SerializeField] private float detectionRange = 6f;

    private PlayerStateContext _context;
    private PlayerController _playerController;

    void Awake() {
        // Initialize components if not assigned
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_input == null) _input = GetComponent<PlayerInput>();
        if (_animator == null) _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();

        ValidateRequiredComponents();

        _context = new PlayerStateContext(_rb, _input, _animator, pivot, cameraTransform,
                                         maxAngle, detectionRange, _playerController);
        Initialize();
    }

    private void ValidateRequiredComponents() {
        Assert.IsNotNull(_rb, "Rigidbody is not assigned in PlayerStateMachine.");
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