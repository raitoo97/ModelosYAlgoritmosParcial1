using System;
using System.Collections.Generic;
using UnityEngine;
public class Gun : MonoBehaviour ,IObserver<PlayerEvent>
{
    [SerializeField] private Transform _gunSight;
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private ParticleSystem _impactEffect;
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private float _maxShootDistance = 100f;
    [SerializeField] private float _damageMultiplier = 4f;
    private GunModel _model;
    private GunView _view;
    [SerializeField] private LineRenderer _laser;
    [SerializeField] private Transform _laserDot;
    private Dictionary<PlayerEvent, Action> _actions = new Dictionary<PlayerEvent, Action>();
    private void Start()
    {
        _model = new GunModel(transform.localRotation, new HitscanShootStrategy(FlyWeightPointer.Projectile.damage * _damageMultiplier, Factions.Player, _hitMask, _maxShootDistance), _hitMask, _maxShootDistance);
        _view = new GunView(_muzzleFlash, _impactEffect, _laser, _laserDot);
        GameManager.instance.player.SubscribeObserver(this);
        FillDictionary();
    }
    private void LateUpdate()
    {
        Quaternion target = _model.ComputeTargetLocalRotation(GameManager.instance.player.GetAimPoint(), transform.position, transform.parent);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, FlyWeightPointer.Entity.rotateSpeed * Time.deltaTime);
        UpdateLaserSight();
    }
    private void UpdateLaserSight()
    {
        LaserState laser = _model.ComputeLaser(_gunSight.position, GameManager.instance.player.GetAimPoint());
        _view.UpdateLaser(laser);
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.Shoot, Shoot);
        _actions.Add(PlayerEvent.Aim, () => _model.SetAiming(true));
        _actions.Add(PlayerEvent.StopAim, () => _model.SetAiming(false));
        _actions.Add(PlayerEvent.Death, () => _model.SetAiming(false));
    }
    private void Shoot()
    {
        ShotResult result = _model.Shoot(_gunSight.position, GameManager.instance.player.GetAimPoint(), _gunSight.forward);
        _view.PlayShootEffects(result);
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
