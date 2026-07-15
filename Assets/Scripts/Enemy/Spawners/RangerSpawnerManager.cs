using UnityEngine;
public class RangerSpawnerManager : AreaSpawnerManager
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _bulletPoolSize = 50;
    protected override Color GizmoColor => Color.blue;
    protected override IShootStrategy CreateShootStrategy()
    {
        BulletService bulletService = new BulletService(_bulletPrefab, GameManager.instance.projectilesParent, _bulletPoolSize);
        return new ProjectileShootStrategy(bulletService, Factions.Enemy, Color.red, BulletImpactCallback);
    }
}
