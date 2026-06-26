using System;
using System.Collections.Generic;
using UnityEngine;
public class EnemyService : IMementoEntity<List<EnemyMemento>>, IObserver<SaveEvent>
{
    private Pool<Enemy> _pool;
    private EnemyFactory _factory;
    private IObserver<EnemyEvent> _observer;
    private List<Enemy> _allEnemies = new List<Enemy>();
    private MementoState<List<EnemyMemento>> _enemiesMemento;
    private Dictionary<SaveEvent, Action> _actions;
    public EnemyService(Enemy prefab, Transform parent, int size , IObserver<EnemyEvent> observer)
    {
        _factory = new EnemyFactory(prefab, parent);
        _observer = observer;
        _enemiesMemento = new MementoState<List<EnemyMemento>>();
        FillDictionary();
        _pool = new Pool<Enemy>(CreateEnemy, TurnOn, TurnOff, size);
    }
    private Enemy CreateEnemy()
    {
        Enemy enemy = _factory.CreateObject();
        enemy.SetReturnToPoolCallBack(ReturnToPool);
        enemy.Subscribe(_observer);
        _allEnemies.Add(enemy);
        return enemy;
    }
    public Enemy Spawn(Vector3 position)
    {
        Enemy enemy = _pool.GetObject();
        enemy.WarpToPosition(position);
        enemy.ResetEnemy();
        return enemy;
    }
    private void TurnOn(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }
    private void TurnOff(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }
    public void ReturnToPool(Enemy enemy)
    {
        _pool.ReturnObject(enemy);
    }
    private void FillDictionary()
    {
        _actions = new Dictionary<SaveEvent, Action>();
        _actions.Add(SaveEvent.Save, SaveState);
        _actions.Add(SaveEvent.Load, TryLoadStates);
    }
    public void Notify(SaveEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
        {
            _actions[Actions].Invoke();
        }
    }
    public void SaveState()
    {
        var snapshot = new List<EnemyMemento>();
        foreach (var enemy in _allEnemies)
        {
            snapshot.Add(enemy.CaptureState());
        }
        _enemiesMemento.SaveMemory(snapshot);
    }
    public void LoadState(List<EnemyMemento> memory)
    {
        foreach (var enemy in _allEnemies)
        {
            enemy.gameObject.SetActive(false);
        }
        for (int i = 0; i < memory.Count; i++)
        {
            _allEnemies[i].LoadState(memory[i]);
        }
    }
    public void TryLoadStates()
    {
        if (_enemiesMemento.memoriesAmount == 0) return;
        var lastMemory = _enemiesMemento.LoadMemory();
        LoadState(lastMemory);
    }
}
