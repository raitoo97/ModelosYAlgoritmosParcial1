using UnityEngine;
[CreateAssetMenu(menuName = "Weapons/Spread Weapon", fileName = "SpreadWeapon")]
public class SpreadWeaponConfig : WeaponConfig
{
    [SerializeField] private int _pelletCount = 5;
    [SerializeField] private float _spreadArcDegrees = 35f;
    // MULTIPLICADOR: Bullet lo aplica sobre el danio base al impactar.
    [SerializeField] private float _pelletDamageMultiplier = 0.8f;
    [SerializeField] private float _pelletScale = 1.6f;
    // El MISMO arco alimenta el disparo y la V del indicador:
    // una sola fuente de verdad, el telegraph nunca se desincroniza.
    public override float ConeArcDegrees => _spreadArcDegrees;
    public override IShootStrategy CreateStrategy(ShootStrategyDependencies deps)
    {
        return new SpreadShootStrategy(deps.bulletService, deps.owner, WeaponColor, _pelletCount, _spreadArcDegrees, FlyWeightPointer.Player.damage * _pelletDamageMultiplier, deps.onImpact, _pelletScale);
    }
}
