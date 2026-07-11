using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerModel : IObservable<PlayerEvent> , IObserver<SaveEvent> , IMementoEntity<PlayerMemento>
{
    private Rigidbody _rb;
    private float _currentLife;
    private ObserverList<PlayerEvent> _playerObservers = new ObserverList<PlayerEvent>();
    private bool _isDead;
    private bool _isMoving;
    private bool _isInvulnerable;
    private Dictionary<SaveEvent, Action> _actions = new Dictionary<SaveEvent, Action>();
    private MementoState<PlayerMemento> _playerMemento;
    private Vector3 _pausedVelocity;
    private Transform _cameraReference;
    private bool _isAiming;
    private Dictionary<bool, IRotationStrategy> _rotationStrategies;
    private IRotationStrategy _currentRotation;
    private LayerMask _aimMask;
    private float _speedMultiplier = 1f;
    public PlayerModel(Player user, Transform cameraReference, LayerMask aimMask)
    {
        _rb = user.GetComponent<Rigidbody>();
        _isDead = false;
        _currentLife = FlyWeightPointer.Player.maxLife;
        _playerMemento = new MementoState<PlayerMemento>();
        _aimMask = aimMask;
        _cameraReference = cameraReference;
        FillRotationStrategies();
        FillDictionary();
    }
    private void FillRotationStrategies()
    {
        _rotationStrategies = new Dictionary<bool, IRotationStrategy>
        {
            { false, new MovementRotationStrategy(_rb, _cameraReference) },
            { true,  new AimRotationStrategy(_rb, _cameraReference) }
        };
        _currentRotation = _rotationStrategies[false];
    }
    public void Move(Vector3 direction)
    {
        bool isMoving = direction.sqrMagnitude > 0.001f;
        if (isMoving)
        {
            float targetRotation = GetTargetRotation(direction);
            Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;
            Vector3 moveVelocity = targetDirection * (FlyWeightPointer.Player.speed * _speedMultiplier);
            _rb.MovePosition(_rb.position + moveVelocity * Time.fixedDeltaTime);
        }
        SetMoving(isMoving);
    }
    private void SetMoving(bool isMoving)
    {
        if (isMoving == _isMoving) return;
        _isMoving = isMoving;
        NotifyObservers(isMoving ? PlayerEvent.Move : PlayerEvent.Idle);
    }
    public void Rotate(Vector3 direction)
    {
        _currentRotation.Rotate(direction);
    }
    private float GetTargetRotation(Vector3 inputDir)
    {
        Vector3 input = new Vector3(inputDir.x, 0, inputDir.z);
        return Quaternion.LookRotation(input).eulerAngles.y + _cameraReference.eulerAngles.y;
    }
    public void Shoot()
    {
        NotifyObservers(PlayerEvent.Shoot);
    }
    public void UsePowerUp()
    {
        NotifyObservers(PlayerEvent.UsePowerUp);
    }
    public void NextShootType()
    {
        NotifyObservers(PlayerEvent.NextShootType);
    }
    public void CyclePowerUp()
    {
        NotifyObservers(PlayerEvent.CyclePowerUp);
    }
    public void PreviousShootType()
    {
        NotifyObservers(PlayerEvent.PreviousShootType);
    }
    public void TakeDamage(float dmg)
    {
        if (_isDead || _isInvulnerable) return;
        _currentLife -= dmg;
        float normalizedLife = _currentLife / FlyWeightPointer.Player.maxLife;
        EventManager.TriggerEvent(EventType.PlayerDamage, normalizedLife);
        if (_currentLife <= 0)
        {
            _currentLife = 0;
            _isDead = true;
            NotifyObservers(PlayerEvent.Death);
            EventManager.TriggerEvent(EventType.PlayerDeath);
        }
    }
    public void NotifyObservers(PlayerEvent action) 
    { 
        _playerObservers.NotifyObservers(action); 
    }
    public void Subscribe(IObserver<PlayerEvent> observer) 
    { 
        _playerObservers.Subscribe(observer); 
    }
    public void Unsubscribe(IObserver<PlayerEvent> observer) 
    {
        _playerObservers.Unsubscribe(observer);
    }
    public void Notify(SaveEvent action)
    {
        if (_actions.ContainsKey(action))
        {
            _actions[action].Invoke();
        }
    }
    private void FillDictionary()
    {
        _actions.Add(SaveEvent.Save, SaveState);
        _actions.Add(SaveEvent.Load, TryLoadStates);
    }
    public void SaveState()
    {
        _playerMemento.SaveMemory(
            new PlayerMemento
            {
                position = _rb.position,
                rotation = _rb.rotation,
                life = _currentLife,
                isDead = _isDead,
            });
    }
    public void LoadState(PlayerMemento memory)
    {
        _rb.position = memory.position;
        _rb.rotation = memory.rotation;
        _currentLife = memory.life;
        _isDead = memory.isDead;
        float normalizedLife = _currentLife / FlyWeightPointer.Player.maxLife;
        EventManager.TriggerEvent(EventType.PlayerDamage, normalizedLife);
        _isMoving = false;
        NotifyObservers(_isDead ? PlayerEvent.Death : PlayerEvent.Idle);
    }
    public void TryLoadStates()
    {
        if (_playerMemento.memoriesAmount == 0) return;
        var lastMemory = _playerMemento.LoadMemory();
        LoadState(lastMemory);
    }
    public void Pause()
    {
        _isMoving = false;
        NotifyObservers(PlayerEvent.Idle);
    }
    public void PausePhysics()
    {
        _pausedVelocity = _rb.linearVelocity;
        _rb.linearVelocity = Vector3.zero;
        _rb.useGravity = false;
    }
    public void ResumePhysics()
    {
        _rb.linearVelocity = _pausedVelocity;
        _rb.useGravity = true;
    }
    public void SetAiming(bool isAiming)
    {
        if (isAiming == _isAiming) return;
        _isAiming = isAiming;
        _currentRotation = _rotationStrategies[isAiming];
        NotifyObservers(isAiming ? PlayerEvent.Aim : PlayerEvent.StopAim);
    }
    //Punto de mira: raycast desde la posicion de la camara en la direccion en que mira.
    //Del impacto solo uso el punto; si no pega en nada, devuelvo un punto lejano como fallback.
    //Es la referencia comun a la que convergen todos los sistemas de apuntado,
    //asi todo apunta a lo mismo que el jugador ve bajo el crosshair. Lo consumen:
    //  - GunModel.Shoot()        -> direccion del disparo hitscan (trayectoria, no rotacion).
    //  - Gun.ComputeLaser()      -> endpoint del laser (tampoco es rotacion).
    //  - Gun.LateUpdate()        -> target de la rotacion del arma (esto si es rotacion).
    //  - Player.OnAnimatorIK()   -> UpdateAimIK, el LookAt del torso/cabeza (rotacion via IK).
    public Vector3 GetAimPoint()
    {
        float aimDistance = FlyWeightPointer.Player.maxDistance;
        Ray ray = new Ray(_cameraReference.position, _cameraReference.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, aimDistance, _aimMask, QueryTriggerInteraction.Ignore))
            return hit.point;
        return _cameraReference.position + _cameraReference.forward * aimDistance;
    }
    public bool GetAiming => _isAiming;
    public bool IsDead => _isDead;
    #region PowerUps
    /// <summary>
    /// Notifico a los observadores de los eventos ShieldOn y ShieldOff dependiendo si el usuario es invunerable o no
    /// </summary>
    /// <param name="value"></param>
    public void SetInvulnerable(bool value)
    {
        if (value == _isInvulnerable) return;
        _isInvulnerable = value;
        NotifyObservers(value ? PlayerEvent.ShieldOn : PlayerEvent.ShieldOff);
    }
    public void ApplySpeedBoost(float multiplier)
    {
        _speedMultiplier = multiplier;
    }
    public void RemoveSpeedBoost()
    {
        _speedMultiplier = 1f;
    }
    //Cura clampeando al maximo de vida. Reuso el evento PlayerDamage:
    //es el que actualiza la barra con la vida normalizada
    //(el nombre quedo de cuando la vida solo podia bajar).
    public void Heal(float amount)
    {
        if (_isDead) return;
        _currentLife = Mathf.Min(_currentLife + amount, FlyWeightPointer.Player.maxLife);
        float normalizedLife = _currentLife / FlyWeightPointer.Player.maxLife;
        EventManager.TriggerEvent(EventType.PlayerDamage, normalizedLife);
    }
    #endregion
}