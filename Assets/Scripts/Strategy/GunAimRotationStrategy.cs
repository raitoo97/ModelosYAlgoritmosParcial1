using UnityEngine;
public class GunAimRotationStrategy : IGunRotationStrategy
{
    private float _maxDeviationAngle;
    private Quaternion _initLocalRotation;
    public GunAimRotationStrategy(float maxDeviationAngle, Quaternion initLocalRotation)
    {
        _maxDeviationAngle = maxDeviationAngle;
        _initLocalRotation = initLocalRotation;
    }
    public Quaternion ComputeTargetLocalRotation(Vector3 aimPoint, Vector3 gunPosition, Vector3 currentForward, Transform parent)
    {
        Vector3 desiredDirection = (aimPoint - gunPosition).normalized;
        if (desiredDirection.sqrMagnitude < 0.0001f)
            return _initLocalRotation;
        Vector3 clampedDirection = Vector3.RotateTowards(currentForward, desiredDirection, _maxDeviationAngle * Mathf.Deg2Rad, 0f);
        Quaternion targetWorldRotation = Quaternion.LookRotation(clampedDirection, Vector3.up);
        if (parent != null)
            return Quaternion.Inverse(parent.rotation) * targetWorldRotation;
        return targetWorldRotation;
    }
}
