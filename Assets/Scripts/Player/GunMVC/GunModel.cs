using System.Collections.Generic;
using UnityEngine;
// Que dibuja el arma mientras apunta, segun el tipo de disparo:
// Laser = linea central con dot (pistola). Cone = V con los dos bordes
// del abanico (escopeta). None = nada.
public enum AimIndicatorType
{
    None,
    Laser,
    Cone
}
public class GunModel
{
    private Dictionary<bool, IGunRotationStrategy> _strategies;
    private IGunRotationStrategy _currentStrategy;
    private IShootStrategy _shootStrategy;
    private LayerMask _hitMask;
    private float _maxDistance;
    // Arco del cono DEL ARMA ACTIVA: se actualiza en cada SetShootStrategy
    // (viene del asset del arma equipada).
    // _coneRange si es global:
    // la distancia de vida del proyectil (maxLife), igual para todas.
    private float _coneArcDegrees;
    private float _coneRange;
    // Arranca en None: el modelo nace SIN arma equipada y no dibuja nada
    // hasta que Gun llama ApplyShootType (mismo frame, en Start).
    private AimIndicatorType _indicator = AimIndicatorType.None;
    public bool IsAiming { get; private set; }
    public GunModel(Quaternion initLocalRotation, LayerMask hitMask, float maxDistance, float coneRange)
    {
        _hitMask = hitMask;
        _maxDistance = maxDistance;
        _coneRange = coneRange;
        _strategies = new Dictionary<bool, IGunRotationStrategy>
        {
            { false, new GunIdleRotationStrategy(initLocalRotation) },
            { true,  new GunAimRotationStrategy(initLocalRotation) }
        };
        _currentStrategy = _strategies[false];
    }
    // Se ejecuta al equipar un arma (arranque y ciclado). Deja guardada la
    // config activa: que estrategia dispara, que se dibuja y con que arco.
    public void SetShootStrategy(IShootStrategy shootStrategy, AimIndicatorType indicator, float coneArcDegrees)
    {
        _shootStrategy = shootStrategy;
        _indicator = indicator;
        _coneArcDegrees = coneArcDegrees;
    }
    public void SetAiming(bool isAiming)
    {
        IsAiming = isAiming;
        _currentStrategy = _strategies[isAiming];
    }
    public Quaternion ComputeTargetLocalRotation(Vector3 aimPoint, Vector3 gunPosition, Transform parent)
    {
        return _currentStrategy.ComputeTargetLocalRotation(aimPoint, gunPosition, parent);
    }
    public ShotResult Shoot(Vector3 origin, Vector3 aimPoint, Vector3 barrelForward)
    {
        //El modelo no pregunta que arma es: ejecuta la estrategia que tenga puesta.
        //La estrategia se setea en SetShootStrategy: lo llama Gun al equipar la primera arma en Start y en cada cambio de arma.
        Vector3 direction = IsAiming ? (aimPoint - origin).normalized : barrelForward;
        return _shootStrategy.Shoot(origin, direction);
    }
    public AimIndicatorState ComputeAimIndicator(Vector3 origin, Vector3 aimPoint)
    {
        AimIndicatorState state = new AimIndicatorState();
        if (!IsAiming || _indicator == AimIndicatorType.None)
            return state;
        Vector3 direction = (aimPoint - origin).normalized;
        // Y aca _indicator decide QUE geometria se calcula este frame:
        // Laser -> una linea central. Cone -> los dos bordes de la V.
        // Las lineas que no se calculan quedan isVisible = false (default del struct).
        switch (_indicator)
        {
            case AimIndicatorType.Laser:
                state.laser = ComputeLine(origin, direction, _maxDistance);
                break;
            case AimIndicatorType.Cone:
                // Bordes de la V: la misma rotacion sobre Y con la que la
                // escopeta abre los perdigones -> telegraph = disparo real.
                float halfArc = _coneArcDegrees * 0.5f;
                state.coneLeft = ComputeLine(origin, Quaternion.AngleAxis(-halfArc, Vector3.up) * direction, _coneRange);
                state.coneRight = ComputeLine(origin, Quaternion.AngleAxis(halfArc, Vector3.up) * direction, _coneRange);
                break;
        }
        return state;
    }
    //Raycast que utuliza para calcular las lineas de dibujado de los line renderers.
    private LaserState ComputeLine(Vector3 origin, Vector3 direction, float maxDistance)
    {
        bool hasHit = Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, _hitMask, QueryTriggerInteraction.Ignore);
        return new LaserState
        {
            isVisible = true,
            start = origin,
            end = hasHit ? hit.point : origin + direction * maxDistance,
            hasHit = hasHit,
            hitNormal = hasHit ? hit.normal : Vector3.zero
        };
    }
}
public struct LaserState
{
    public bool isVisible;
    public Vector3 start;
    public Vector3 end;
    public bool hasHit;
    public Vector3 hitNormal;
}
public struct AimIndicatorState
{
    public LaserState laser;
    public LaserState coneLeft;
    public LaserState coneRight;
}
