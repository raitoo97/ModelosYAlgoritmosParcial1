using System.Collections;
using UnityEngine;
public enum PlayerEvent
{
    Move,
    Idle,
    Shoot,
    Death,
    Aim,
    StopAim
}
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour , IDamageable ,IPauseable , IFactionMember
{
    private PlayerModel _model;
    private ICharacterController _controller;
    private PlayerView _view;
    private Gun _gun;
    [SerializeField] private Transform _cameraReference;
    public Factions Faction => Factions.Player;
    private void Awake()
    {
        _model = new PlayerModel(this, _cameraReference);
        _controller = new PlayerController(_model);
        _view = new PlayerView(this);
        _model.Subscribe(_view);
        _gun = GetComponentInChildren<Gun>();
        if (_gun != null) _model.Subscribe(_gun);
        EventManager.SubscribeToEvent(EventType.PlayerDeath, OnDeath);
        StartCoroutine(LateAwake());
    }
    private void Update()
    {
        _controller.UpdateInputs();
    }
    private void FixedUpdate()
    {
        _controller.FixedUpdateInputs();
    }
    private void OnDeath(params object[] parameters)
    {
        this.enabled = false;
    }
    public void TakeDamage(float dmg)
    {
        _model.TakeDamage(dmg);
    }
    private void OnAnimatorIK(int layerIndex)
    {
        _view.UpdateAimIK(_model.GetAiming, GetAimPoint());
    }
    public Vector3 GetAimPoint()
    {
        return _model.GetAimPoint();
    }
    IEnumerator LateAwake()
    {
        yield return null;
        SaveManager.instance.Subscribe(_model);
    }
    private void OnDestroy()
    {
        _model.Unsubscribe(_view);
        _model.Unsubscribe(_gun);
        EventManager.UnsubscribeToEvent(EventType.PlayerDeath, OnDeath);
        SaveManager.instance.Unsubscribe(_model);
    }
    public void SubscribeObserver(IObserver<PlayerEvent> observer)
    {
        _model.Subscribe(observer);
    }
    public void UnsubscribeObserver(IObserver<PlayerEvent> observer)
    {
        _model.Unsubscribe(observer);
    }
    public void Pause()
    {
        _model.Pause();
        _model.PausePhysics();
        _view.GetAnimator.speed = 0;
        enabled = false;
    }
    public void Resume()
    {
         enabled = true;
        _model.ResumePhysics();
        _view.GetAnimator.speed = 1;
    }
}