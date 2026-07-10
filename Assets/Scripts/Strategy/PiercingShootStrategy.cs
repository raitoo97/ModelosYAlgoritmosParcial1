using System;
using UnityEngine;
public class PiercingShootStrategy : IShootStrategy
{
    private BulletService _bulletService;
    private Factions _owner;
    private Color _bulletColor;
    private int _bulletCount;
    private float _arcDegrees;
    private float _damageMultiplier;
    private float _bulletScale;
    private Action<Vector3, Vector3> _onImpact;
    public PiercingShootStrategy(BulletService bulletService, Factions owner, Color bulletColor, int bulletCount, float arcDegrees, float damageMultiplier, float bulletScale, Action<Vector3, Vector3> onImpact = null)
    {
        _bulletService = bulletService;
        _owner = owner;
        _bulletColor = bulletColor;
        _bulletCount = bulletCount;
        _arcDegrees = arcDegrees;
        _damageMultiplier = damageMultiplier;
        _bulletScale = bulletScale;
        _onImpact = onImpact;
    }
    public ShotResult Shoot(Vector3 origin, Vector3 direction)
    {
        // Mismo reparto en abanico que la escopeta pero con pocas balas
        // grandes en un cono mucho mas cerrado.
        // step NO lleva menos: es la SEPARACION entre bala y bala (una
        // distancia, siempre positiva), no una direccion. El signo que
        // decide hacia donde arranca el abanico vive solo en startAngle:
        // step camina, startAngle posiciona.
        // EJ: _bulletCount = 3, _arcDegrees = 12 -> step = 12 / 2 = 6.
        float step = _bulletCount > 1 ? _arcDegrees / (_bulletCount - 1) : 0f;
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
        float startAngle = _bulletCount > 1 ? -_arcDegrees * 0.5f : 0f;
        for (int i = 0; i < _bulletCount; i++)
        {
            // Calculo el angulo de esta bala.
            // EJ: startAngle = -45, step = 22.5
            // i = 0 -> -45
            // i = 1 -> -22.5
            // i = 2 ->   0
            // i = 3 ->  22.5
            // i = 4 ->  45
            float angle = startAngle + step * i;
            // Roto la direccion sobre el eje Y para obtener la direccion
            // final de esta bala dentro del cono.
            Vector3 bulletDir = Quaternion.AngleAxis(angle, Vector3.up) * direction;
            Bullet bullet = _bulletService.Shoot(origin, Quaternion.LookRotation(bulletDir));
            new BulletBuilder(bullet)
                .SetColorMaterial(_bulletColor)
                .SetOwnerBullet(_owner)
                .SetDamageMultiplierBullet(_damageMultiplier)
                .SetScaleBullet(_bulletScale)
                .SetPiercingBullet(true)
                .SetOnImpactBullet(_onImpact)
                .Build();
        }
        return new ShotResult { didHit = false };
    }
}
