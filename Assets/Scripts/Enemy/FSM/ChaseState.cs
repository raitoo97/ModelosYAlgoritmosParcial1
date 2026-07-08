using UnityEngine;
using UnityEngine.AI;
public class ChaseState : IState
{
    private NavMeshAgent _agent;
    private Transform _transform;
    private FSM _fsm;
    private Enemy _enemy;
    private Transform _playerTransform;
    public ChaseState(Transform transform, NavMeshAgent agent, Enemy enemy, FSM fsm,Transform playerTransform)
    {
        _transform = transform;
        _agent = agent;
        _enemy = enemy;
        _fsm = fsm;
        _playerTransform = playerTransform;
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
        if (distance < FlyWeightPointer.Entity.maxDistance)
        {
            _fsm.ChangeState(FSM.StateID.Attack);
            return;
        }
        _agent.SetDestination(_playerTransform.position);
        _enemy.Rotate(dir);
    }
}