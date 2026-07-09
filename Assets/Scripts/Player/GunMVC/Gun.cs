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
    private IAimPointProvider _aimPointProvider;
    [Header("DisparoRayCast")]
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private ParticleSystem _impactEffect;
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private float _damageMultiplier = 4f;
    [SerializeField] private LineRenderer _laser;
    [SerializeField] private Transform _laserDot;
    [Header("Abanico")]
    [SerializeField] private int _impactEffectPoolSize = 10;
    [SerializeField] private float _pelletScale = 1.6f;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _bulletPoolSize = 30;
    [SerializeField] private int _pelletCount = 5;
    [SerializeField] private float _spreadArcDegrees = 35f;
    // Celeste para distinguirlas de las rojas del Ranger y naranjas del Shotgunner.
    [SerializeField] private Color _spreadBulletColor = new Color(0.3f, 0.8f, 1f);
    public void Init(IAimPointProvider aimPointProvider)
    {
        _aimPointProvider = aimPointProvider;
    }
    private void Start()
    {
        _view = new GunView(_muzzleFlash, _impactEffect, _laser, _laserDot, GameManager.instance._projectilesParent, _impactEffectPoolSize);
        _shootTypes = CreateShootTypes();
        _currentShootType = 0;
        _model = new GunModel(transform.localRotation, _shootTypes[_currentShootType].strategy, _hitMask, FlyWeightPointer.Player.maxDistance);
        FillDictionary();
    }
    private List<ShootType> CreateShootTypes()
    {
        // Pool de balas propio del gun, mismo patron que usan los spawners enemigos.
        BulletService bulletService = new BulletService(_bulletPrefab, GameManager.instance._projectilesParent, _bulletPoolSize);
        return new List<ShootType>
        {
            new ShootType
            {
                strategy = new HitscanShootStrategy(FlyWeightPointer.Projectile.damage * _damageMultiplier, Factions.Player, _hitMask, FlyWeightPointer.Player.maxDistance),
                showLaser = true
            },
            new ShootType
            {
                strategy = new SpreadShootStrategy(bulletService, Factions.Player, _spreadBulletColor, _pelletCount, _spreadArcDegrees, FlyWeightPointer.Projectile.damage * _damageMultiplier,_view.PlayImpactEffect, _pelletScale),
                showLaser = false
            }
        };
    }
    private void LateUpdate()
    {
        Quaternion target = _model.ComputeTargetLocalRotation(_aimPointProvider.GetAimPoint(), transform.position, transform.parent);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, FlyWeightPointer.Player.rotateSpeed * Time.deltaTime);
        UpdateLaserSight();
    }
    private void UpdateLaserSight()
    {
        LaserState laser = _model.ComputeLaser(_gunSight.position, _aimPointProvider.GetAimPoint());
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
        // Cambio el indice del tipo de disparo de forma circular.
        // step = 1  -> siguiente tipo.
        // step = -1 -> tipo anterior.
        // Ejemplo con 2 estrategias (0 = Pistola, 1 = Escopeta):
        // (0 + 1 + 2) % 2 = 1 -> pasa a Escopeta.
        // (1 + 1 + 2) % 2 = 0 -> vuelve a Pistola.
        // (0 - 1 + 2) % 2 = 1 -> retrocede a Escopeta.
        // Sumar Count evita indices negativos y el modulo (%) mantiene
        // el indice siempre entre 0 y Count - 1.
        _currentShootType = (_currentShootType + step + _shootTypes.Count) % _shootTypes.Count;
        ShootType current = _shootTypes[_currentShootType];
        _model.SetShootStrategy(current.strategy, current.showLaser);
    }
    private void Shoot()
    {
        ShotResult result = _model.Shoot(_gunSight.position, _aimPointProvider.GetAimPoint(), _gunSight.forward);
        _view.PlayShootEffects(result);
    }
    public void Notify(PlayerEvent actions)
    {
        if (_actions.ContainsKey(actions))
            _actions[actions].Invoke();
    }
    private struct ShootType
    {
        public IShootStrategy strategy;
        public bool showLaser;
    }
}
