using UnityEngine;
public class SpreadShootStrategy : IShootStrategy
{
    private BulletService _bulletService;
    private Factions _owner;
    private Color _bulletColor;
    private int _pelletCount;
    private float _totalArcDegrees;
    private float _damageMultiplier;
    public SpreadShootStrategy(BulletService bulletService, Factions owner, Color bulletColor, int pelletCount, float totalArcDegrees, float damageMultiplier)
    {
        _bulletService = bulletService;
        _owner = owner;
        _bulletColor = bulletColor;
        _pelletCount = pelletCount;
        _totalArcDegrees = totalArcDegrees;
        _damageMultiplier = damageMultiplier;
    }
    public ShotResult Shoot(Vector3 origin, Vector3 direction)
    {
        // Reparto las balas en un abanico centrado en la direccion original.
        // Si hay una sola bala, sale derecho sin offset.
        // Calculo la separacion entre balas.
        // EJ: _pelletCount = 5, _totalArcDegrees = 90
        // step = 90 / (5 - 1) = 22.5
        float step = _pelletCount > 1 ? _totalArcDegrees / (_pelletCount - 1) : 0f;
        // Divido el arco por dos porque quiero que el abanico quede centrado
        // en la direccion original.
        // EJ: 90 -> primera bala a -45 y ultima a +45.
        float startAngle = -_totalArcDegrees * 0.5f;
        for (int i = 0; i < _pelletCount; i++)
        {
            // Calculo el angulo de esta bala.
            // EJ: startAngle = -45, step = 22.5
            // i = 0 -> -45
            // i = 1 -> -22.5
            // i = 2 ->   0
            // i = 3 ->  22.5
            // i = 4 ->  45
            float angle = startAngle + step * i;
            // Roto la direccion original sobre el eje Y para obtener
            // la direccion final de esta bala dentro del abanico.
            Vector3 pelletDir = Quaternion.AngleAxis(angle, Vector3.up) * direction;
            Bullet bullet = _bulletService.Shoot(origin, Quaternion.LookRotation(pelletDir));
            new BulletBuilder(bullet)
                .SetColorMaterial(_bulletColor)
                .SetOwnerBullet(_owner)
                .SetDamageMultiplierBullet(_damageMultiplier)
                .Build();
        }
        return new ShotResult { didHit = false };
    }
}
