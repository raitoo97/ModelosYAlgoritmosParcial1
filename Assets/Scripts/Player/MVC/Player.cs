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
    private bool _isPaused;
    private void Awake()
    {
        _model = new PlayerModel(this);
        _controler = new PlayerController(_model);
        _view = new PlayerView(this);
        _model.Subscribe(_view);
        _gun = GetComponentInChildren<Gun>();
        if (_gun != null) _model.Subscribe(_gun);
        EventManager.SubscribeToEvent(EventType.PlayerDeath, OnDeath);
        _isPaused = false;
        StartCoroutine(LateAwake());
    }
    private void Update()
    {
        if (_isPaused) return;
        _controler.UpdateInputs();
    }
    private void FixedUpdate()
    {
        if (_isPaused) return;
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
        _isPaused = true;
        _model.Pause();
        _view.GetAnimator.speed = 0;
        _velocity = _model.GetRb.linearVelocity;
        _model.GetRb.linearVelocity = Vector3.zero;
    }
    public void Resume()
    {
        _isPaused = false;
        _view.GetAnimator.speed = 1;
        _model.GetRb.linearVelocity = _velocity;
    }
}