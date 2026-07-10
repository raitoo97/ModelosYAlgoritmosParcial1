using UnityEngine;
public class MovementRotationStrategy : IRotationStrategy
{
    private Rigidbody _rb;
    private Transform _cameraReference;
    public MovementRotationStrategy(Rigidbody rb, Transform cameraReference)
    {
        _rb = rb;
        _cameraReference = cameraReference;
    }
    public void Rotate(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.001f) return;
        Vector3 input = new Vector3(direction.x, 0, direction.z);
        float targetY = Quaternion.LookRotation(input).eulerAngles.y + _cameraReference.eulerAngles.y;
        Quaternion finalRotation = Quaternion.Euler(0, targetY, 0);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, finalRotation, FlyWeightPointer.Player.rotateSpeed * Time.fixedDeltaTime));
    }
}
