using UnityEngine;
[CreateAssetMenu(menuName = "Weapons/Hitscan Weapon", fileName = "HitscanWeapon")]
public class HitscanWeaponConfig : WeaponConfig
{
    [SerializeField] private float _damageMultiplier = 4f;
    public override IShootStrategy CreateStrategy(ShootStrategyDependencies deps)
    {
        return new HitscanShootStrategy(FlyWeightPointer.Projectile.damage * _damageMultiplier, deps.owner, deps.hitMask, deps.maxDistance);
    }
}
