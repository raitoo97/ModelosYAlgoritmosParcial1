using UnityEngine;
public class AimRotationStrategy : IRotationStrategy
{
    private Rigidbody _rb;
    private Transform _cameraReference;
    private float _rotateSpeed;
    public AimRotationStrategy(Rigidbody rb, Transform cameraReference, float rotateSpeed)
    {
        _rb = rb;
        _cameraReference = cameraReference;
        _rotateSpeed = rotateSpeed;
    }
    public void Rotate(Vector3 direction)
    {
        Quaternion finalRotation = Quaternion.Euler(0, _cameraReference.eulerAngles.y, 0);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, finalRotation, _rotateSpeed * Time.fixedDeltaTime));
    }
}
