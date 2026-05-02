using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private float _spawnRate = 2f;
    [SerializeField] private float _radius = 10f;
    [SerializeField] private int _poolSize = 20;
    private EnemyService _enemyService;
    private float _timer;
    private void Start()
    {
        _enemyService = new EnemyService(_enemyPrefab, _parent, _poolSize);
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
        randomPos.y = 0;
        _enemyService.Spawn(randomPos,Quaternion.identity);
    }
}
