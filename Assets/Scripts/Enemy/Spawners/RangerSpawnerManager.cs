using UnityEngine;
public class RangerSpawnerManager : EnemySpawnerManager
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _bulletPoolSize = 50;
    [SerializeField] private int _poolSize = 20;
    [SerializeField] private float _spawnRate = 5f;
    [SerializeField] private float _spawnRateDecrease = 1f;
    [SerializeField] private float _minSpawnRate = 1f;
    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _spawnHeight = 1.5f;
    private float _timer;
    protected override IShootStrategy CreateShootStrategy()
    {
        BulletService bulletService = new BulletService(_bulletPrefab, GameManager.instance._projectilesParent, _bulletPoolSize);
        return new ProjectileShootStrategy(bulletService, Factions.Enemy, Color.red);
    }
    protected override int GetPoolSize()
    {
        return _poolSize;
    }
    protected override void IncreaseDifficulty()
    {
        _spawnRate = Mathf.Max(_minSpawnRate, _spawnRate - _spawnRateDecrease);
    }
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _spawnRate)
        {
            _timer = 0;
            SpawnEnemy();
        }
    }
    private void SpawnEnemy()
    {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * _radius;
        randomPos.y = _spawnHeight;
        _enemyService.Spawn(randomPos);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
