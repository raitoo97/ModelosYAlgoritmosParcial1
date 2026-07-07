using System;
using System.Collections.Generic;
using UnityEngine;
public class EnemyView : IObserver<EnemyEvent>
{
    private Animator _animator;
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
        _actions.Add(EnemyEvent.Run, () => SetRunning(true));
        _actions.Add(EnemyEvent.Aim, () => SetRunning(false));
        _actions.Add(EnemyEvent.Reset, ResetView);
    }
    private void OnEnemyDeath()
    {
        if (_animator == null) return;
        _animator.SetTrigger("OnDeath");
    }
    private void SetRunning(bool isRunning)
    {
        if (_animator == null) return;
        _animator.SetBool("IsRunning", isRunning);
    }
    // El estado Death no tiene transiciones de salida: al reutilizar el enemigo
    // del pool hay que forzar al animator a volver al estado inicial.
    private void ResetView()
    {
        if (_animator == null) return;
        _animator.ResetTrigger("OnDeath");
        _animator.SetBool("IsRunning", false);
        _animator.Play("Aim", 0, 0f);
    }
    public void Notify(EnemyEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
            _actions[Actions].Invoke();
    }
}
