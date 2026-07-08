using System;
using System.Collections.Generic;
using UnityEngine;
public class EnemyView : IObserver<EnemyEvent>
{
    private Animator _animator;
    private Dictionary<EnemyEvent, Action> _actions;
    private float _currentAimWeight;
    private bool _isAiming;
    public EnemyView(Enemy user)
    {
        _animator = user.GetComponent<Animator>();
        _actions = new Dictionary<EnemyEvent, Action>();
        FillDictionary();
    }
    private void FillDictionary()
    {
        _actions.Add(EnemyEvent.EnemyDie, () =>
        {
            _isAiming = false;
            OnEnemyDeath();
        });
        _actions.Add(EnemyEvent.Run, () =>
        {
            _isAiming = false;
            SetRunning(true);
        });
        _actions.Add(EnemyEvent.Aim, () =>
        {
            _isAiming = true;
            SetRunning(false);
        });
        _actions.Add(EnemyEvent.Reset, ResetView);
    }
    public void UpdateAimIK(Vector3 aimPoint)
    {
        if (_animator == null) return;
        float targetWeight = _isAiming ? 1f : 0f;
        _currentAimWeight = Mathf.Lerp(_currentAimWeight, targetWeight, Time.deltaTime * 8f);
        _animator.SetLookAtPosition(aimPoint);
        _animator.SetLookAtWeight(
            weight: _currentAimWeight,
            bodyWeight: 0.3f,
            headWeight: 0.7f,
            eyesWeight: 0f,
            clampWeight: 0.2f
        );
    }
    protected virtual void OnEnemyDeath()
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
    protected virtual void ResetView()
    {
        if (_animator == null) return;
        _animator.ResetTrigger("OnDeath");
        _animator.SetBool("IsRunning", false);
        _animator.Play("Aim", 0, 0f);
        _isAiming = false;
        _currentAimWeight = 0f;
    }
    public void Notify(EnemyEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
            _actions[Actions].Invoke();
    }
}
