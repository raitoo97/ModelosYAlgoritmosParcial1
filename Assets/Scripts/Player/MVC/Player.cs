using System.Collections;
using UnityEngine;
public enum PlayerEvent
{
    Move,
    Idle,
    Shoot,
    Death,
}
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour , IDamageable ,IPauseable
{
    private PlayerModel _model;
    private IController _controler;
    private PlayerView _view;
    private Gun _gun;
    private Vector3 _velocity;
    private void Awake()
    {
        _model = new PlayerModel(this);
        _controler = new PlayerController(_model);
        _view = new PlayerView(this);
        _model.Subscribe(_view);
        _gun = GetComponentInChildren<Gun>();
        if (_gun != null) _model.Subscribe(_gun);
        EventManager.SubscribeToEvent(EventType.PlayerDeath, OnDeath);
        StartCoroutine(LateAwake());
    }
    private void Update()
    {
        if (GameState.IsPaused) return;
        _controler.UpdateInputs();
    }
    private void FixedUpdate()
    {
        if (GameState.IsPaused) return;
        _controler.FixedUpdateInputs();
    }
    private void OnDeath(params object[] parameters)
    {
        this.enabled = false;
    }
    public void TakeDamage(float dmg)
    {
        _model.TakeDamage(dmg);
    }
    IEnumerator LateAwake()
    {
        yield return null;
        SaveManager.instance.Subscribe(_model);
    }
    private void OnDisable()
    {
        _model.Unsubscribe(_view);
        _model.Unsubscribe(_gun);
        EventManager.UnsubscribeToEvent(EventType.PlayerDeath, OnDeath);
        SaveManager.instance.Unsubscribe(_model);
    }
    public void Pause()
    {
        _model.Pause();
        _view.GetAnimator.speed = 0;
        _velocity = _model.GetRb.linearVelocity;
        _model.GetRb.linearVelocity = Vector3.zero;
        _model.GetRb.useGravity = false;
    }
    public void Resume()
    {
        _view.GetAnimator.speed = 1;
        _model.GetRb.linearVelocity = _velocity;
        _model.GetRb.useGravity = true;
    }
}