using System;
using System.Collections.Generic;
using UnityEngine;
public class Gun : MonoBehaviour ,IObserver<PlayerEvent>
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _gunSight;
    private BulletService _bulletService;
    private Dictionary<PlayerEvent, Action> _actions;
    private int _initPoolSize = 50;
    private void Start()
    {
        _bulletService = new BulletService(_bulletPrefab,GameManager.instance._projectilesParent,_initPoolSize);
        FillDictionary();
    }
    private void FillDictionary()
    {
        _actions = new Dictionary<PlayerEvent, Action>();
        _actions.Add(PlayerEvent.Shoot, Shoot);
    }
    private void Shoot()
    {
        Bullet bullet = _bulletService.Shoot(_gunSight.position, _gunSight.rotation);
        new BulletBuilder(bullet).SetSpeed(FlyWeightPointer.Projectile.speed).SetDamage(FlyWeightPointer.Projectile.damage * 4).SetColorMaterial(Color.blue).SetOwnerBullet(BulletOwner.Player).Build();
    }
    public void Notify(PlayerEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
        {
            _actions[Actions].Invoke();
        }
    }
}
