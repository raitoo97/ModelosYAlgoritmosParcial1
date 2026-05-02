using UnityEngine;
public enum PlayerEvent
{
    Move,
    Idle,
    Shoot,
    Death
}
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private PlayerModel _model;
    private IController _controler;
    private PlayerView _view;
    private Gun _gun;
    private void Awake()
    {
        _model = new PlayerModel(this);
        _controler = new PlayerController(_model);
        _view = new PlayerView(this);
        _model.Subscribe(_view);
        _gun = GetComponentInChildren<Gun>();
        if (_gun != null) _model.Subscribe(_gun);
        EventManager.SubscribeToEvent(EventType.PlayerDeath, OnDeath);
    }
    private void Update()
    {
        _controler.UpdateInputs();
    }
    private void FixedUpdate()
    {
        _controler.FixedUpdateInputs();
    }
    private void OnDeath(params object[] parameters)
    {
        this.enabled = false;
    }
    private void OnDisable()
    {
        _model.Unsubscribe(_view);
        _model.Unsubscribe(_gun);
        EventManager.UnsubscribeToEvent(EventType.PlayerDeath, OnDeath);
    }
}
