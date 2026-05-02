using System;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    private FSM _fsm;
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    [SerializeField] private Bullet _bulletPrefab;
    private BulletService _bulletService;
    [SerializeField]private Transform _projectilesParent;// poner al game manager
    [SerializeField]private Transform _gunSight;
    private int _initPoolSize = 50;
    private Action<Enemy> _returnToPoolCallBack;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _bulletService = new BulletService(_bulletPrefab, _projectilesParent, _initPoolSize);
    }
    private void OnEnable()
    {
        _fsm = new FSM();
        _fsm.AddState(FSM.StateID.Chase, new ChaseState(transform,_agent,this,_fsm));
        _fsm.AddState(FSM.StateID.Attack, new AttackState(transform, _agent, this, _fsm));
        _fsm.ChangeState(FSM.StateID.Chase);
    }
    void Update()
    {
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
            .SetDamage(FlyWeightPointer.Projectile._damage)
            .SetColorMaterial(Color.red)
            .Build();
    }
    public void Die()
    {
        _returnToPoolCallBack?.Invoke(this);
    }
    public void ResetEnemy()
    {
        //_life = FlyWeightPointer.Entity.maxLife;
        // o reiniciar estado inicial
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, FlyWeightPointer.Entity.maxDistance);
    }
}