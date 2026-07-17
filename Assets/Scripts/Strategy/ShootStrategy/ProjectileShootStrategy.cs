using System;
using UnityEngine;
public class ProjectileShootStrategy : IShootStrategy
{
    private BulletService _bulletService;
    private Factions _owner;
    private Color _bulletColor;
    private Action<Vector3, Vector3> _onImpact;
    private float _damage;
    public ProjectileShootStrategy(BulletService bulletService, Factions owner, Color bulletColor,float damage, Action<Vector3, Vector3> onImpact = null)
    {
        _bulletService = bulletService;
        _owner = owner;
        _bulletColor = bulletColor;
        _onImpact = onImpact;
        _damage = damage;
    }
    public ShotResult Shoot(Vector3 origin, Vector3 direction)
    {
        Bullet bullet = _bulletService.Shoot(origin, Quaternion.LookRotation(direction));
        new BulletBuilder(bullet)
            .SetColorMaterial(_bulletColor)
            .SetOwnerBullet(_owner)
            .SetOnImpactBullet(_onImpact)
            .SetDamageBullet(_damage)
            .Build();
        return new ShotResult { didHit = false };
    }
}
