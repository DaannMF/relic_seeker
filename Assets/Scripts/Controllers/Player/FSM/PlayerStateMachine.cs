using UnityEngine;
using System.Collections.Generic;

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

        _context = new PlayerStateContext(_rb, _input, _animator, pivot, cameraTransform,
                                         maxAngle, detectionRange, _playerController);
        Initialize();
    }

    private void Initialize() {
        foreach (var stateData in _statesData)
            AddState(stateData.GetState(_context));

        _currentState = _states[EPlayerStates.Idle];
    }

    // Method to update player controller reference if needed
    public void SetPlayerController(PlayerController controller) {
        _playerController = controller;
        _context.SetPlayerController(controller);
    }
}