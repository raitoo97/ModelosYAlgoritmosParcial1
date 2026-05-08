using System;
using UnityEngine;
public enum BulletOwner
{
    Player,
    Enemy
}
public class Bullet : MonoBehaviour
{
    private float _currentDistance;
    private float _damageMultiplier = 1f;
    private Renderer _renderer;
    private Action<Bullet> _returnToPoolCallBack;
    private BulletOwner _owner;
    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
    }
    void Update()
    {
        float distanceToTravel = FlyWeightPointer.Projectile.speed * Time.deltaTime;
        transform.position += transform.forward * distanceToTravel;
        _currentDistance += distanceToTravel;
        if(_currentDistance >= FlyWeightPointer.Projectile.maxLife)
            _returnToPoolCallBack?.Invoke(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IDamageable>(out var entity))
        {
            if (ShouldHit(other))
            {
                entity.TakeDamage(FlyWeightPointer.Projectile.damage * _damageMultiplier);
            }
            _returnToPoolCallBack?.Invoke(this);
        }
    }
    private bool ShouldHit(Collider other)
    {
        if (_owner == BulletOwner.Player && other.CompareTag("Player"))
            return false;
        if (_owner == BulletOwner.Enemy && other.CompareTag("Enemy"))
            return false;
        return true;
    }
    public void SetReturnToPoolCallBack(Action<Bullet> returnToPoolCallBack)
    {
        _returnToPoolCallBack = returnToPoolCallBack;
    }
    public void ResetBullet()
    {
        _damageMultiplier = 1f;
        _currentDistance = 0;
    }
    public void SetDamageMultiplier(float damage)
    {
        _damageMultiplier = damage;
    }
    public void SetOwner(BulletOwner owner)
    {
        _owner = owner;
    }
    public void SetColor(Color color)
    {
        if (_renderer != null)
            _renderer.material.SetColor("_BulletColor", color);
    }
}