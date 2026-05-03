using System;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour , IDamageable
{
    private FSM _fsm;
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    [SerializeField] private Bullet _bulletPrefab;
    private BulletService _bulletService;
    [SerializeField]private Transform _gunSight;
    private int _initPoolSize = 50;
    private Action<Enemy> _returnToPoolCallBack;
    private float _currentLife;
    private bool _isDead;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _isDead = false;
        _fsm = new FSM();
        _bulletService = new BulletService(_bulletPrefab, GameManager.instance._projectilesParent, _initPoolSize);
        Transform playerTransform = GameManager.instance.player.transform;
        _fsm.AddState(FSM.StateID.Chase, new ChaseState(transform, _agent, this, _fsm, playerTransform));
        _fsm.AddState(FSM.StateID.Attack, new AttackState(transform, _agent, this, _fsm, playerTransform));
        _fsm.ChangeState(FSM.StateID.Chase);
    }
    public void ResetEnemy()
    {
        _currentLife = FlyWeightPointer.Entity.maxLife;
        _isDead = false;
        if(_agent.hasPath)
            _agent.ResetPath();
        _fsm.ChangeState(FSM.StateID.Chase);
    }
    void Update()
    {
        if (!enabled) return;
        _fsm.onUpdateState();
    }
    public void Rotate(Vector3 direction)
    {
        Vector3 _dirRot = new Vector3(direction.x, 0, direction.z).normalized;
        if (_dirRot.sqrMagnitude > 0.001f)
        {
            Quaternion _rotDir = Quaternion.LookRotation(_dirRot);
            _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, _rotDir, FlyWeightPointer.Entity.rotateSpeed * Time.deltaTime));
        }
    }
    public void SetReturnToPoolCallBack(Action<Enemy> returnToPoolCallBack)
    {
        _returnToPoolCallBack = returnToPoolCallBack;
    }
    public void Shoot()
    {
        Bullet bullet = _bulletService.Shoot(_gunSight.position, _gunSight.rotation);
        new BulletBuilder(bullet)
            .SetSpeed(FlyWeightPointer.Projectile.speed)
            .SetDamage(FlyWeightPointer.Projectile.damage)
            .SetColorMaterial(Color.red)
            .SetOwnerBullet(BulletOwner.Enemy)
            .Build();
    }
    public void Die()
    {
        _returnToPoolCallBack?.Invoke(this);
    }
    public void TakeDamage(float dmg)
    {
        if (_isDead) return;
        _currentLife -= dmg;
        if (_currentLife <= 0) 
        {
            _isDead = true;
            EventManager.TriggerEvent(EventType.EnemyKilled, 1);
            Die();
        }
    }
    public void WarpToPosition(Vector3 position)
    {
        _agent.Warp(position);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, FlyWeightPointer.Entity.maxDistance);
    }
}