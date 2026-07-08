using UnityEngine;

public class ProjectileShootStrategy : IShootStrategy
{
    private BulletService _bulletService;
    private Factions _owner;
    private Color _bulletColor;
    public ProjectileShootStrategy(BulletService bulletService, Factions owner, Color bulletColor)
    {
        _bulletService = bulletService;
        _owner = owner;
        _bulletColor = bulletColor;
    }
    public ShotResult Shoot(Vector3 origin, Vector3 direction)
    {
        Bullet bullet = _bulletService.Shoot(origin, Quaternion.LookRotation(direction));
        new BulletBuilder(bullet)
            .SetColorMaterial(_bulletColor)
            .SetOwnerBullet(_owner)
            .Build();
        return new ShotResult { didHit = false };
    }
}
