using UnityEngine;
public class EnemyModel : IObservable<EnemyEvent>
{
    private Rigidbody _rb;
    private float _currentLife;
    private bool _isDead;
    private ObserverList<EnemyEvent> _enemyObservers;
    private IShootStrategy _shootStrategy;
    private Transform _playerTransform;
    private FlyWeight _stats;
    public EnemyModel(Rigidbody rb, Transform playerTransform, FlyWeight stats)
    {
        _enemyObservers = new ObserverList<EnemyEvent>();
        _rb = rb;
        _playerTransform = playerTransform;
        _stats = stats;
    }
    public void ResetLife()
    {
        _currentLife = _stats.maxLife;
        _isDead = false;
    }
    public void SetShootStrategy(IShootStrategy strategy)
    {
        _shootStrategy = strategy;
    }
    public ShotResult Shoot(Vector3 origin)
    {
        Vector3 dir = (GetAimPoint() - origin).normalized;
        return _shootStrategy?.Shoot(origin, dir) ?? default;
    }
    public void TakeDamage(float dmg)
    {
        if (_isDead) return;
        _currentLife -= dmg;
        if (_currentLife <= 0)
        {
            _currentLife = 0;
            _isDead = true;
            NotifyObservers(EnemyEvent.EnemyDie);
        }
    }
    public void Rotate(Vector3 direction)
    {
        Vector3 dirRot = new Vector3(direction.x, 0, direction.z).normalized;
        if (dirRot.sqrMagnitude <= 0.001f) return;
        Quaternion rotDir = Quaternion.LookRotation(dirRot);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, rotDir, _stats.rotateSpeed * Time.deltaTime));
    }
    public void RestoreLife(float life, bool isDead)
    {
        _currentLife = life;
        _isDead = isDead;
    }
    public void Subscribe(IObserver<EnemyEvent> observer) 
    {
        _enemyObservers.Subscribe(observer);
    }
    public void Unsubscribe(IObserver<EnemyEvent> observer) 
    {
        _enemyObservers.Unsubscribe(observer);
    }
    public void NotifyObservers(EnemyEvent action) 
    {
        _enemyObservers.NotifyObservers(action);
    }
    public Vector3 GetAimPoint()
    {
        return _playerTransform.position + Vector3.up * 1.5f;
    }
    public bool IsDead => _isDead;
    public float CurrentLife => _currentLife;
}
