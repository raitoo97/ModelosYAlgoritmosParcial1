using UnityEngine;
using UnityEngine.AI;
public class ChaseState : IState
{
    private NavMeshAgent _agent;
    private Transform _transform;
    private FSM _fsm;
    private Enemy _enemy;
    private Transform _playerTransform;
    private FlyWeight _stats;
    public ChaseState(Transform transform, NavMeshAgent agent, Enemy enemy, FSM fsm,Transform playerTransform, FlyWeight stats)
    {
        _transform = transform;
        _agent = agent;
        _enemy = enemy;
        _fsm = fsm;
        _playerTransform = playerTransform;
        _stats = stats;
    }
    public void OnEnter()
    {
        _agent.isStopped = false;
        _enemy.PlayRunAnimation();
    }
    public void OnExit()
    {
    }
    public void OnUpdate()
    {
        var dir = _playerTransform.position - _transform.position;
        var distance = dir.magnitude;
        if (distance < _stats.maxDistance && LineOfSight.IsOnSight(_transform.position, _playerTransform.position))
        {
            _fsm.ChangeState(FSM.StateID.Attack);
            return;
        }
        _agent.SetDestination(_playerTransform.position);
        _enemy.Rotate(dir);
    }
}