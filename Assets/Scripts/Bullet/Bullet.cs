using System;
using UnityEngine;
public enum Factions
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
    private Factions _owner;
    private bool _isActive;
    private Action<Vector3, Vector3> _onImpact;
    private Vector3 _initialScale;
    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _initialScale = transform.localScale;
    }
    void Update()
    {
        float distanceToTravel = FlyWeightPointer.Projectile.speed * Time.deltaTime;
        transform.position += transform.forward * distanceToTravel;
        _currentDistance += distanceToTravel;
        if(_currentDistance >= FlyWeightPointer.Projectile.maxLife)
            ReturnToPool();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var entity))
        {
            //si el objeto que colisiona es del mismo bando,no hace nada
            if (!FactionRules.ShouldHit(other, _owner)) return;
            entity.TakeDamage(FlyWeightPointer.Projectile.damage * _damageMultiplier);
            Impact();
        }
        // Pared u obstaculo: cualquier collider SOLIDO sin IDamageable.
        // Los triggers ajenos (powerups, zonas, otras balas) se atraviesan,
        // por eso se filtra con !other.isTrigger.
        else if (!other.isTrigger)
        {
            Impact();
        }
    }
    private void Impact()
    {
        _onImpact?.Invoke(transform.position, -transform.forward);
        ReturnToPool();
    }
    private void ReturnToPool()
    {
        if (!_isActive) return;
        _isActive = false;
        _returnToPoolCallBack?.Invoke(this);
    }
    public void SetReturnToPoolCallBack(Action<Bullet> returnToPoolCallBack)
    {
        _returnToPoolCallBack = returnToPoolCallBack;
    }
    public void ResetBullet()
    {
        _damageMultiplier = 1f;
        _currentDistance = 0;
        _isActive = true;
        _onImpact = null;
        transform.localScale = _initialScale;
    }
    public void SetDamageMultiplier(float damage)
    {
        _damageMultiplier = damage;
    }
    public void SetOwner(Factions owner)
    {
        _owner = owner;
    }
    public void SetColor(Color color)
    {
        if (_renderer != null)
            _renderer.material.SetColor("_BulletColor", color);
    }
    public void SetOnImpact(Action<Vector3, Vector3> onImpact)
    {
        _onImpact = onImpact;
    }
    public void SetScale(float multiplier)
    {
        transform.localScale = _initialScale * multiplier;
    }
}