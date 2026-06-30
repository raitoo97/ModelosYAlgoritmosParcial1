using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerModel : IObservable<PlayerEvent> , IObserver<SaveEvent> , IMementoEntity<PlayerMemento>
{
    private Rigidbody _rb;
    private float _currentLife;
    private List<IObserver<PlayerEvent>> _myobservers;
    private bool _isDead;
    private Dictionary<SaveEvent, Action> _actions;
    private MementoState<PlayerMemento> _playerMemento;
    private Transform _cameraReference;
    public PlayerModel(Player user, Transform cameraReference)
    {
        _rb = user.GetComponent<Rigidbody>();
        _isDead = false;
        _currentLife = FlyWeightPointer.Entity.maxLife;
        _myobservers = new List<IObserver<PlayerEvent>>();
        _playerMemento = new MementoState<PlayerMemento>();
        _cameraReference = cameraReference;
        FillDictionary();
    }
    public void Move(Vector3 direction)
    {
        bool isMoving = direction.sqrMagnitude > 0.001f;
        if (isMoving)
        {
            float targetRotation = GetTargetRotation(direction);
            Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;
            Vector3 moveVelocity = targetDirection * FlyWeightPointer.Entity.speed;
            _rb.MovePosition(_rb.position + moveVelocity * Time.fixedDeltaTime);
        }
        NotifyObservers(isMoving ? PlayerEvent.Move : PlayerEvent.Idle);
    }
    public void Rotate(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
        {
            float targetRotation = GetTargetRotation(direction);
            Quaternion finalRotation = Quaternion.Euler(0, targetRotation, 0);
            _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, finalRotation, 200f * Time.fixedDeltaTime));
        }
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
    public void TakeDamage(float dmg)
    {
        if (_isDead) return;
        _currentLife -= dmg;
        float normalizedLife = _currentLife / FlyWeightPointer.Entity.maxLife;
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
        for (int i = _myobservers.Count - 1; i >= 0; i--)
        {
            _myobservers[i].Notify(action);
        }
    }
    public void Subscribe(IObserver<PlayerEvent> observer)
    {
        if (!_myobservers.Contains(observer))
        {
            _myobservers.Add(observer);
        }
    }
    public void Unsubscribe(IObserver<PlayerEvent> observer)
    {
        if (_myobservers.Contains(observer))
        {
            _myobservers.Remove(observer);
        }
    }
    public void Notify(SaveEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
        {
            _actions[Actions].Invoke();
        }
    }
    private void FillDictionary()
    {
        _actions = new Dictionary<SaveEvent, Action>();
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
        float normalizedLife = _currentLife / FlyWeightPointer.Entity.maxLife;
        EventManager.TriggerEvent(EventType.PlayerDamage, normalizedLife);
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
        NotifyObservers(PlayerEvent.Idle);
    }
    public Rigidbody GetRb => _rb;
}