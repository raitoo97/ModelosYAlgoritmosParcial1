using UnityEngine;
public class BulletBuilder
{
    private Bullet _bullet;
    public BulletBuilder(Bullet bullet)
    {
        _bullet = bullet;
    }
    public BulletBuilder SetSpeed(float speed)
    {
        _bullet.SetSpeed(speed);
        return this;
    }
    public BulletBuilder SetDamage(float damage)
    {
        _bullet.SetDamage(damage);
        return this;
    }
    public BulletBuilder SetColorMaterial(Color color)
    {
        _bullet.SetColor(color);
        return this;
    }
    public Bullet Build()
    {
        return _bullet;
    }
}