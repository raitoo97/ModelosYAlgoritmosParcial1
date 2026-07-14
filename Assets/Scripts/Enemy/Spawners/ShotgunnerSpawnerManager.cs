using UnityEngine;
using UnityEngine.AI;
public class ShotgunnerSpawnerManager : EnemySpawnerManager
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _bulletPoolSize = 50;
    [SerializeField] private int _poolSize = 20;
    [SerializeField] private float _spawnRate = 8f;
    [SerializeField] private float _spawnRateDecrease = 1f;
    [SerializeField] private float _minSpawnRate = 2f;
    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _spawnHeight = 1.5f;
    [SerializeField] private int _pelletCount = 5;
    [SerializeField] private float _spreadArcDegrees = 35f;
    [SerializeField] private float _pelletDamageMultiplier = 0.5f;
    private float _timer;
    protected override IShootStrategy CreateShootStrategy()
    {
        BulletService bulletService = new BulletService(_bulletPrefab, GameManager.instance.projectilesParent, _bulletPoolSize);
        // Naranja para distinguir los balas de las balas rojas del Ranger.
        return new SpreadShootStrategy(bulletService, Factions.Enemy, new Color(1f, 0.5f, 0f), _pelletCount, _spreadArcDegrees, _pelletDamageMultiplier, BulletImpactCallback);
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
        if (!IsActive) return;
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
        // Busco el punto navegable mas cercano (hasta 2 unidades) al punto aleatorio.
        // Si no encuentro un punto valido del NavMesh, no hago el spawn.
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            _enemyService.Spawn(hit.position);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
