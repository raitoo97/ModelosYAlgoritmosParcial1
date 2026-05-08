using UnityEngine;
public class EnemyService
{
    private Pool<Enemy> _pool;
    private EnemyFactory _factory;
    private IObserver<EnemyEvent> _observer;
    public EnemyService(Enemy prefab, Transform parent, int size , IObserver<EnemyEvent> observer)
    {
        _factory = new EnemyFactory(prefab, parent);
        _observer = observer;
        _pool = new Pool<Enemy>(CreateEnemy, TurnOn, TurnOff, size);
    }
    private Enemy CreateEnemy()
    {
        Enemy enemy = _factory.CreateObject();
        enemy.SetReturnToPoolCallBack(ReturnToPool);
        enemy.Subscribe(_observer);
        return enemy;
    }
    public Enemy Spawn(Vector3 position)
    {
        Enemy enemy = _pool.GetObject();
        enemy.WarpToPosition(position);
        enemy.ResetEnemy();
        return enemy;
    }
    private void TurnOn(Enemy enemy)
    {
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
