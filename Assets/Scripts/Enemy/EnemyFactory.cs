using UnityEngine;
public class EnemyFactory : Factory<Enemy>
{
    public EnemyFactory(Enemy prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }
}
