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
    public Factions Faction => Factions.Enemy;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = Stats.speed;
        _playerTransform = GameManager.instance.player.transform;
        _model = new EnemyModel(GetComponent<Rigidbody>(), _playerTransform, Stats);
        _view = new EnemyView(this);
        _model.Subscribe(_view);
        _fsm = new FSM();
        _fsm.AddState(FSM.StateID.Chase, new ChaseState(transform, _agent, this, _fsm, _playerTransform, Stats));
        _fsm.AddState(FSM.StateID.Attack, CreateAttackState(_playerTransform));
        _fsm.ChangeState(FSM.StateID.Chase);
    }
    protected virtual IState CreateAttackState(Transform playerTransform)
    {
        return new AttackState(transform, _agent, this, _fsm, playerTransform, Stats);
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
    public void Shoot()
    {
        _model.Shoot(_gunSight.position);
    }
    public void TakeDamage(float dmg)
    {
        if (_isDying) return;
        _model.TakeDamage(dmg);
        if (_model.IsDead)
            StartCoroutine(DeathRoutine());
    }
    // Espera a que termine la animacion de muerte antes de devolverlo al pool.
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
    }
    private void OnAnimatorIK(int layerIndex)
    {
        _view.UpdateAimIK(_model.GetAimPoint());
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