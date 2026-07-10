using UnityEngine;
[CreateAssetMenu(menuName = "Weapons/Piercing Weapon", fileName = "PiercingWeapon")]
public class PiercingWeaponConfig : WeaponConfig
{
    [SerializeField] private Color _bulletColor = new Color(0.85f, 0.3f, 1f);
    [SerializeField] private int _bulletCount = 3;
    // Cono bien mas cerrado que los 35 grados de la escopeta.
    [SerializeField] private float _arcDegrees = 12f;
    // MULTIPLICADOR: Bullet lo aplica sobre el danio base al impactar.
    [SerializeField] private float _damageMultiplier = 1.5f;
    [SerializeField] private float _bulletScale = 2.5f;
    // El MISMO arco alimenta el disparo y la V del indicador:
    // una sola fuente de verdad, el telegraph nunca se desincroniza.
    public override float ConeArcDegrees => _arcDegrees;
    public override IShootStrategy CreateStrategy(ShootStrategyDependencies deps)
    {
        return new PiercingShootStrategy(deps.bulletService, deps.owner, _bulletColor, _bulletCount, _arcDegrees, _damageMultiplier, _bulletScale, deps.onImpact);
    }
}
