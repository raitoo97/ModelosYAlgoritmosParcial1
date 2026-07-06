using System;
using System.Collections.Generic;
using UnityEngine;
public class Gun : MonoBehaviour ,IObserver<PlayerEvent>
{
    [SerializeField]private Bullet _bulletPrefab;
    [SerializeField]private Transform _gunSight;
    [SerializeField] private float _maxDeviationAngle = 25f;
    private GunModel _model;
    private BulletService _bulletService;
    private Dictionary<PlayerEvent, Action> _actions = new Dictionary<PlayerEvent, Action>();
    private int _initPoolSize = 50;
    private void Start()
    {
        _bulletService = new BulletService(_bulletPrefab, GameManager.instance._projectilesParent, _initPoolSize);
        _model = new GunModel(_maxDeviationAngle, transform.localRotation);
        GameManager.instance.player.SubscribeObserver(this);
        FillDictionary();
    }
    private void LateUpdate()
    {
        Quaternion target = _model.ComputeTargetLocalRotation(GameManager.instance.player.GetAimPoint(), transform.position, transform.parent);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, FlyWeightPointer.Entity.rotateSpeed * Time.deltaTime);
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.Shoot, Shoot);
        _actions.Add(PlayerEvent.Aim, () => _model.SetAiming(true));
        _actions.Add(PlayerEvent.StopAim, () => _model.SetAiming(false));
    }
    private void Shoot()
    {
        Quaternion bulletRotation = _model.IsAiming? Quaternion.LookRotation(GameManager.instance.player.GetAimPoint() - _gunSight.position): _gunSight.rotation;
        Bullet bullet = _bulletService.Shoot(_gunSight.position, bulletRotation);
        new BulletBuilder(bullet).SetDamageMultiplierBullet(4).SetColorMaterial(Color.blue).SetOwnerBullet(Factions.Player).Build();
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
