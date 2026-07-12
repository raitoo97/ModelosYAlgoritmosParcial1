using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class EnemySpawnerManager : MonoBehaviour, IObserver<EnemyEvent>, IPauseable
{
    [SerializeField] protected Enemy _enemyPrefab;
    [SerializeField] protected int _killsPerDifficultyIncrease = 5;
    [SerializeField] private ImpactEffect _impactEffectPrefab;
    [SerializeField] private int _impactEffectPoolSize = 10;
    protected EnemyService _enemyService;
    private Dictionary<EnemyEvent, Action> _actions = new Dictionary<EnemyEvent, Action>();
    protected Action<Vector3, Vector3> BulletImpactCallback { get; private set; }
    private int _enemyKills;
    protected virtual void Start()
    {
        // El pool de particulas se crea ANTES que el EnemyService porque
        // CreateShootStrategy() (que corre adentro de esa linea) lo necesita.
        if (_impactEffectPrefab != null)
        {
            ImpactEffectService impactEffectService = new ImpactEffectService(_impactEffectPrefab, GameManager.instance.projectilesParent, _impactEffectPoolSize);
            BulletImpactCallback = impactEffectService.PlayAt;
        }
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
