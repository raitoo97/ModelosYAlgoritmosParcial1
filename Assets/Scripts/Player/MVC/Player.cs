using System.Collections;
using UnityEngine;
public enum PlayerEvent
{
    Move,
    Idle,
    Shoot,
    Death,
    Aim,
    StopAim,
    NextShootType,
    PreviousShootType,
    UsePowerUp,
    ShieldOn,
    ShieldOff
}
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour , IDamageable ,IPauseable , IFactionMember,IAimPointProvider, IShieldable, ISpeedBoostable, IHealable
{
    private PlayerModel _model;
    private ICharacterController _controller;
    private PlayerView _view;
    private Gun _gun;
    [SerializeField] private Transform _cameraReference;
    [SerializeField] private LayerMask _aimMask;
    private PowerUpController _powerUp;
    [SerializeField] private GameObject _shieldVisual;//es un gameObject porque necesito desactivar la visual en view
    public Factions Faction => Factions.Player;
    private void Awake()
    {
        _model = new PlayerModel(this, _cameraReference, _aimMask);
        _controller = new PlayerController(_model);
        _view = new PlayerView(this, _shieldVisual);
        _model.Subscribe(_view);
        _gun = GetComponentInChildren<Gun>();
        if (_gun != null)
        {
            _gun.Init(this);
            _model.Subscribe(_gun);
        }
        // busco el componente PowerUpController del player
        // lo incializo pasandole de user al player mismo
        //ademas suscribo al PowerUpController a los eventos de player a traves del model
        _powerUp = GetComponent<PowerUpController>();
        if (_powerUp != null)
        {
            _powerUp.Init(this.gameObject);
            _model.Subscribe(_powerUp);
        }
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
    //metodo del IShieldable para activar y desactivar el escudo
    public void ActivateShield()
    {
        _model.SetInvulnerable(true);
    }
    public void DeactivateShield()
    {
        _model.SetInvulnerable(false);
    }
    public void ApplySpeedBoost(float multiplier)
    {
        _model.ApplySpeedBoost(multiplier);
    }
    public void RemoveSpeedBoost()
    {
        _model.RemoveSpeedBoost();
    }
    public void Heal(float amount)
    {
        _model.Heal(amount);
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
        if (_gun != null) _model.Unsubscribe(_gun);
        if (_powerUp != null) _model.Unsubscribe(_powerUp);
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