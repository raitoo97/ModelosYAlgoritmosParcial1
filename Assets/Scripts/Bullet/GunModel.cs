using UnityEngine;
public class GunModel
{
    private float _maxDeviationAngle;
    public bool IsAiming { get; private set; }
    public Quaternion InitLocalRotation { get; }

    public GunModel(float maxDeviationAngle, Quaternion initLocalRotation)
    {
        _maxDeviationAngle = maxDeviationAngle;
        InitLocalRotation = initLocalRotation;
    }
    public void SetAiming(bool isAiming) => IsAiming = isAiming;
    public Quaternion ComputeTargetLocalRotation(Vector3 aimPoint, Vector3 gunPosition, Vector3 currentForward, Transform parent)
    {
        if (!IsAiming) return InitLocalRotation;
        Vector3 desiredDirection = (aimPoint - gunPosition).normalized;
        if (desiredDirection.sqrMagnitude < 0.0001f)
            return InitLocalRotation;
        Vector3 clampedDirection = Vector3.RotateTowards(currentForward, desiredDirection, _maxDeviationAngle * Mathf.Deg2Rad, 0f);
        Quaternion targetWorldRotation = Quaternion.LookRotation(clampedDirection, Vector3.up);
        if (parent != null)
            return Quaternion.Inverse(parent.rotation) * targetWorldRotation;
        return targetWorldRotation;
    }
}
