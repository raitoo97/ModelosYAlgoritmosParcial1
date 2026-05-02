using System.Collections.Generic;
using UnityEngine;
public class PlayerModel : IObservable<PlayerEvent>
{
    private Rigidbody _rb;
    private float _currentLife;
    private List<IObserver<PlayerEvent>> _myobservers;
    private bool _isDead;
    public PlayerModel(Player user)
    {
        _rb = user.GetComponent<Rigidbody>();
        _isDead = false;
        _currentLife = FlyWeightPointer.Entity.maxLife;
        _myobservers = new List<IObserver<PlayerEvent>>();
    }
    public void Move(Vector3 direction)
    {
        bool isMoving = direction.sqrMagnitude > 0.001f;
        direction.Normalize();
        _rb.MovePosition(_rb.position + direction * FlyWeightPointer.Entity.speed * Time.fixedDeltaTime);
        NotifyObservers(isMoving ? PlayerEvent.Move : PlayerEvent.Idle);
    }
    public void Rotate(Vector3 direction)
    {
        Vector3 _dirRot = new Vector3(direction.x, 0, direction.z).normalized;
        if (_dirRot.sqrMagnitude > 0.001f)
        {
            Quaternion _rotDir = Quaternion.LookRotation(_dirRot);
            _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, _rotDir, FlyWeightPointer.Entity.rotateSpeed * Time.fixedDeltaTime));
        }
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
}