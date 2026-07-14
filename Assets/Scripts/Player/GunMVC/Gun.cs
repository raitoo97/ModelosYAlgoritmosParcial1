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
    // las armas ahora son assets. Agregar un arma = crear el asset
    // y sumarlo a esta lista. El orden de la lista es el orden de ciclado
    // y el indice 0 es el arma inicial. Gun no conoce tipos concretos.
    [Header("Armas")]
    [SerializeField] private List<WeaponConfig> _weapons;
    [Header("ParticulasDeImpacto")]
    [SerializeField] private ImpactEffect _impactEffectPrefab;
    [SerializeField] private int _impactEffectPoolSize = 10;
    [Header("ReferenciasDeEscena")]
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private LineRenderer _laser;
    [SerializeField] private Transform _laserDot;
    [SerializeField] private LineRenderer _coneLeftLine;
    [SerializeField] private LineRenderer _coneRightLine;
    [Header("PoolDeBalas")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _bulletPoolSize = 30;
    public void Init(IAimPointProvider aimPointProvider)
    {
        _aimPointProvider = aimPointProvider;
    }
    private void Start()
    {
        ImpactEffectService impactEffectService = _impactEffectPrefab != null ? new ImpactEffectService(_impactEffectPrefab, GameManager.instance.projectilesParent, _impactEffectPoolSize) : null;
        _view = new GunView(_muzzleFlash, _laser, _laserDot, _coneLeftLine, _coneRightLine, impactEffectService);
        _shootTypes = CreateShootTypes();
        _currentShootType = 0;
        _model = new GunModel(transform.localRotation, _hitMask, FlyWeightPointer.Player.maxDistance, FlyWeightPointer.Projectile.maxLife);
        ApplyShootType();
        FillDictionary();
    }
    private List<ShootType> CreateShootTypes()
    {
        // Pool de balas propio del gun, compartido por todas sus armas de proyectil.
        BulletService bulletService = new BulletService(_bulletPrefab, GameManager.instance.projectilesParent, _bulletPoolSize);
        ShootStrategyDependencies deps = new ShootStrategyDependencies
        {
            bulletService = bulletService,
            owner = Factions.Player,
            hitMask = _hitMask,
            maxDistance = FlyWeightPointer.Player.maxDistance,
            onImpact = _view.PlayImpactEffect
        };
        // Cada asset construye su propia estrategia//
        // se crea DENTRO del asset, no en el Gun. Gun solo recorre la lista y le
        // pide a cada arma la suya. Camino para agregar un arma: creo el asset,
        // lo agrego a la lista del Gun, y listo (Gun no se toca).
        // Todas comparten el struct de dependencias, pero es cada CONFIG la que
        // elige que usar al construir: Por ejemplo la estrategia de pistola no necesita el onImpact, pero la de escopeta si.
        List<ShootType> shootTypes = new List<ShootType>();
        foreach (WeaponConfig weapon in _weapons)
        {
            shootTypes.Add(new ShootType
            {
                strategy = weapon.CreateStrategy(deps),
                indicator = weapon.Indicator,
                coneArcDegrees = weapon.ConeArcDegrees,
                color = weapon.WeaponColor,
                icon = weapon.Icon
            });
        }
        return shootTypes;
    }
    private void LateUpdate()
    {
        Quaternion target = _model.ComputeTargetLocalRotation(_aimPointProvider.GetAimPoint(), transform.position, transform.parent);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, FlyWeightPointer.Player.rotateSpeed * Time.deltaTime);
        UpdateAimIndicator();
    }
    private void UpdateAimIndicator()
    {
        // PASO 1 (modelo, la matematica): le pido al modelo que calcule las lineas
        // de ESTE frame. El modelo decide QUE calcular segun el _indicator que
        // tenga guardado desde el ultimo cambio de arma.
        AimIndicatorState indicatorState = _model.ComputeAimIndicator(_gunSight.position, _aimPointProvider.GetAimPoint());
        // PASO 2 (vista, el dibujo): le paso el RESULTADO ya calculado al view para que dibuje las lineas. La vista no sabe que tipo de indicador es ni como se calcula, solo dibuja lo que le llega.
        _view.UpdateAimIndicator(indicatorState);
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
        ApplyShootType();
    }
    private void ApplyShootType()
    {
        // UNICO punto donde se equipa el arma activa (lo usan Start y
        // CycleShootType). Reparte cada dato a quien le corresponde:
        // al MODELO lo que calcula (estrategia, indicador, arco) y a la
        // VISTA lo que dibuja (el color del arma). Se pinta UNA vez por
        // cambio de arma.
        ShootType current = _shootTypes[_currentShootType];
        _model.SetShootStrategy(current.strategy, current.indicator, current.coneArcDegrees);
        _view.SetIndicatorColor(current.color);
        EventManager.TriggerEvent(EventType.WeaponChanged, current.icon);
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
        public AimIndicatorType indicator;
        public float coneArcDegrees;
        public Color color;
        public Sprite icon;
    }
}
