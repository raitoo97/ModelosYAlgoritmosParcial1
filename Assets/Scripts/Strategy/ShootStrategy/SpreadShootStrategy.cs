using System;
using UnityEngine;
public class SpreadShootStrategy : IShootStrategy
{
    private BulletService _bulletService;
    private Factions _owner;
    private Color _bulletColor;
    private int _pelletCount;
    private float _totalArcDegrees;
    private float _damageMultiplier;
    private Action<Vector3, Vector3> _onImpact;
    private float _bulletScale;
    public SpreadShootStrategy(BulletService bulletService, Factions owner, Color bulletColor, int pelletCount, float totalArcDegrees, float damageMultiplier,Action<Vector3, Vector3> onImpact = null,float bulletScale = 1f)
    {
        _bulletService = bulletService;
        _owner = owner;
        _bulletColor = bulletColor;
        _pelletCount = pelletCount;
        _totalArcDegrees = totalArcDegrees;
        _damageMultiplier = damageMultiplier;
        _bulletScale = bulletScale;
        _onImpact = onImpact;
    }
    public ShotResult Shoot(Vector3 origin, Vector3 direction)
    {
        // Reparto las balas en un abanico centrado en la direccion original.
        // Si hay una sola bala, sale derecho sin offset.
        // Calculo la separacion entre balas.
        // EJ: _pelletCount = 5, _totalArcDegrees = 90
        // step = 90 / (5 - 1) = 22.5
        float step = _pelletCount > 1 ? _totalArcDegrees / (_pelletCount - 1) : 0f;
        // EL MENOS: startAngle es el angulo de la PRIMERA bala, no la mitad
        // del arco. El loop solo SUMA step hacia la derecha, asi que hay que
        // arrancar parado en el borde IZQUIERDO (-arco/2) para terminar en el
        // derecho (+arco/2) y que el centro quede justo donde apunta el laser.
        // EJ: 3 balas, arco 12 -> step 6 -> angulos -6, 0, +6 (centrado).
        // Sin el menos serian +6, +12, +18: todo el abanico corrido a la
        // derecha y desincronizado de la V del indicador, que GunModel
        // dibuja centrada con -halfArc / +halfArc.
        // CASO 1 BALA: step = 0 no corrige nada, asi que arranca en 0
        // para salir por el centro.
        // EJ: 90 -> primera bala a -45 y ultima a +45.
        float startAngle = _pelletCount > 1 ? -_totalArcDegrees * 0.5f : 0f;
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
                .SetScaleBullet(_bulletScale)
                .SetOnImpactBullet(_onImpact)
                .Build();
        }
        return new ShotResult { didHit = false };
    }
}
