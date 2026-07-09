using System.Collections.Generic;
using UnityEngine;
public class GunModel
{
    private Dictionary<bool, IGunRotationStrategy> _strategies;
    private IGunRotationStrategy _currentStrategy;
    private IShootStrategy _shootStrategy;
    private LayerMask _hitMask;
    private float _maxDistance;
    public bool IsAiming { get; private set; }
    public GunModel(Quaternion initLocalRotation, IShootStrategy shootStrategy, LayerMask hitMask, float maxDistance)
    {
        _shootStrategy = shootStrategy;
        _hitMask = hitMask;
        _maxDistance = maxDistance;
        _strategies = new Dictionary<bool, IGunRotationStrategy>
        {
            { false, new GunIdleRotationStrategy(initLocalRotation) },
            { true,  new GunAimRotationStrategy(initLocalRotation) }
        };
        _currentStrategy = _strategies[false];
    }
    public void SetShootStrategy(IShootStrategy strategy)
    {
        _shootStrategy = strategy;
    }
    public void SetAiming(bool isAiming)
    {
        IsAiming = isAiming;
        _currentStrategy = _strategies[isAiming];
    }
    public Quaternion ComputeTargetLocalRotation(Vector3 aimPoint, Vector3 gunPosition, Transform parent)
    {
        return _currentStrategy.ComputeTargetLocalRotation(aimPoint, gunPosition, parent);
    }
    public ShotResult Shoot(Vector3 origin, Vector3 aimPoint, Vector3 barrelForward)
    {
        Vector3 direction = IsAiming ? (aimPoint - origin).normalized : barrelForward;
        return _shootStrategy.Shoot(origin, direction);
    }
    public LaserState ComputeLaser(Vector3 origin, Vector3 aimPoint)
    {
        if (!IsAiming)
            return new LaserState { isVisible = false };
        Vector3 direction = (aimPoint - origin).normalized;
        bool hasHit = Physics.Raycast(origin, direction, out RaycastHit hit, _maxDistance, _hitMask, QueryTriggerInteraction.Ignore);
        return new LaserState
        {
            isVisible = true,
            start = origin,
            end = hasHit ? hit.point : origin + direction * _maxDistance,
            hasHit = hasHit,
            hitNormal = hasHit ? hit.normal : Vector3.zero
        };
    }
}
public struct LaserState
{
    public bool isVisible;
    public Vector3 start;
    public Vector3 end;
    public bool hasHit;
    public Vector3 hitNormal;
}
