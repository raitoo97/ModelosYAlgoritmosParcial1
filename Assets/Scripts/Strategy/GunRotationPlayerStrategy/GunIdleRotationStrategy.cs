using UnityEngine;
public class GunIdleRotationStrategy : IGunRotationStrategy
{
    private Quaternion _initLocalRotation;
    public GunIdleRotationStrategy(Quaternion initLocalRotation) 
    {
        _initLocalRotation = initLocalRotation;
    }
    public Quaternion ComputeTargetLocalRotation(Vector3 aimPoint, Vector3 gunPosition, Transform parent) 
    {
        return _initLocalRotation;
    }
}
