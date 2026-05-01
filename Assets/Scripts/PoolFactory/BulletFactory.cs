using UnityEngine;

public class BulletFactory : Factory<Bullet>
{
    public BulletFactory(Bullet prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }
}
