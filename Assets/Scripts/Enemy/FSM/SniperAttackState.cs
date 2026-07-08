using UnityEngine;
using UnityEngine.AI;
public class SniperAttackState : IState
{
    private Transform _transform;
    private FSM _fsm;
    private SniperEnemy _enemy;
    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private Transform _gunSight;
    private LayerMask _losMask;
    private FlyWeight _stats;
    private float _charge;
    public SniperAttackState(Transform transform, NavMeshAgent agent, SniperEnemy enemy, FSM fsm,
                             Transform playerTransform, FlyWeight stats, Transform gunSight, LayerMask losMask)
    {
        _transform = transform;
        _agent = agent;
        _enemy = enemy;
        _fsm = fsm;
        _playerTransform = playerTransform;
        _stats = stats;
        _gunSight = gunSight;
        _losMask = losMask;
    }
    public void OnEnter()
    {
        _agent.isStopped = true;
        _charge = 0;
        _enemy.PlayAimAnimation();
        _enemy.ShowLaser(true);
    }
    public void OnExit()
    {
        _enemy.ShowLaser(false);
        _agent.isStopped = false;
    }
    public void OnUpdate()
    {
        var dir = _playerTransform.position - _transform.position;
        if (dir.magnitude > _stats.maxDistance)
        {
            _fsm.ChangeState(FSM.StateID.Chase);
            return;
        }
        _enemy.Rotate(dir);
        Vector3 origin = _gunSight.position;
        Vector3 aimDir = (_enemy.GetAimPoint() - origin).normalized;
        // La carga solo avanza si el rayo llega al player sin obstaculos en el medio
        if (Physics.Raycast(origin, aimDir, out RaycastHit hit, _stats.maxDistance, _losMask, QueryTriggerInteraction.Ignore)&& hit.collider.TryGetComponent<Player>(out _))// uso el _ porque descarto el valor de salida, solo me interesa si es player o no
        {
            _charge += Time.deltaTime;
            _enemy.UpdateLaser(origin, hit.point, _charge / _stats.coolDown);
            if (_charge >= _stats.coolDown)
            {
                _charge = 0;
                _enemy.Shoot();
            }
        }
        else
        {
            _charge = 0;
            Vector3 end = hit.collider != null ? hit.point : origin + aimDir * _stats.maxDistance;
            _enemy.UpdateLaser(origin, end, 0f);
        }
    }
}
