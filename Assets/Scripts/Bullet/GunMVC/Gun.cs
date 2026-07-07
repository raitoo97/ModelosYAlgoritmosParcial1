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
    private IShootStrategy _shootStrategy;
    private GunView _view;
    [SerializeField] private LineRenderer _laser;
    [SerializeField] private Transform _laserDot;
    private Dictionary<PlayerEvent, Action> _actions = new Dictionary<PlayerEvent, Action>();
    private void Start()
    {
        _model = new GunModel(transform.localRotation);
        _shootStrategy = new HitscanShootStrategy(FlyWeightPointer.Projectile.damage * _damageMultiplier, Factions.Player, _hitMask, _maxShootDistance);
        _view = new GunView(_muzzleFlash, _impactEffect, _laser, _laserDot);
        GameManager.instance.player.SubscribeObserver(this);
        FillDictionary();
    }
    private void LateUpdate()
    {
        Quaternion target = _model.ComputeTargetLocalRotation(GameManager.instance.player.GetAimPoint(), transform.localPosition, transform.parent);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, FlyWeightPointer.Entity.rotateSpeed * Time.deltaTime);
        UpdateLaserSight();
    }
    private void UpdateLaserSight()
    {
        if (!_model.IsAiming)
        {
            _view.UpdateLaser(false, Vector3.zero, Vector3.zero, false, Vector3.zero);
            return;
        }
        Vector3 direction = (GameManager.instance.player.GetAimPoint() - _gunSight.position).normalized;
        bool hasHit = Physics.Raycast(_gunSight.position, direction, out RaycastHit hit, _maxShootDistance, _hitMask, QueryTriggerInteraction.Ignore);
        Vector3 end = hasHit ? hit.point : _gunSight.position + direction * _maxShootDistance;
        _view.UpdateLaser(true, _gunSight.position, end, hasHit, hasHit ? hit.normal : Vector3.zero);
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.Shoot, Shoot);
        _actions.Add(PlayerEvent.Aim, () => _model.SetAiming(true));
        _actions.Add(PlayerEvent.StopAim, () => _model.SetAiming(false));
    }
    private void Shoot()
    {
        Vector3 direction = _model.IsAiming? (GameManager.instance.player.GetAimPoint() - _gunSight.position).normalized: _gunSight.forward;
        ShotResult result = _shootStrategy.Shoot(_gunSight.position, direction);
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
