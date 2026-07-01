using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerView : IObserver<PlayerEvent>
{
    private Animator _animator;
    private Dictionary<PlayerEvent, Action> _actions;
    private float _currentAimWeight;
    public PlayerView(Player user)
    {
        _animator = user.GetComponent<Animator>();
        _actions = new Dictionary<PlayerEvent, Action>();
        FillDictionary();
    }
    public void MoveAnimation(bool isRunning)
    {
        _animator.SetBool("IsRunning", isRunning);
    }
    public void OnPlayerDeath()
    {
        _animator.SetTrigger("OnDeath");
    }
    public void UpdateAimIK(bool isAiming, Vector3 aimPoint)
    {
        float targetWeight = isAiming ? 1f : 0f;
        _currentAimWeight = Mathf.Lerp(_currentAimWeight, targetWeight, Time.deltaTime * 8f);
        _animator.SetLookAtPosition(aimPoint);
        _animator.SetLookAtWeight(
            weight: _currentAimWeight,
            bodyWeight: 0.6f,
            headWeight: 0.4f,
            eyesWeight: 0f,
            clampWeight: 0.5f
        );
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.Move, () => MoveAnimation(true));
        _actions.Add(PlayerEvent.Idle, () => MoveAnimation(false));
        _actions.Add(PlayerEvent.Death, OnPlayerDeath);
    }
    public void Notify(PlayerEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
        {
            _actions[Actions].Invoke();
        }
    }
    public Animator GetAnimator => _animator;
}