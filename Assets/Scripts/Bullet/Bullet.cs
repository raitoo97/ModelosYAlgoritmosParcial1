using System;
using UnityEngine;
public class Bullet : MonoBehaviour
{
    private float _currentDistance;
    private float _speed;
    private float _damage;
    private Renderer _renderer;
    private Action<Bullet> _returnToPoolCallBack;
    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
    }
    void Update()
    {
        float distanceToTravel = _speed * Time.deltaTime;
        transform.position += transform.forward * distanceToTravel;
        _currentDistance += distanceToTravel;
        if(_currentDistance >= FlyWeightPointer.Projectile.maxLife)
            _returnToPoolCallBack?.Invoke(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IDamageable>(out var entity))
        {
            entity.TakeDamage(_damage);
            _returnToPoolCallBack?.Invoke(this);
        }
    }
    public void SetReturnToPoolCallBack(Action<Bullet> returnToPoolCallBack)
    {
        _returnToPoolCallBack = returnToPoolCallBack;
    }
    public void ResetBullet()
    {
        _currentDistance = 0;
    }
    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
    public void SetDamage(float damage)
    {
        _damage = damage;
    }
    public void SetColor(Color color)
    {
        if (_renderer != null)
            _renderer.material.SetColor("_BulletColor", color);
    }
}
