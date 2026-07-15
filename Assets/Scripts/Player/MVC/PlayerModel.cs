using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerModel : IObservable<PlayerEvent> , IObserver<SaveEvent> , IMementoEntity<PlayerMemento>
{
    private Rigidbody _rb;
    private float _currentLife;
    private ObserverList<PlayerEvent> _playerObservers = new ObserverList<PlayerEvent>();
    private bool _isDead;
    private bool _isMoving;
    private bool _isInvulnerable;
    private Dictionary<SaveEvent, Action> _actions = new Dictionary<SaveEvent, Action>();
    private MementoState<PlayerMemento> _playerMemento;
    private Vector3 _pausedVelocity;
    private Transform _cameraReference;
    private Camera _renderCamera;
    private int _aimPointFrame = -1;
    private Vector3 _cachedAimPoint;
    private bool _isAiming;
    private Dictionary<bool, IRotationStrategy> _rotationStrategies;
    private IRotationStrategy _currentRotation;
    private LayerMask _aimMask;
    private LayerMask _wallMask;
    private float _speedMultiplier = 1f;
    private CapsuleCollider _capsule;
    private float WallCheckSkin = 0.25f;
    public PlayerModel(Player user, Transform cameraReference, LayerMask aimMask,LayerMask wallMask)
    {
        _rb = user.GetComponent<Rigidbody>();
        _isDead = false;
        _currentLife = FlyWeightPointer.Player.maxLife;
        _playerMemento = new MementoState<PlayerMemento>();
        _aimMask = aimMask;
        _wallMask = wallMask;
        _capsule = user.GetComponent<CapsuleCollider>();
        _cameraReference = cameraReference;
        FillRotationStrategies();
        FillDictionary();
    }
    private void FillRotationStrategies()
    {
        _rotationStrategies = new Dictionary<bool, IRotationStrategy>
        {
            { false, new MovementRotationStrategy(_rb, _cameraReference) },
            { true,  new AimRotationStrategy(_rb, _cameraReference) }
        };
        _currentRotation = _rotationStrategies[false];
    }
    public void Move(Vector3 direction)
    {
        bool isMoving = direction.sqrMagnitude > 0.001f;
        if (isMoving)
        {
            float targetRotation = GetTargetRotation(direction);
            //Quaternion.Euler(0, targetRotation, 0)-> Arma una rotacion de targetRotation grados alrededor del eje Y
            // Al multiplicarlo por Vector3.forward -> obtenés el vector unitario del mundo que apunta hacia ese angulo
            Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;
            Vector3 moveVelocity = targetDirection * (FlyWeightPointer.Player.speed * _speedMultiplier);
            moveVelocity = ApplyWallCollision(moveVelocity);
            _rb.MovePosition(_rb.position + moveVelocity * Time.fixedDeltaTime);
        }
        SetMoving(isMoving);
    }
    //metodo que decide que hacer cuando hay pared recibe por parametro una velocidad (dirección + magnitud , y la magnitud se usa para la distancia dinamica del cast).
    //if (IsWallInDirection(moveVelocity, out RaycastHit hit)) -> Hay una pared delante ? -> Intenta deslizarte por ella.
    //moveVelocity.y = 0f; -> No subas por la pared.
    //El movimiento ya proyectado sigue chocando con otra pared -> EJ PJ en una esquina? si es asi cancela el movimiento
    private Vector3 ApplyWallCollision(Vector3 moveVelocity)
    {
        if (IsWallInDirection(moveVelocity, out RaycastHit hit))
        {
            //ProjectOnPlane(Vector que quiero proyectar, normal del plano)
            //es decir le paso la direccion hacia donde me muevo y le paso la normal de la pared con la que choque. hit.normal -> apunta hacia afuera de la pared
            //EJ moveVelocity = (1,0,1) , hit.normal = (0,0,-1)
            //moveVelocity = (1,0,0) -> se elimino la componente Z (la que iba contra la pared)
            //y queda solo la X, que es la paralela a la pared: por eso me deslizo
            //Matematicamente: Calcula cuanto del vector apunta en la dirección de la normal.
            //Se lo resta.
            //Lo que queda es completamente paralelo al plano.
            //OJO: no modifica el vector original, DEVUELVE uno nuevo -> por eso la reasignacion moveVelocity =
            //Conclusion : Vector3.ProjectOnPlane(Vector que quiero proyectar, normal del plano)Elimina la parte del vector que va en la direccion de la normal del plano, dejando solo la componente paralela al plano. :D
            moveVelocity = Vector3.ProjectOnPlane(moveVelocity, hit.normal);
            //si la pared esta inclinada, ProjectOnPlane puede devolver componente en Y
            //y el PJ treparia la pared. Mi movimiento es siempre horizontal (XZ), anulo la Y
            moveVelocity.y = 0f;
            //segundo chequeo en la direccion YA deslizada: caso esquina (dos paredes).
            //si tambien esta bloqueada no intento otro slide, cancelo el movimiento
            //(encadenar proyecciones genera movimientos raros)
            if (IsWallInDirection(moveVelocity, out _)) moveVelocity = Vector3.zero;
        }
        //devuelve la velocity que le paso por parametro ya se modificada o no modificada
        return moveVelocity;
    }
    //devuelve true or false dependiendo si hay pared
    private bool IsWallInDirection(Vector3 moveVelocity, out RaycastHit hit)
    {
        //C# obliga a asignar los parametros out en todos los caminos:
        //lo inicializo por si salgo temprano en el return de abajo sin castear
        hit = default;
        //si practicamente no me estoy moviendo devolve false
        if (moveVelocity.sqrMagnitude < 0.0001f) return false;
        //normalizame la direccion
        Vector3 dir = moveVelocity.normalized;
        //calculo la distancia  
        //moveVelocity.magnitude -> largo del vector
        //moveVelocity.magnitude * Time.fixedDeltaTime ->cuantos metros avanzo en este tick de fisica
        //WallCheckSkin -> margen para no quedar dentro de la pared
        float distance = moveVelocity.magnitude * Time.fixedDeltaTime + WallCheckSkin;
        //calculo origen
        //si mi collider es != null va a ser el centro de la capsula TransformPoint-> paso de local a mundo
        //sino la posicion del rigidbody mas un vector3.up
        Vector3 origin = _capsule != null ? _capsule.transform.TransformPoint(_capsule.center) : _rb.position + Vector3.up;
        //radio
        //si la capsula != null usa _capsule.radius sino 0.3 y al resultado queda al 90%
        float radius = (_capsule != null ? _capsule.radius : 0.3f) * 0.9f;
        //ahora si tiro la sphereCast
        bool blocked = Physics.SphereCast(origin, radius, dir, out hit, distance, _wallMask, QueryTriggerInteraction.Ignore);
        //Debug
        Debug.DrawRay(origin, dir * (distance + radius), blocked ? Color.red : Color.green);
        //retorno true si toco una pared false si no toco nada
        return blocked;
    }
    private void SetMoving(bool isMoving)
    {
        if (isMoving == _isMoving) return;
        _isMoving = isMoving;
        NotifyObservers(isMoving ? PlayerEvent.Move : PlayerEvent.Idle);
    }
    public void Rotate(Vector3 direction)
    {
        _currentRotation.Rotate(direction);
    }
    private float GetTargetRotation(Vector3 inputDir)
    {
        Vector3 input = new Vector3(inputDir.x, 0, inputDir.z);
        return Quaternion.LookRotation(input).eulerAngles.y + _cameraReference.eulerAngles.y;
    }
    public void Shoot()
    {
        NotifyObservers(PlayerEvent.Shoot);
    }
    public void UsePowerUp()
    {
        NotifyObservers(PlayerEvent.UsePowerUp);
    }
    public void NextShootType()
    {
        NotifyObservers(PlayerEvent.NextShootType);
    }
    public void CyclePowerUp()
    {
        NotifyObservers(PlayerEvent.CyclePowerUp);
    }
    public void PreviousShootType()
    {
        NotifyObservers(PlayerEvent.PreviousShootType);
    }
    public void TakeDamage(float dmg)
    {
        if (_isDead || _isInvulnerable) return;
        _currentLife -= dmg;
        float normalizedLife = _currentLife / FlyWeightPointer.Player.maxLife;
        EventManager.TriggerEvent(EventType.PlayerDamage, normalizedLife);
        if (_currentLife <= 0)
        {
            _currentLife = 0;
            _isDead = true;
            NotifyObservers(PlayerEvent.Death);
            EventManager.TriggerEvent(EventType.PlayerDeath);
        }
    }
    public void NotifyObservers(PlayerEvent action) 
    { 
        _playerObservers.NotifyObservers(action); 
    }
    public void Subscribe(IObserver<PlayerEvent> observer) 
    { 
        _playerObservers.Subscribe(observer); 
    }
    public void Unsubscribe(IObserver<PlayerEvent> observer) 
    {
        _playerObservers.Unsubscribe(observer);
    }
    public void Notify(SaveEvent action)
    {
        if (_actions.ContainsKey(action))
        {
            _actions[action].Invoke();
        }
    }
    private void FillDictionary()
    {
        _actions.Add(SaveEvent.Save, SaveState);
        _actions.Add(SaveEvent.Load, TryLoadStates);
    }
    public void SaveState()
    {
        _playerMemento.SaveMemory(
            new PlayerMemento
            {
                position = _rb.position,
                rotation = _rb.rotation,
                life = _currentLife,
                isDead = _isDead,
            });
    }
    public void LoadState(PlayerMemento memory)
    {
        _rb.position = memory.position;
        _rb.rotation = memory.rotation;
        _currentLife = memory.life;
        _isDead = memory.isDead;
        float normalizedLife = _currentLife / FlyWeightPointer.Player.maxLife;
        EventManager.TriggerEvent(EventType.PlayerDamage, normalizedLife);
        _isMoving = false;
        NotifyObservers(_isDead ? PlayerEvent.Death : PlayerEvent.Idle);
    }
    public void TryLoadStates()
    {
        if (_playerMemento.memoriesAmount == 0) return;
        var lastMemory = _playerMemento.LoadMemory();
        LoadState(lastMemory);
    }
    public void Pause()
    {
        _isMoving = false;
        NotifyObservers(PlayerEvent.Idle);
    }
    public void PausePhysics()
    {
        _pausedVelocity = _rb.linearVelocity;
        _rb.linearVelocity = Vector3.zero;
        _rb.useGravity = false;
    }
    public void ResumePhysics()
    {
        _rb.linearVelocity = _pausedVelocity;
        _rb.useGravity = true;
    }
    public void SetAiming(bool isAiming)
    {
        if (isAiming == _isAiming) return;
        _isAiming = isAiming;
        _currentRotation = _rotationStrategies[isAiming];
        NotifyObservers(isAiming ? PlayerEvent.Aim : PlayerEvent.StopAim);
    }
    //Punto de mira: raycast desde la posicion de la camara en la direccion en que mira.
    //Del impacto solo uso el punto; si no pega en nada, devuelvo un punto lejano como fallback.
    //Es la referencia comun a la que convergen todos los sistemas de apuntado,
    //asi todo apunta a lo mismo que el jugador ve bajo el crosshair. Lo consumen:
    //  - GunModel.Shoot()        -> direccion del disparo hitscan (trayectoria, no rotacion).
    //  - Gun.ComputeLaser()      -> endpoint del laser (tampoco es rotacion).
    //  - Gun.LateUpdate()        -> target de la rotacion del arma (esto si es rotacion).
    //  - Player.OnAnimatorIK()   -> UpdateAimIK, el LookAt del torso/cabeza (rotacion via IK).
    public Vector3 GetAimPoint()
    {
        // Un solo raycast por frame: todos los consumidores comparten el mismo punto.
        if (_aimPointFrame == Time.frameCount) return _cachedAimPoint;
        _aimPointFrame = Time.frameCount;
        float aimDistance = FlyWeightPointer.Player.maxDistance;
        if (_renderCamera == null) _renderCamera = Camera.main;
        Ray ray = _renderCamera != null? _renderCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f)): new Ray(_cameraReference.position, _cameraReference.forward);
        _cachedAimPoint = Physics.Raycast(ray, out RaycastHit hit, aimDistance, _aimMask, QueryTriggerInteraction.Ignore)? hit.point: ray.origin + ray.direction * aimDistance;
        return _cachedAimPoint;
    }
    public bool GetAiming => _isAiming;
    public bool IsDead => _isDead;
    #region PowerUps
    /// <summary>
    /// Notifico a los observadores de los eventos ShieldOn y ShieldOff dependiendo si el usuario es invunerable o no
    /// </summary>
    /// <param name="value"></param>
    public void SetInvulnerable(bool value)
    {
        if (value == _isInvulnerable) return;
        _isInvulnerable = value;
        NotifyObservers(value ? PlayerEvent.ShieldOn : PlayerEvent.ShieldOff);
    }
    public void ApplySpeedBoost(float multiplier)
    {
        _speedMultiplier = multiplier;
        NotifyObservers(PlayerEvent.SpeedOn);
    }
    public void RemoveSpeedBoost()
    {
        _speedMultiplier = 1f;
        NotifyObservers(PlayerEvent.SpeedOff);
    }
    //Cura clampeando al maximo de vida. Reuso el evento PlayerDamage:
    //es el que actualiza la barra con la vida normalizada
    //(el nombre quedo de cuando la vida solo podia bajar).
    public void Heal(float amount)
    {
        if (_isDead) return;
        _currentLife = Mathf.Min(_currentLife + amount, FlyWeightPointer.Player.maxLife);
        float normalizedLife = _currentLife / FlyWeightPointer.Player.maxLife;
        EventManager.TriggerEvent(EventType.PlayerDamage, normalizedLife);
        NotifyObservers(PlayerEvent.HealOn);
    }
    #endregion
}