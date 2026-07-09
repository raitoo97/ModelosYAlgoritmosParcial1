using UnityEngine;
using UnityEngine.AI;
public class AttackState : BaseAttackState
{
    private float _timer;
    public AttackState(Transform transform, NavMeshAgent agent, Enemy enemy, FSM fsm,
                       Transform playerTransform, FlyWeight stats): base(transform, agent, enemy, fsm, playerTransform, stats) { }
    public override void OnEnter()
    {
        base.OnEnter();
        _timer = 0;
    }
    protected override void OnAttackUpdate(Vector3 dirToPlayer)
    {
        _timer += Time.deltaTime;
        if (_timer >= _stats.coolDown)
        {
            _timer = 0;
            _enemy.Shoot();
        }
    }
}