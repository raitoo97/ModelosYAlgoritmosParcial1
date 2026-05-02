using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerView : IObserver<PlayerEvent>
{
    private Animator _animator;
    private Dictionary<PlayerEvent, Action> _actions;
    public PlayerView(Player user)
    {
        _animator = user.GetComponent<Animator>();
        EventManager.SubscribeToEvent(EventType.PlayerDeath, OnPlayerDeath);
        FillDictionary();
    }
    public void MoveAnimation(bool isRunning)
    {
        _animator.SetBool("IsRunning", isRunning);
    }
    public void OnPlayerDeath(params object[] parameters)
    {
        _animator.SetTrigger("OnDeath");
    }
    private void FillDictionary()
    {
        _actions = new Dictionary<PlayerEvent, Action>();
        _actions.Add(PlayerEvent.Move, () => MoveAnimation(true));
        _actions.Add(PlayerEvent.Idle, () => MoveAnimation(false));
    }
    public void Notify(PlayerEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
        {
            _actions[Actions].Invoke();
        }
    }
}
