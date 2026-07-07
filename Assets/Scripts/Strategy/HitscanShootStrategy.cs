using UnityEngine;
public class HitscanShootStrategy : IShootStrategy
{
    private float _damage;
    private Factions _owner;
    private LayerMask _hitMask;
    private float _maxDistance;
    public HitscanShootStrategy(float damage, Factions owner, LayerMask hitMask, float maxDistance)
    {
        _damage = damage;
        _owner = owner;
        _hitMask = hitMask;
        _maxDistance = maxDistance;
    }
    //estrategia de disparo que utiliza raycast para determinar si un disparo impacta en un objetivo
    //devuelve un ShotResult que indica si el disparo impactó y la informacion del impacto
    public ShotResult Shoot(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, _maxDistance, _hitMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var target) && ShouldHit(hit.collider))
                target.TakeDamage(_damage);
            return new ShotResult { didHit = true, hitPoint = hit.point, hitNormal = hit.normal };
        }
        return new ShotResult { didHit = false, hitPoint = Vector3.zero, hitNormal = Vector3.zero };
    }
    private bool ShouldHit(Collider other)
    {
        if (other.TryGetComponent<IFactionMember>(out var member))
            return member.Faction != _owner;
        return true;
    }
}
