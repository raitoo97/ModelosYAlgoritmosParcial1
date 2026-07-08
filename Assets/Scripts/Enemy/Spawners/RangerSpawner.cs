using System;
using System.Collections.Generic;
using UnityEngine;
public class RangerSpawner : MonoBehaviour , IObserver<EnemyEvent> ,IPauseable
{
    [SerializeField] private Enemy _enemyPrefab;
    private float _spawnRate = 5f;
    private float _radius = 10f;
    private int _poolSize = 20;
    private EnemyService _enemyService;
    [SerializeField]private Bullet _bulletPrefab;
    [SerializeField]private int _bulletPoolSize = 50;
    private float _timer;
    private Dictionary<EnemyEvent, Action> _actions = new Dictionary<EnemyEvent, Action>();
    private int _enemyKills;
    private int _killsPerDifficultyIncrease = 5;
    private float _minSpawnRate = 1f;
    private void Start()
    {
        BulletService bulletService = new BulletService(_bulletPrefab, GameManager.instance._projectilesParent, _bulletPoolSize);
        IShootStrategy enemyStrategy = new ProjectileShootStrategy(bulletService, Factions.Enemy, Color.red);
        _enemyService = new EnemyService(_enemyPrefab, this.transform, _poolSize, this, enemyStrategy);
        SaveManager.instance.Subscribe(_enemyService);
        FillDictionary();
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
    private void FillDictionary()
    {
        _actions.Add(EnemyEvent.EnemyDie, IncreaseEnemyKilled);
    }
    private void IncreaseEnemyKilled()
    {
        EventManager.TriggerEvent(EventType.EnemyKilled, 1);
        _enemyKills++;
        if (_enemyKills % _killsPerDifficultyIncrease == 0)
        {
            Debug.Log("Increasing difficulty, new spawn rate: " + (_spawnRate - 1f));
            _spawnRate -= 1f;
            _spawnRate = Mathf.Max(_minSpawnRate, _spawnRate);
        }
    }
    private void SpawnEnemy()
    {
        Vector3 randomPos = transform.position + UnityEngine.Random.insideUnitSphere * _radius;
        randomPos.y = 1.5f;
        _enemyService.Spawn(randomPos);
    }
    public void Notify(EnemyEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
        {
            _actions[Actions].Invoke();
        }
    }
    private void OnDestroy()
    {
        SaveManager.instance.Unsubscribe(_enemyService);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, 10f);
    }
    public void Pause()
    {
        enabled = false;
    }
    public void Resume()
    {
        enabled = true;
    }
}
