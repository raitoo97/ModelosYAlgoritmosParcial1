using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CrosshairUI : MonoBehaviour, IObserver<PlayerEvent>
{
    [SerializeField] private Image _crosshairImage;
    private Dictionary<PlayerEvent, Action> _actions = new Dictionary<PlayerEvent, Action>();
    private void Start()
    {
        GameManager.instance.player.SubscribeObserver(this);
        FillDictionary();
        SetVisible(false);
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.Aim, () => SetVisible(true));
        _actions.Add(PlayerEvent.StopAim, () => SetVisible(false));
    }
    public void Notify(PlayerEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
            _actions[Actions].Invoke();
    }
    private void SetVisible(bool visible)
    {
        _crosshairImage.enabled = visible;
    }
    private void OnDestroy()
    {
        GameManager.instance.player.UnsubscribeObserver(this);
    }
}
