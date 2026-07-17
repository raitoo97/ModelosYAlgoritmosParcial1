using System;
using UnityEngine;
public class BulletBuilder
{
    private Bullet _bullet;
    public BulletBuilder(Bullet bullet)
    {
        _bullet = bullet;
    }
    public BulletBuilder SetDamageBullet(float damage)
    {
        _bullet.SetDamage(damage);
        return this;
    }
    public BulletBuilder SetColorMaterial(Color color)
    {
        _bullet.SetColor(color);
        return this;
    }
    public BulletBuilder SetOwnerBullet(Factions owner)
    {
        _bullet.SetOwner(owner);
        return this;
    }
    public BulletBuilder SetOnImpactBullet(Action<Vector3, Vector3> onImpact)
    {
        _bullet.SetOnImpact(onImpact);
        return this;
    }
    public BulletBuilder SetScaleBullet(float scaleMultiplier)
    {
        _bullet.SetScale(scaleMultiplier);
        return this;
    }
    public BulletBuilder SetPiercingBullet(bool isPiercing)
    {
        _bullet.SetPiercing(isPiercing);
        return this;
    }
    public Bullet Build()
    {
        return _bullet;
    }
}