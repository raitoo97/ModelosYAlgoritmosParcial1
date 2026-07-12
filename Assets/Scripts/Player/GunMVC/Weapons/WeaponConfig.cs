using System;
using UnityEngine;
//Dependencias que las estrategias necesitan
//El Gun arma este paquete una sola vez y se lo pasa a cada config , cada una usa solo lo que necesita.
public struct ShootStrategyDependencies
{
    public BulletService bulletService;
    public Factions owner;
    public LayerMask hitMask;
    public float maxDistance;
    public Action<Vector3, Vector3> onImpact;
}
//  cada config SABE CONSTRUIR su estrategia
public abstract class WeaponConfig : ScriptableObject
{
    // Que dibuja el arma al apuntar (Laser, Cone, None).
    [SerializeField] private AimIndicatorType _indicator;
    public AimIndicatorType Indicator => _indicator;
    [SerializeField] private Color _weaponColor = Color.white;
    public Color WeaponColor => _weaponColor;
    // Icono del arma para el HUD. Vive en el asset como el color:
    // agregar un arma nueva ya trae su icono, la UI no se toca.
    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;
    // Arco del cono para el indicador en V. Solo lo redefine el arma que
    // dispara en abanico; para el resto 0 (no se usa).
    public virtual float ConeArcDegrees => 0f;
    // Cada arma concreta construye SU estrategia con SUS datos + las deps.
    public abstract IShootStrategy CreateStrategy(ShootStrategyDependencies deps);
}
