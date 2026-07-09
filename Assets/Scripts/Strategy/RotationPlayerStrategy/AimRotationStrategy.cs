using UnityEngine;
public class AimRotationStrategy : IRotationStrategy
{
    private Rigidbody _rb;
    private Transform _cameraReference;
    public AimRotationStrategy(Rigidbody rb, Transform cameraReference)
    {
        _rb = rb;
        _cameraReference = cameraReference;
    }
    public void Rotate(Vector3 direction)
    {
        Quaternion finalRotation = Quaternion.Euler(0, _cameraReference.eulerAngles.y, 0);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, finalRotation, FlyWeightPointer.Player.rotateSpeed * Time.fixedDeltaTime));
    }
}
