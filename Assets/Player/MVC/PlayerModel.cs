using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerModel : IObservable<PlayerEvent>
{
    private Rigidbody _rb;
    private float _currentLife;
    private List<IObserver> _myobservers;
    public event Action<bool> onMovement;
    public event Action OnDeath;
    public PlayerModel(Player user)
    {
        _rb = user.GetComponent<Rigidbody>();
        _currentLife = FlyWeightPointer.flyWeight.maxLife;
        _myobservers = new List<IObserver>();
    }
    public void Move(Vector3 direction)
    {
        bool isMoving = direction.magnitude > 0.001f;
        direction.Normalize();
        _rb.MovePosition(_rb.position + direction * FlyWeightPointer.flyWeight.speed * Time.fixedDeltaTime);
        onMovement?.Invoke(isMoving);
    }
    public void Rotate(Vector3 direction)
    {
        Vector3 _dirRot = new Vector3(direction.x, 0, direction.z).normalized;
        if (_dirRot.magnitude > 0.001f)
        {
            Quaternion _rotDir = Quaternion.LookRotation(_dirRot);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, _rotDir, FlyWeightPointer.flyWeight.rotateSpeed * Time.fixedDeltaTime));
        }
    }
    public void Shoot()
    {
        Debug.Log("Shoot");
    }
    public void TakeDamage(float dmg)
    {
        _currentLife -= dmg;
        if (_currentLife <= 0)
        {
            _currentLife = 0;
            OnDeath?.Invoke();
        }
    }
    public void Subscribe(IObserver observer)
    {
        if (!_myobservers.Contains(observer))
            _myobservers.Add(observer);
    }

    public void Unsubscribe(IObserver observer)
    {
        throw new NotImplementedException();
    }

    public void NotifyObservers(PlayerEvent action)
    {
        throw new NotImplementedException();
    }
}
