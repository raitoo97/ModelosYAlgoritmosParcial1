using UnityEngine;
public class GunAimRotationStrategy : IGunRotationStrategy
{
    private Quaternion _initLocalRotation;
    private const float MaxAimDegrees = 2f;
    public GunAimRotationStrategy(Quaternion initLocalRotation)
    {
        _initLocalRotation = initLocalRotation;
    }
    public Quaternion ComputeTargetLocalRotation(Vector3 aimPoint, Vector3 gunPosition, Transform parent)
    {
        Vector3 desiredDirection = (aimPoint - gunPosition).normalized;
        if (desiredDirection.sqrMagnitude < 0.0001f)
            return _initLocalRotation;
        //paso la pose de reposo de LOCAL a MUNDO combinandola con la rotacion del padre.
        Quaternion restWorldRotation = parent != null ? parent.rotation * _initLocalRotation : _initLocalRotation;
        //consigo el forward del arma a partir de esa rotacion
        Vector3 restForward = restWorldRotation * Vector3.forward;
        //creo un vector 3 en que no tengo en cuenta el vector Y con la direccion de restForward en X,Z.
        Vector3 flatRest = new Vector3(restForward.x, 0f, restForward.z);
        //obtengo los componentes X y Z de la direccion deseada.
        Vector3 flatDesired = new Vector3(desiredDirection.x, 0f, desiredDirection.z);
        //si restForward o desiredDirection apuntan casi en VERTICAL, su proyeccion
        //horizontal queda ~0 y no puedo calcular un yaw fiable -> devuelvo la inicial.
        if (flatRest.sqrMagnitude < 0.0001f || flatDesired.sqrMagnitude < 0.0001f)
            return _initLocalRotation;
        //normalizo ambas direcciones
        flatRest.Normalize();
        flatDesired.Normalize();
        //roto la direccion de descanso hacia la deseada, COMO MAXIMO MaxAimDegrees grados (clamp).
        Vector3 clampedFlat = Vector3.RotateTowards(flatRest, flatDesired, MaxAimDegrees * Mathf.Deg2Rad, 0f);
        //cuanto grados necesito para pasar de flatRest a clampedFlat
        Quaternion yaw = Quaternion.FromToRotation(flatRest, clampedFlat);
        //aplico el yaw sobre la pose de reposo en mundo (los quaternions se combinan
        //multiplicando, no sumando; el orden importa).
        Quaternion targetWorldRotation = yaw * restWorldRotation;
        //multiplico por la inversa del padre para pasar de MUNDO a LOCAL.
        return parent != null ? Quaternion.Inverse(parent.rotation) * targetWorldRotation : targetWorldRotation;
    }
}
