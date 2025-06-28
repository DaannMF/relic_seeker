using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class EnemyStateMachine : BaseStateMachine<EEnemyStates> {
    [SerializeField] private List<BaseStateDO<EEnemyStates>> _statesData;
    private Animator _animator;
    private EnemyController _enemyController;

    private EnemyStateContext _context;

    void Awake() {
        if (_animator == null) _animator = GetComponent<Animator>();
        _enemyController = GetComponent<EnemyController>();

        _context = new EnemyStateContext(_animator, _enemyController);

        ValidateRequiredComponents();

        Initialize();
    }

    private void ValidateRequiredComponents() {
        Assert.IsNotNull(_animator, "Animator is not assigned in EnemyStateMachine.");
        Assert.IsNotNull(_enemyController, "EnemyController is not assigned in EnemyStateMachine.");
    }

    private void Initialize() {
        foreach (var stateData in _statesData)
            AddState(stateData.GetState(_context));

        _currentState = _states[EEnemyStates.Idle];
    }
}