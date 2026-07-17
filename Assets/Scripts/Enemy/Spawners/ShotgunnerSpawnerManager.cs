using UnityEngine;
public class ShotgunnerSpawnerManager : AreaSpawnerManager
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _bulletPoolSize = 50;
    [SerializeField] private int _pelletCount = 5;
    [SerializeField] private float _spreadArcDegrees = 35f;
    protected override Color GizmoColor => new Color(1f, 0.5f, 0f);
    protected override IShootStrategy CreateShootStrategy()
    {
        BulletService bulletService = new BulletService(_bulletPrefab, GameManager.instance.projectilesParent, _bulletPoolSize);
        // Naranja para distinguir las balas de las rojas del Ranger.
        return new SpreadShootStrategy(bulletService, Factions.Enemy, new Color(1f, 0.5f, 0f), _pelletCount, _spreadArcDegrees, FlyWeightPointer.Shotgunner.damage, BulletImpactCallback);
    }
}
