using UnityEngine;
public class EnemyService
{
    private Pool<Enemy> _pool;
    private EnemyFactory _factory;
    public EnemyService(Enemy prefab, Transform parent, int size)
    {
        _factory = new EnemyFactory(prefab, parent);
        _pool = new Pool<Enemy>(CreateEnemy, TurnOn, TurnOff, size);
    }
    private Enemy CreateEnemy()
    {
        Enemy enemy = _factory.CreateObject();
        enemy.SetReturnToPoolCallBack(ReturnToPool);
        return enemy;
    }
    public Enemy Spawn(Vector3 position, Quaternion rot)
    {
        Enemy enemy = _pool.GetObject();
        enemy.transform.position = position;
        enemy.transform.rotation = rot;
        return enemy;
    }
    private void TurnOn(Enemy enemy)
    {
        enemy.ResetEnemy();
        enemy.gameObject.SetActive(true);
    }
    private void TurnOff(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }
    public void ReturnToPool(Enemy enemy)
    {
        _pool.ReturnObject(enemy);
    }
}
