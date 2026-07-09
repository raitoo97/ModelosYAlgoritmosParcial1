using UnityEngine;
using UnityEngine.AI;
public class SniperAttackState : BaseAttackState
{
    private SniperEnemy _sniper;
    private Transform _gunSight;
    private LayerMask _losMask;
    private float _charge;
    private const float FacingAngleThreshold = 1f;
    public SniperAttackState(Transform transform, NavMeshAgent agent, SniperEnemy enemy, FSM fsm,
                             Transform playerTransform, FlyWeight stats, Transform gunSight, LayerMask losMask)
                             : base(transform, agent, enemy, fsm, playerTransform, stats)
    {
        _sniper = enemy;
        _gunSight = gunSight;
        _losMask = losMask;
    }
    public override void OnEnter()
    {
        base.OnEnter();
        _charge = 0;
        _sniper.ShowLaser(true);
    }
    public override void OnExit()
    {
        base.OnExit();
        _sniper.ShowLaser(false);
    }
    protected override void OnAttackUpdate(Vector3 dirToPlayer)
    {
        //Chequeo que el sniper este mirando al player en base al angulo entre su forward y la direccion al player
        bool isFacing = Vector3.Angle(_transform.forward, new Vector3(dirToPlayer.x, 0f, dirToPlayer.z)) <= FacingAngleThreshold;
        _sniper.ShowLaser(isFacing);
        if (!isFacing)
        {
            _charge = 0;
            return;
        }
        Vector3 origin = _gunSight.position;
        Vector3 aimDir = (_sniper.GetAimPoint() - origin).normalized;
        // La carga solo avanza si el rayo llega al player sin obstaculos en el medio
        if (Physics.Raycast(origin, aimDir, out RaycastHit hit, _stats.maxDistance, _losMask, QueryTriggerInteraction.Ignore) && hit.collider.TryGetComponent<Player>(out _))
        {
            _charge += Time.deltaTime;
            _sniper.UpdateLaser(origin, hit.point, _charge / _stats.coolDown);
            if (_charge >= _stats.coolDown)
            {
                _charge = 0;
                _sniper.Shoot();
            }
        }
        else
        {
            _charge = 0;
            Vector3 end = hit.collider != null ? hit.point : origin + aimDir * _stats.maxDistance;
            _sniper.UpdateLaser(origin, end, 0f);
        }
    }
}
