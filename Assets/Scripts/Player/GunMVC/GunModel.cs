using System.Collections.Generic;
using UnityEngine;
public enum AimIndicatorType
{
    None,
    Laser,
    Cone
}
public class GunModel
{
    private Dictionary<bool, IGunRotationStrategy> _strategies;
    private IGunRotationStrategy _currentStrategy;
    private IShootStrategy _shootStrategy;
    private LayerMask _hitMask;
    private float _maxDistance;
    private bool _showLaser = true;
    // Datos del cono de la escopeta: el arco real de dispersion y el alcance
    // real del perdigon, para que la V telegrafie exactamente el disparo.
    private float _coneArcDegrees;
    private float _coneRange;
    private AimIndicatorType _indicator;
    public bool IsAiming { get; private set; }
    public GunModel(Quaternion initLocalRotation, IShootStrategy shootStrategy, AimIndicatorType indicator, LayerMask hitMask, float maxDistance, float coneArcDegrees, float coneRange)
    {
        _shootStrategy = shootStrategy;
        _indicator = indicator;
        _hitMask = hitMask;
        _maxDistance = maxDistance;
        _coneArcDegrees = coneArcDegrees;
        _coneRange = coneRange;
        _strategies = new Dictionary<bool, IGunRotationStrategy>
        {
            { false, new GunIdleRotationStrategy(initLocalRotation) },
            { true,  new GunAimRotationStrategy(initLocalRotation) }
        };
        _currentStrategy = _strategies[false];
    }
    public void SetShootStrategy(IShootStrategy shootStrategy, AimIndicatorType indicator)
    {
        _shootStrategy = shootStrategy;
        _indicator = indicator;
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
    public AimIndicatorState ComputeAimIndicator(Vector3 origin, Vector3 aimPoint)
    {
        AimIndicatorState state = new AimIndicatorState();
        if (!IsAiming || _indicator == AimIndicatorType.None)
            return state;
        Vector3 direction = (aimPoint - origin).normalized;
        if (_indicator == AimIndicatorType.Laser)
        {
            state.laser = ComputeLine(origin, direction, _maxDistance);
        }
        else
        {
            float halfArc = _coneArcDegrees * 0.5f;
            state.coneLeft = ComputeLine(origin, Quaternion.AngleAxis(-halfArc, Vector3.up) * direction, _coneRange);
            state.coneRight = ComputeLine(origin, Quaternion.AngleAxis(halfArc, Vector3.up) * direction, _coneRange);
        }
        return state;
    }
    private LaserState ComputeLine(Vector3 origin, Vector3 direction, float maxDistance)
    {
        bool hasHit = Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, _hitMask, QueryTriggerInteraction.Ignore);
        return new LaserState
        {
            isVisible = true,
            start = origin,
            end = hasHit ? hit.point : origin + direction * maxDistance,
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
public struct AimIndicatorState
{
    public LaserState laser;
    public LaserState coneLeft;
    public LaserState coneRight;
}
