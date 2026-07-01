using UnityEngine;
public class MovementRotationStrategy : IRotationStrategy
{
    private Rigidbody _rb;
    private Transform _cameraReference;
    private float _rotateSpeed;
    public MovementRotationStrategy(Rigidbody rb, Transform cameraReference, float rotateSpeed)
    {
        _rb = rb;
        _cameraReference = cameraReference;
        _rotateSpeed = rotateSpeed;
    }
    public void Rotate(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.001f) return;
        Vector3 input = new Vector3(direction.x, 0, direction.z);
        float targetY = Quaternion.LookRotation(input).eulerAngles.y + _cameraReference.eulerAngles.y;
        Quaternion finalRotation = Quaternion.Euler(0, targetY, 0);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, finalRotation, _rotateSpeed * Time.fixedDeltaTime));
    }
}
