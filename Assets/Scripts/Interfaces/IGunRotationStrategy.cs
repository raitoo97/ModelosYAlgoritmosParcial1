using UnityEngine;
public interface IGunRotationStrategy
{
    Quaternion ComputeTargetLocalRotation(Vector3 aimPoint, Vector3 gunPosition, Transform parent);
}
