using UnityEngine;
public class GunAimRotationStrategy : IGunRotationStrategy
{
    private Quaternion _initLocalRotation;
    public GunAimRotationStrategy(Quaternion initLocalRotation)
    {
        _initLocalRotation = initLocalRotation;
    }
    public Quaternion ComputeTargetLocalRotation(Vector3 aimPoint, Vector3 gunPosition, Transform parent)
    {
        Vector3 desiredDirection = (aimPoint - gunPosition).normalized;
        if (desiredDirection.sqrMagnitude < 0.0001f)
            return _initLocalRotation;
        //si el padre es disinto de nulo sumo la rotacion del padre a la rotacion local inicial para obtener la rotacion en el mundo
        Quaternion restWorldRotation = parent != null ? parent.rotation * _initLocalRotation : _initLocalRotation;
        //obtengo el forward del arma cuando esta en reposo
        Vector3 restForward = restWorldRotation * Vector3.forward;
        //girdo el forward del arma hacia la direccion deseada
        Vector3 clampedDirection = Vector3.RotateTowards(restForward, desiredDirection,2 * Mathf.Deg2Rad,0f);
        //Que rotacion necesito aplicar para que a pase a ser b? y luego aplico esa rotacion sobre la rotacion en reposo.
        Quaternion targetWorldRotation = Quaternion.FromToRotation(restForward, clampedDirection) * restWorldRotation;
        //si el padre es distinto de nulo, aplico la rotacion inversa del padre para obtener la rotacion local
        return parent != null ? Quaternion.Inverse(parent.rotation) * targetWorldRotation : targetWorldRotation;
    }
}
