using System.Collections.Generic;
using UnityEngine;
public class GunModel
{
    private Dictionary<bool, IGunRotationStrategy> _strategies;
    private IGunRotationStrategy _currentStrategy;
    public bool IsAiming { get; private set; }
    public GunModel(float maxDeviationAngle, Quaternion initLocalRotation)
    {
        _strategies = new Dictionary<bool, IGunRotationStrategy>
        {
            { false, new GunIdleRotationStrategy(initLocalRotation) },
            { true,  new GunAimRotationStrategy(maxDeviationAngle, initLocalRotation) }
        };
        _currentStrategy = _strategies[false];
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
}
