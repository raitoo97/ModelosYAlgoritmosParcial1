using System;
using System.Collections.Generic;
using UnityEngine;
public class SniperSpawner : MonoBehaviour, IObserver<EnemyEvent>, IPauseable
{
    [SerializeField] private Enemy _sniperPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private float _respawnTime;
    private EnemyService _sniperService;
    private Enemy[] _activeSnipers;
    private float[] _respawnTimers;
    private Dictionary<EnemyEvent, Action> _actions = new Dictionary<EnemyEvent, Action>();
    private void Start()
    {
        IShootStrategy sniperStrategy = new HitscanShootStrategy(
            FlyWeightPointer.Sniper.damage, Factions.Enemy, _hitMask, FlyWeightPointer.Sniper.maxDistance);
        _sniperService = new EnemyService(_sniperPrefab, transform, _spawnPoints.Length, this, sniperStrategy);
        SaveManager.instance.Subscribe(_sniperService);
        FillDictionary();
        _activeSnipers = new Enemy[_spawnPoints.Length];
        _respawnTimers = new float[_spawnPoints.Length];
        for (int i = 0; i < _spawnPoints.Length; i++)
            _activeSnipers[i] = _sniperService.Spawn(_spawnPoints[i].position);
    }
    private void Update()
    {
        // Puesto vacio (el sniper murio y volvio al pool) -> corre el timer y respawnea ahi mismo
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            if (_activeSnipers[i].gameObject.activeSelf) continue;
            _respawnTimers[i] += Time.deltaTime;
            if (_respawnTimers[i] >= _respawnTime)
            {
                _respawnTimers[i] = 0;
                _activeSnipers[i] = _sniperService.Spawn(_spawnPoints[i].position);
            }
        }
    }
    private void FillDictionary()
    {
        _actions.Add(EnemyEvent.EnemyDie, OnSniperKilled);
    }
    private void OnSniperKilled()
    {
        EventManager.TriggerEvent(EventType.EnemyKilled, 1); // cuenta para la UI, no para la dificultad del ranger
    }
    public void Notify(EnemyEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
            _actions[Actions].Invoke();
    }
    public void Pause() { enabled = false; }
    public void Resume() { enabled = true; }
    private void OnDestroy()
    {
        SaveManager.instance.Unsubscribe(_sniperService);
    }
}
