using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    private FSM _fsm;
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        _fsm = new FSM();
        _fsm.AddState(FSM.StateID.Chase, new ChaseState(transform,_agent,this,_fsm));
        _fsm.AddState(FSM.StateID.Attack, new AttackState(transform, _agent, this, _fsm));
        _fsm.ChangeState(FSM.StateID.Chase);
    }
    void Update()
    {
        _fsm.onUpdateState();
    }
    public void Rotate(Vector3 direction)
    {
        Vector3 _dirRot = new Vector3(direction.x, 0, direction.z).normalized;
        if (_dirRot.sqrMagnitude > 0.001f)
        {
            Quaternion _rotDir = Quaternion.LookRotation(_dirRot);
            _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, _rotDir, FlyWeightPointer.Entity.rotateSpeed * Time.deltaTime));
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, FlyWeightPointer.Entity.maxDistance);
    }
}