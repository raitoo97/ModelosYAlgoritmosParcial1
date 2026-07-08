using UnityEngine;
using UnityEngine.AI;
public class AttackState : IState
{
    private Transform _transform;
    private FSM _fsm;
    private Enemy _enemy;
    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private float _timer;
    private FlyWeight _stats;
    public AttackState(Transform transform, NavMeshAgent agent, Enemy enemy, FSM fsm, Transform playerTransform,FlyWeight stats)
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
        _agent.isStopped = true;
        _timer = 0;
        _enemy.PlayAimAnimation();
    }
    public void OnExit()
    {
        _agent.isStopped = false;
    }
    public void OnUpdate()
    {
        _timer += Time.deltaTime;
        var dir = _playerTransform.position - _transform.position;
        var distance = dir.magnitude;
        if (distance > _stats.maxDistance)
        {
            _fsm.ChangeState(FSM.StateID.Chase);
            return;
        }
        _enemy.Rotate(dir);
        if (_timer >= _stats.coolDown)
        {
            _timer = 0;
            _enemy.Shoot();
        }
    }
}