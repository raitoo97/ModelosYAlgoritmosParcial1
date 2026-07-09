using System;
using System.Collections.Generic;
using UnityEngine;
public class Gun : MonoBehaviour ,IObserver<PlayerEvent>
{
    [SerializeField] private Transform _gunSight;
    private Dictionary<PlayerEvent, Action> _actions = new Dictionary<PlayerEvent, Action>();
    private List<ShootType> _shootTypes = new List<ShootType>();
    private GunModel _model;
    private GunView _view;
    private int _currentShootType;
    [Header("DisparoRayCast")]
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private ParticleSystem _impactEffect;
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private float _maxShootDistance = 100f;
    [SerializeField] private float _damageMultiplier = 4f;
    [SerializeField] private LineRenderer _laser;
    [SerializeField] private Transform _laserDot;
    [Header("Abanico")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _bulletPoolSize = 30;
    [SerializeField] private int _pelletCount = 5;
    [SerializeField] private float _spreadArcDegrees = 35f;
    [SerializeField] private float _pelletDamageMultiplier = 0.8f;
    // Celeste para distinguirlas de las rojas del Ranger y naranjas del Shotgunner.
    [SerializeField] private Color _spreadBulletColor = new Color(0.3f, 0.8f, 1f);
    private void Start()
    {
        // Pool de balas propio del gun, mismo patron que usan los spawners enemigos.
        BulletService bulletService = new BulletService(_bulletPrefab, GameManager.instance._projectilesParent, _bulletPoolSize);
        _shootTypes = new List<ShootType>
        {
            new ShootType
            {
                strategy = new HitscanShootStrategy(FlyWeightPointer.Projectile.damage * _damageMultiplier, Factions.Player, _hitMask, _maxShootDistance),
                showLaser = true
            },
            new ShootType
            {
                strategy = new SpreadShootStrategy(bulletService, Factions.Player, _spreadBulletColor, _pelletCount, _spreadArcDegrees, _pelletDamageMultiplier),
                showLaser = false
            }
        };
        _currentShootType = 0;
        _model = new GunModel(transform.localRotation, _shootTypes[_currentShootType].strategy, _hitMask, _maxShootDistance);
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
        _actions.Add(PlayerEvent.NextShootType, () => CycleShootType(1));
        _actions.Add(PlayerEvent.PreviousShootType, () => CycleShootType(-1));
    }
    private void CycleShootType(int step)
    {
        // Cambio el índice del tipo de disparo de forma circular.
        // step = 1  -> siguiente tipo.
        // step = -1 -> tipo anterior.
        // Ejemplo con 4 estrategias (indices 0,1,2,3):
        // Si estoy en 3 y avanzo:
        // (3 + 1 + 4) % 4 = 8 % 4 = 0  -> vuelve al primero.
        // Si estoy en 0 y retrocedo:
        // (0 - 1 + 4) % 4 = 3 % 4 = 3  -> vuelve al último.
        // Sumar Count evita índices negativos y el módulo (%) mantiene
        // el resultado siempre entre 0 y Count - 1.
        _currentShootType = (_currentShootType + step + _shootTypes.Count) % _shootTypes.Count;
        ShootType current = _shootTypes[_currentShootType];
        _model.SetShootStrategy(current.strategy, current.showLaser);
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
    private struct ShootType
    {
        public IShootStrategy strategy;
        public bool showLaser;
    }
}
