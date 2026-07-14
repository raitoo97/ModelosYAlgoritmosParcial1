using UnityEngine;
public class SniperSpawnerManager : EnemySpawnerManager
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private float _respawnTime;
    [SerializeField] private float _respawnTimeDecrease = 1f;
    [SerializeField] private float _minRespawnTime = 1f;
    private Enemy[] _activeSnipers;
    private float[] _respawnTimers;
    protected override void Start()
    {
        base.Start(); // la base crea el EnemyService y se suscribe al SaveManager
        _activeSnipers = new Enemy[_spawnPoints.Length];
        _respawnTimers = new float[_spawnPoints.Length];
    }
    protected override IShootStrategy CreateShootStrategy()
    {
        return new HitscanShootStrategy(FlyWeightPointer.Sniper.damage, Factions.Enemy, _hitMask, FlyWeightPointer.Sniper.maxDistance);
    }
    protected override int GetPoolSize()
    {
        return _spawnPoints.Length;
    }
    protected override void IncreaseDifficulty()
    {
        _respawnTime = Mathf.Max(_minRespawnTime, _respawnTime - _respawnTimeDecrease);
    }
    protected override void OnActivated()
    {
        for (int i = 0; i < _spawnPoints.Length; i++)
            _activeSnipers[i] = _enemyService.Spawn(_spawnPoints[i].position);
    }
    private void Update()
    {
        if (!IsActive) return;
        SpawnSnipers();
    }
    private void SpawnSnipers()
    {
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            if (_activeSnipers[i] != null && _activeSnipers[i].gameObject.activeSelf) continue;
            _activeSnipers[i] = null;
            _respawnTimers[i] += Time.deltaTime;
            if (_respawnTimers[i] >= _respawnTime)
            {
                _respawnTimers[i] = 0;
                Enemy spawned = _enemyService.Spawn(_spawnPoints[i].position);
                for (int j = 0; j < _activeSnipers.Length; j++)
                    if (j != i && _activeSnipers[j] == spawned) _activeSnipers[j] = null;
                _activeSnipers[i] = spawned;
            }
        }
    }
}
