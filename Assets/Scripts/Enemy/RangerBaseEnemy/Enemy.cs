using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyEvent
{
    EnemyDie,
    Run,
    Aim,
    Reset
}
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour , IDamageable , IPauseable , IFactionMember
{
    protected FSM _fsm;
    protected NavMeshAgent _agent;
    private EnemyModel _model;
    private EnemyView _view;
    [SerializeField]protected Transform _gunSight;
    [SerializeField]private float _deathAnimationDuration = 2f;
    private bool _isDying;
    private Action<Enemy> _returnToPoolCallBack;
    private Transform _playerTransform;
    protected virtual FlyWeight Stats => FlyWeightPointer.Entity;
    protected EnemyModel Model => _model;
    public Factions Faction => Factions.Enemy;
    private Rigidbody _rb;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = Stats.speed;
        _playerTransform = GameManager.instance.player.transform;
        _rb = GetComponent<Rigidbody>();
        _model = CreateModel(_rb, _playerTransform);
        _view = CreateView();
        _model.Subscribe(_view);
        _fsm = new FSM();
        _fsm.AddState(FSM.StateID.Chase, CreateChaseState(_playerTransform));
        _fsm.AddState(FSM.StateID.Attack, CreateAttackState(_playerTransform));
        _fsm.ChangeState(FSM.StateID.Chase);
    }
    protected virtual EnemyView CreateView()
    {
        return new EnemyView(this);
    }
    protected virtual EnemyModel CreateModel(Rigidbody rb, Transform playerTransform)
    {
        return new EnemyModel(rb, playerTransform, Stats);
    }
    protected virtual IState CreateAttackState(Transform playerTransform)
    {
        return new AttackState(transform, _agent, this, _fsm, playerTransform, Stats);
    }
    protected virtual IState CreateChaseState(Transform playerTransform)
    {
        return new ChaseState(transform, _agent, this, _fsm, playerTransform, Stats);
    }
    public void SetShootStrategy(IShootStrategy strategy)
    {
        _model.SetShootStrategy(strategy);
    }
    public void ResetEnemy()
    {
        _isDying = false;
        _model.ResetLife();
        _model.NotifyObservers(EnemyEvent.Reset);
        if (_agent.hasPath)
            _agent.ResetPath();
        _fsm.ChangeState(FSM.StateID.Chase);
    }
    void Update()
    {
        if (_isDying) return;
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
    public virtual void Shoot()  
    {
        _model.Shoot(_gunSight.position);
    }
    //Lo usa solo SniperAttackState para obtener el punto de mira del player
    //se usa para el OnAnimatorIK del Enemyview
    public Vector3 GetAimPoint()
    {
        return _model.GetAimPoint();
    }
    public void TakeDamage(float dmg)
    {
        if (_isDying) return;
        _model.TakeDamage(dmg);
        if (_model.IsDead)
            StartCoroutine(DeathRoutine());
    }
    private IEnumerator DeathRoutine()
    {
        _isDying = true;
        if (_agent.enabled && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
            _agent.isStopped = true;
        }
        yield return new WaitForSeconds(_deathAnimationDuration);
        _returnToPoolCallBack?.Invoke(this);
    }
    public void PlayRunAnimation()
    {
        _model.NotifyObservers(EnemyEvent.Run);
    }
    public void PlayAimAnimation()
    {
        _model.NotifyObservers(EnemyEvent.Aim);
    }
    public void WarpToPosition(Vector3 position)
    {
        _agent.Warp(position);
        _rb.position = position;
    }
    private void OnAnimatorIK(int layerIndex)
    {
        _view.UpdateAimIK(GetAimPoint());
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, Stats.maxDistance);
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
        _isDying = false;
        _model.RestoreLife(memory.life, memory.isDead);
        if (!memory.isDead)
            _model.NotifyObservers(EnemyEvent.Reset);
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