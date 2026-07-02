using System;
using System.Collections.Generic;
using UnityEngine;
public class Gun : MonoBehaviour ,IObserver<PlayerEvent>
{
    [SerializeField]private Bullet _bulletPrefab;
    [SerializeField]private Transform _gunSight;
    private bool _isAiming;
    private BulletService _bulletService;
    private Dictionary<PlayerEvent, Action> _actions = new Dictionary<PlayerEvent, Action>();
    private int _initPoolSize = 50;
    private void Start()
    {
        _bulletService = new BulletService(_bulletPrefab,GameManager.instance._projectilesParent,_initPoolSize);
        GameManager.instance.player.SubscribeObserver(this);
        FillDictionary();
    }
    private void LateUpdate()
    {
        if (!_isAiming) return;
        AimAt(GameManager.instance.player.GetAimPoint());
    }
    private void AimAt(Vector3 aimPoint)
    {
        Vector3 direction = (aimPoint - this.transform.position).normalized;
        if (direction.sqrMagnitude < 0.0001f) return;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * FlyWeightPointer.Projectile.damage);
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
