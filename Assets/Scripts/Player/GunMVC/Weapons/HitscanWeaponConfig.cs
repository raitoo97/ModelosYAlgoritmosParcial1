using UnityEngine;
[CreateAssetMenu(menuName = "Weapons/Hitscan Weapon", fileName = "HitscanWeapon")]
public class HitscanWeaponConfig : WeaponConfig
{
    // MULTIPLICADOR sobre el danio base. El hitscan recibe el danio PLANO ya
    // resuelto (base * esto). OJO: distinto de la escopeta, cuyo multiplicador
    // viaja en la bala y lo aplica Bullet al impactar.
    [SerializeField] private float _damageMultiplier = 4f;
    public override IShootStrategy CreateStrategy(ShootStrategyDependencies deps)
    {
        return new HitscanShootStrategy(FlyWeightPointer.Projectile.damage * _damageMultiplier, deps.owner, deps.hitMask, deps.maxDistance);
    }
}
