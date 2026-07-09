using System;
using UnityEngine;
public class BulletBuilder
{
    private Bullet _bullet;
    public BulletBuilder(Bullet bullet)
    {
        _bullet = bullet;
    }
    public BulletBuilder SetDamageMultiplierBullet(float multiplier)
    {
        _bullet.SetDamageMultiplier(multiplier);
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
    public Bullet Build()
    {
        return _bullet;
    }
}