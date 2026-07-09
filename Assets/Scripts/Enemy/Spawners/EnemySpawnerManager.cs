using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class EnemySpawnerManager : MonoBehaviour, IObserver<EnemyEvent>, IPauseable
{
    [SerializeField] protected Enemy _enemyPrefab;
    [SerializeField] protected int _killsPerDifficultyIncrease = 5;
    protected EnemyService _enemyService;
    private Dictionary<EnemyEvent, Action> _actions = new Dictionary<EnemyEvent, Action>();
    private int _enemyKills;
    protected virtual void Start()
    {
        _enemyService = new EnemyService(_enemyPrefab, transform, GetPoolSize(), this, CreateShootStrategy(), GameManager.instance.player.transform);
        SaveManager.instance.Subscribe(_enemyService);
        FillDictionary();
    }
    protected abstract IShootStrategy CreateShootStrategy();
    protected abstract int GetPoolSize();
    protected abstract void IncreaseDifficulty();
    private void FillDictionary()
    {
        _actions.Add(EnemyEvent.EnemyDie, OnEnemyKilled);
    }
    private void OnEnemyKilled()
    {
        EventManager.TriggerEvent(EventType.EnemyKilled, 1);
        _enemyKills++;
        if (_enemyKills % _killsPerDifficultyIncrease == 0)
            IncreaseDifficulty();
    }
    public void Notify(EnemyEvent action)
    {
        if (_actions.ContainsKey(action))
            _actions[action].Invoke();
    }
    public void Pause() { enabled = false; }
    public void Resume() { enabled = true; }
    protected virtual void OnDestroy()
    {
        SaveManager.instance.Unsubscribe(_enemyService);
    }
}
