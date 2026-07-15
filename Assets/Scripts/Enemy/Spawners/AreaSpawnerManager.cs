using UnityEngine;
using UnityEngine.AI;
// Spawner por AREA con timer: cada cierto tiempo spawnea un enemigo en una
// posicion aleatoria dentro de un radio, apoyada en el NavMesh.
// Ranger y Shotgunner solo cambian su estrategia (y el color del gizmo).
public abstract class AreaSpawnerManager : EnemySpawnerManager
{
    [SerializeField] private int _poolSize = 20;
    [SerializeField] private float _spawnRate = 5f;
    [SerializeField] private float _spawnRateDecrease = 1f;
    [SerializeField] private float _minSpawnRate = 1f;
    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _spawnHeight = 1.5f;
    private float _timer;
    // Cada tipo pinta su gizmo para distinguirlos en la Scene view.
    protected virtual Color GizmoColor => Color.white;
    protected override int GetPoolSize() => _poolSize;
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
        Gizmos.color = GizmoColor;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
