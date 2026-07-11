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
    //Este raycast ES la bala: reemplaza al proyectil fisico por un impacto instantaneo (hitscan).
    //Sale desde la boca del arma (origin) en la direccion del disparo, hasta _maxDistance,
    //ignorando triggers y las layers fuera de _hitMask (ej. el propio Player).
    //A diferencia del raycast de GetAimPoint (que solo usa el punto), este consume todo el RaycastHit:
    //  - hit.collider -> busca IDamageable y valida faccion ( FactionRules.ShouldHit) para aplicar el dmg.
    //  - hit.point y hit.normal -> viajan en el ShotResult para que la vista (GunView)
    //    posicione los efectos de impacto sobre la superficie golpeada.
    public ShotResult Shoot(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, _maxDistance, _hitMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var target) && FactionRules.ShouldHit(hit.collider, _owner))
                target.TakeDamage(_damage);
            return new ShotResult { didHit = true, hitPoint = hit.point, hitNormal = hit.normal };
        }
        return new ShotResult { didHit = false, hitPoint = Vector3.zero, hitNormal = Vector3.zero };
    }
}
