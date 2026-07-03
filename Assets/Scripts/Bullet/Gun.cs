using System;
using System.Collections.Generic;
using UnityEngine;
public class Gun : MonoBehaviour ,IObserver<PlayerEvent>
{
    [SerializeField]private Bullet _bulletPrefab;
    [SerializeField]private Transform _gunSight;
    private bool _isAiming;
    [SerializeField] private float _maxDeviationAngle = 25f;
    private BulletService _bulletService;
    private Dictionary<PlayerEvent, Action> _actions = new Dictionary<PlayerEvent, Action>();
    private int _initPoolSize = 50;
    private Quaternion _initLocalRotation;
    private void Start()
    {
        _bulletService = new BulletService(_bulletPrefab, GameManager.instance._projectilesParent, _initPoolSize);
        GameManager.instance.player.SubscribeObserver(this);
        _initLocalRotation = transform.localRotation;
        FillDictionary();
    }
    private void LateUpdate()
    {
        Quaternion targetLocalRotation = _isAiming? ComputeAimLocalRotation(GameManager.instance.player.GetAimPoint()): _initLocalRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLocalRotation, FlyWeightPointer.Entity.rotateSpeed * Time.deltaTime);
    }
    private Quaternion ComputeAimLocalRotation(Vector3 aimPoint)
    {
        Vector3 desiredDirection = (aimPoint - transform.position).normalized;
        if (desiredDirection.sqrMagnitude < 0.0001f)
            return _initLocalRotation;
        Vector3 currentForward = transform.forward;
        Vector3 clampedDirection = Vector3.RotateTowards(
            currentForward,
            desiredDirection,
            _maxDeviationAngle * Mathf.Deg2Rad,
            0f
        );
        Quaternion targetWorldRotation = Quaternion.LookRotation(clampedDirection, Vector3.up);
        if (transform.parent != null)
            return Quaternion.Inverse(transform.parent.rotation) * targetWorldRotation;
        return targetWorldRotation;
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.Shoot, Shoot);
        _actions.Add(PlayerEvent.Aim, () => _isAiming = true);
        _actions.Add(PlayerEvent.StopAim, () => _isAiming = false);
    }
    private void Shoot()
    {
        Bullet bullet = _bulletService.Shoot(_gunSight.position, _gunSight.rotation);
        new BulletBuilder(bullet).SetDamageMultiplierBullet(4).SetColorMaterial(Color.blue).SetOwnerBullet(BulletOwner.Player).Build();
    }
    public void Notify(PlayerEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
            _actions[Actions].Invoke();
    }
    private void OnDestroy()
    {
        GameManager.instance.player.UnsubscribeObserver(this);
    }
}
