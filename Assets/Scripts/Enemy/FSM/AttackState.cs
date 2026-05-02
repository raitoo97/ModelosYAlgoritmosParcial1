using UnityEngine;
using UnityEngine.AI;
public class AttackState : Istate
{
    private Transform _transform;
    private FSM _fsm;
    private Enemy _enemy;
    private NavMeshAgent _agent;
    private float _timer;
    public AttackState(Transform transform, NavMeshAgent agent, Enemy enemy, FSM fsm)
    {
        _transform = transform;
        _agent = agent;
        _enemy = enemy;
        _fsm = fsm;
    }
    public void OnEnter()
    {
        _agent.isStopped = true;
        _timer = 0;
    }
    public void OnExit()
    {
        _agent.isStopped = false;
    }
    public void OnUpdate()
    {
        _timer += Time.deltaTime;
        var dir = GameManager.instance.player.transform.position - _transform.position;
        var distance = dir.magnitude;
        if (distance > FlyWeightPointer.Entity.maxDistance)
        {
            _fsm.ChangeState(FSM.StateID.Chase);
            return;
        }
        _enemy.Rotate(dir);
        if (_timer >= FlyWeightPointer.Entity.coolDown)
        {
            _timer = 0;
            Debug.Log("Shoot real");
        }
    }
}