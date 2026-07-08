using UnityEngine;
using UnityEngine.AI;
public class SniperChaseState : IState
{
    private Transform _transform;
    private FSM _fsm;
    private SniperEnemy _enemy;
    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private FlyWeight _stats;
    public SniperChaseState(Transform transform, NavMeshAgent agent, SniperEnemy enemy, FSM fsm,
                           Transform playerTransform, FlyWeight stats)
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
        if (_agent.enabled && _agent.isOnNavMesh)
            _agent.isStopped = true;
        _enemy.PlayAimAnimation();
    }
    public void OnExit() { }
    public void OnUpdate()
    {
        var dir = _playerTransform.position - _transform.position;
        if (dir.magnitude < _stats.maxDistance)
        {
            _fsm.ChangeState(FSM.StateID.Attack);
            return;
        }
        _enemy.Rotate(dir);
    }
}
