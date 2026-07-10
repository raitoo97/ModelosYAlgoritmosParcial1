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
    [Header("ParticulasDeImpacto")]
    [SerializeField] private ImpactEffect _impactEffectPrefab;
    [SerializeField] private int _impactEffectPoolSize = 10;
    [Header("DisparoRayCast")]
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private float _damageMultiplier = 4f;
    [SerializeField] private LineRenderer _laser;
    [SerializeField] private Transform _laserDot;
    [Header("Abanico")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _bulletPoolSize = 30;
    [SerializeField] private int _pelletCount = 5;
    [SerializeField] private float _spreadArcDegrees = 35f;
    [SerializeField] private float _pelletScale = 1.6f;
    [SerializeField] private Color _spreadBulletColor = new Color(0.3f, 0.8f, 1f);
    // Bordes de la V que telegrafia el abanico de la escopeta al apuntar.
    [SerializeField] private LineRenderer _coneLeftLine;
    [SerializeField] private LineRenderer _coneRightLine;
    public void Init(IAimPointProvider aimPointProvider)
    {
        _aimPointProvider = aimPointProvider;
    }
    private void Start()
    {
        ImpactEffectService impactEffectService = _impactEffectPrefab != null ? new ImpactEffectService(_impactEffectPrefab, GameManager.instance._projectilesParent, _impactEffectPoolSize) : null;
        // La vista se crea antes que las estrategias porque la escopeta
        // recibe su metodo de particula de impacto como callback.
        _view = new GunView(_muzzleFlash, _laser, _laserDot, _coneLeftLine, _coneRightLine, impactEffectService);
        _shootTypes = CreateShootTypes();
        _currentShootType = 0;
        // El cono usa el arco real de la escopeta y el alcance real del perdigon
        // (maxLife de Bullet es distancia), asi la V muestra el disparo verdadero.
        // Al modelo le paso SOLO el tipo de disparo activo (el del indice actual):
        // su estrategia y su indicador. El modelo nunca conoce la lista completa,
        // no sabe cuantos tipos existen ni cuales son: solo sabe ejecutar
        _model = new GunModel(transform.localRotation, _shootTypes[_currentShootType].strategy, _shootTypes[_currentShootType].indicator, _hitMask, FlyWeightPointer.Player.maxDistance, _spreadArcDegrees, FlyWeightPointer.Projectile.maxLife);
        FillDictionary();
    }
    //tipo de disparo tambien cambia su indicador.
    private List<ShootType> CreateShootTypes()
    {
        // Pool de balas propio del gun.
        BulletService bulletService = new BulletService(_bulletPrefab, GameManager.instance._projectilesParent, _bulletPoolSize);
        return new List<ShootType>
        {
            new ShootType
            {
                strategy = new HitscanShootStrategy(FlyWeightPointer.Projectile.damage * _damageMultiplier, Factions.Player, _hitMask, FlyWeightPointer.Player.maxDistance),
                indicator = AimIndicatorType.Laser
            },
            new ShootType
            {
                strategy = new SpreadShootStrategy(bulletService, Factions.Player, _spreadBulletColor, _pelletCount, _spreadArcDegrees, FlyWeightPointer.Projectile.damage * _damageMultiplier, _view.PlayImpactEffect, _pelletScale),
                indicator = AimIndicatorType.Cone
            }
        };
    }
    private void LateUpdate()
    {
        Quaternion target = _model.ComputeTargetLocalRotation(_aimPointProvider.GetAimPoint(), transform.position, transform.parent);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, FlyWeightPointer.Player.rotateSpeed * Time.deltaTime);
        // Cada frame refresco el indicador de punteria: como el player y el mouse
        // se mueven continuamente, las lineas hay que recalcularlas siempre.
        UpdateAimIndicator();
    }
    //Calcula TODO el indicador de punteria del frame segun el tipo activo; la vista solo dibuja el resultado
    private void UpdateAimIndicator()
    {
        // PASO 1 (modelo, la matematica): le pido al modelo que calcule las lineas
        // de ESTE frame. El modelo decide QUE calcular segun el _indicator que
        // tenga guardado desde el ultimo cambio de arma.
        AimIndicatorState indicator = _model.ComputeAimIndicator(_gunSight.position, _aimPointProvider.GetAimPoint());
        // PASO 2 (vista, el dibujo): le paso el RESULTADO ya calculado al view para que dibuje las lineas. La vista no sabe que tipo de indicador es ni como se calcula, solo dibuja lo que le llega.
        _view.UpdateAimIndicator(indicator);
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
        // CAMBIO al modelo la estrategia activa por esta nueva. El modelo no conoce la lista completa de estrategias, solo sabe ejecutar la que tiene asignada.
        // El modelo suelta la referencia a la anterior y agarra la nueva.
        _model.SetShootStrategy(current.strategy, current.indicator);
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
    }
}
