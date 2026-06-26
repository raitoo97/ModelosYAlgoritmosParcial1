using System;
using System.Collections.Generic;
using UnityEngine;
public class EnemySpawner : MonoBehaviour , IObserver<EnemyEvent> ,IPauseable
{
    [SerializeField] private Enemy _enemyPrefab;
    private float _spawnRate = 5f;
    private float _radius = 10f;
    private int _poolSize = 20;
    private EnemyService _enemyService;
    private float _timer;
    private Dictionary<EnemyEvent, Action> _actions;
    private int _enemyKills;
    private int _killsPerDifficultyIncrease = 5;
    private float _minSpawnRate = 1f;
    private void Start()
    {
        _enemyService = new EnemyService(_enemyPrefab, this.transform, _poolSize,this);
        SaveManager.instance.Subscribe(_enemyService);
        FillDictionary();
    }
    private void Update()
    {
        if (GameState.IsPaused) return;
        _timer += Time.deltaTime;
        if (_timer >= _spawnRate)
        {
            _timer = 0;
            SpawnEnemy();
        }
    }
    private void FillDictionary()
    {
        _actions = new Dictionary<EnemyEvent, Action>();
        _actions.Add(EnemyEvent.EnemyDie, IncreaseEnemyKilled);
    }
    private void IncreaseEnemyKilled()
    {
        _enemyKills++;
        if (_enemyKills % _killsPerDifficultyIncrease == 0)
        {
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
