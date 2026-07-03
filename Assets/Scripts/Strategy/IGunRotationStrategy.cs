using UnityEngine;
public interface IGunRotationStrategy
{
    Quaternion ComputeTargetLocalRotation(Vector3 aimPoint, Vector3 gunPosition, Vector3 currentForward, Transform parent);
}
