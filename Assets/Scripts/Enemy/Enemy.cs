using System;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyEvent
{
    EnemyDie
}
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour , IDamageable , IPauseable
{
    private FSM _fsm;
    private NavMeshAgent _agent;
    private EnemyModel _model;
    private EnemyView _view;
    [SerializeField] private Bullet _bulletPrefab;
    private BulletService _bulletService;
    [SerializeField]private Transform _gunSight;
    private int _initPoolSize = 50;
    private Action<Enemy> _returnToPoolCallBack;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _model = new EnemyModel(this);
        _view = new EnemyView(this);
        _model.Subscribe(_view);
        _fsm = new FSM();
        _bulletService = new BulletService(_bulletPrefab, GameManager.instance._projectilesParent, _initPoolSize);
        Transform playerTransform = GameManager.instance.player.transform;
        _fsm.AddState(FSM.StateID.Chase, new ChaseState(transform, _agent, this, _fsm, playerTransform));
        _fsm.AddState(FSM.StateID.Attack, new AttackState(transform, _agent, this, _fsm, playerTransform));
        _fsm.ChangeState(FSM.StateID.Chase);
    }
    public void ResetEnemy()
    {
        _model.ResetLife();
        if (_agent.hasPath)
            _agent.ResetPath();
        _fsm.ChangeState(FSM.StateID.Chase);
    }
    void Update()
    {
        _fsm.onUpdateState();
    }
    public void Rotate(Vector3 direction)
    {
        _model.Rotate(direction);
    }
    public void SetReturnToPoolCallBack(Action<Enemy> returnToPoolCallBack)
    {
        _returnToPoolCallBack = returnToPoolCallBack;
    }
    public void Shoot()
    {
        Bullet bullet = _bulletService.Shoot(_gunSight.position, _gunSight.rotation);
        new BulletBuilder(bullet)
            .SetColorMaterial(Color.red)
            .SetOwnerBullet(BulletOwner.Enemy)
            .Build();
    }
    public void TakeDamage(float dmg)
    {
        _model.TakeDamage(dmg);
        if (_model.IsDead)
            _returnToPoolCallBack?.Invoke(this);
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
    public void Subscribe(IObserver<EnemyEvent> observer)
    {
        _model.Subscribe(observer);
    }
    public EnemyMemento CaptureState()
    {
        return new EnemyMemento
        {
            life = _model.CurrentLife,
            isDead = _model.IsDead,
            isActive = gameObject.activeSelf,
            position = transform.position,
            rotation = transform.rotation,
            currentState = _fsm.currentStateID,
        };
    }
    public void LoadState(EnemyMemento memory)
    {
        gameObject.SetActive(memory.isActive);
        if (!memory.isActive) return;
        _model.RestoreLife(memory.life, memory.isDead);
        _agent.Warp(memory.position);
        transform.rotation = memory.rotation;
        _fsm.ChangeState(memory.currentState);
    }
    public void Pause()
    {
        enabled = false;
        _agent.enabled = false;
    }
    public void Resume()
    {
        enabled = true;
        _agent.enabled = true;
    }
    private void OnDestroy()
    {
        _model.Unsubscribe(_view);
    }
}