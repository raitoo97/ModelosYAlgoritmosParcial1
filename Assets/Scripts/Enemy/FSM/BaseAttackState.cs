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
    // Umbral de punteria: cada estado puede ajustar que tan alineado
    // tiene que estar con el player antes de disparar.
    protected virtual float FacingAngleThreshold => 10f;
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
        if (dir.magnitude > _stats.maxDistance || !LineOfSight.IsOnSight(_transform.position, _playerTransform.position))
        {
            _fsm.ChangeState(FSM.StateID.Chase);
            return;
        }
        _enemy.Rotate(dir);
        OnAttackUpdate(dir);
    }
    // Chequeo comun de punteria. Aplano la direccion en Y para que el
    // desnivel con el player no infle el angulo y trabe el disparo.
    protected bool IsFacingPlayer(Vector3 dirToPlayer)
    {
        return Vector3.Angle(_transform.forward, new Vector3(dirToPlayer.x, 0f, dirToPlayer.z)) <= FacingAngleThreshold;
    }
    protected abstract void OnAttackUpdate(Vector3 dirToPlayer);
}
