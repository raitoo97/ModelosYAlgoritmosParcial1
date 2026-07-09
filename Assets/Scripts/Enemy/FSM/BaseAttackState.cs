using UnityEngine;
using UnityEngine.AI;
public abstract class BaseAttackState : IState
{
    protected Transform _transform;
    protected NavMeshAgent _agent;
    protected Enemy _enemy;
    protected FSM _fsm;
    protected Transform _playerTransform;
    protected FlyWeight _stats;
    protected BaseAttackState(Transform transform, NavMeshAgent agent, Enemy enemy, FSM fsm,Transform playerTransform, FlyWeight stats)
    {
        _transform = transform;
        _agent = agent;
        _enemy = enemy;
        _fsm = fsm;
        _playerTransform = playerTransform;
        _stats = stats;
    }
    public virtual void OnEnter()
    {
        _agent.isStopped = true;
        _enemy.PlayAimAnimation();
    }
    public virtual void OnExit()
    {
        _agent.isStopped = false;
    }
    public void OnUpdate()
    {
        Vector3 dir = _playerTransform.position - _transform.position;
        if (dir.magnitude > _stats.maxDistance)
        {
            _fsm.ChangeState(FSM.StateID.Chase);
            return;
        }
        _enemy.Rotate(dir);
        OnAttackUpdate(dir);
    }
    protected abstract void OnAttackUpdate(Vector3 dirToPlayer);
}
