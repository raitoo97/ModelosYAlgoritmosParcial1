using System;
using System.Collections.Generic;
using UnityEngine;
public class EnemyView : IObserver<EnemyEvent>
{
    private Animator _animator;//En este momento no tiene animaciones pero lo dejo preparado para le final
    private Dictionary<EnemyEvent, Action> _actions;
    public EnemyView(Enemy user)
    {
        _animator = user.GetComponent<Animator>();
        _actions = new Dictionary<EnemyEvent, Action>();
        FillDictionary();
    }
    private void FillDictionary()
    {
        _actions.Add(EnemyEvent.EnemyDie, OnEnemyDeath);
    }
    private void OnEnemyDeath()
    {
        if (_animator == null) return;
        _animator.SetTrigger("OnDeath");
    }
    public void Notify(EnemyEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
            _actions[Actions].Invoke();
    }
}
