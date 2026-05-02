using UnityEngine;
using UnityEngine.AI;
public class ChaseState : Istate
{
    private NavMeshAgent _agent;
    private Transform _transform;
    private FSM _fsm;
    private Enemy _enemy;
    public ChaseState(Transform transform, NavMeshAgent agent, Enemy enemy, FSM fsm)
    {
        _transform = transform;
        _agent = agent;
        _enemy = enemy;
        _fsm = fsm;
    }
    public void OnEnter()
    {
        Debug.Log("Enter Chase state");
    }
    public void OnExit()
    {
        Debug.Log("Exit Chase state");
    }
    public void OnUpdate()
    {
        var dir = GameManager.instance.player.transform.position - _transform.position;
        var distance = dir.magnitude;
        if (distance < FlyWeightPointer.Entity.maxDistance)
        {
            _fsm.ChangeState(FSM.StateID.Attack);
            return;
        }
        _agent.SetDestination(GameManager.instance.player.transform.position);
        _enemy.Rotate(dir);
    }
}