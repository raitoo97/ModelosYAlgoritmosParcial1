using System;
using System.Collections.Generic;
using UnityEngine;
// Runner GENERICO de power ups: cantidad de cargas, el timer (cuanto dura
// el efecto) y el estado activo. Este componente decide CUANDO corre el
// efecto; QUE hace el efecto es de la estrategia.
// En el player se activa via PlayerEvent (observer) con el evento UsePowerUp.
// El efecto termina por timer (Duration) o por corte forzado (evento Death).
public class PowerUpController : MonoBehaviour, IObserver<PlayerEvent>, IPowerUpCollector, IPauseable
{
    private Dictionary<PlayerEvent, Action> _actions = new Dictionary<PlayerEvent, Action>();
    private PowerUpStrategyDependencies _deps;
    private PowerUpConfig _config;
    private IPowerUpStrategy _strategy;
    private int _charges;
    private float _timer;
    private bool _isActive;
    // Expuesto para una futura UI de cargas.
    public int Charges => _charges;
    // user = el duenio de este controller.
    // Las estrategias que fabrique este controller van a apuntar a EL,
    // porque se construyen con estas dependencias.
    /// <summary>
    /// paso como user al usuario que va a recibir PowerUps
    /// </summary>
    /// <param name="user"></param>
    public void Init(GameObject user)
    {
        //creo la dependencia en este momento solo necesito el  user
        _deps = new PowerUpStrategyDependencies { user = user };
        FillDictionary();
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.UsePowerUp, () => TryActivate());
        _actions.Add(PlayerEvent.Death, ForceCancel);
    }
    //cuando un pickup aniade una carga del power up:
    //si es la misma config que tengo equipada, sumo una carga
    //si es una distinta y no hay efecto corriendo, la equipo con una carga
    //y fabrico la estrategia de ESTE controller con MIS dependencias (mi user)
    //si tengo un efecto corriendo y agarro uno de otro tipo, no lo acepto
    //(devuelvo false y el pickup no se consume, queda en el mapa)
    public bool AddCharge(PowerUpConfig config)
    {
        if (config == null) return false;
        if (config == _config)
        {
            _charges++;
            return true;
        }
        if (_isActive) return false;
        //seteo la configuracion y la estrategia
        _config = config;
        _strategy = config.CreateStrategy(_deps);
        _charges = 1;
        return true;
    }
    // Consume una carga y prende el efecto. Devuelve bool para que
    // un enemigo (o una UI) sepa si pudo activarlo.
    public bool TryActivate()
    {
        if (_isActive || _charges <= 0 || _strategy == null) 
        {
            if (_strategy == null)
                Debug.LogWarning("No hay estrategia para activar el power up.");
            if (_isActive)
                Debug.LogWarning("No se puede activar el power up: ya esta activo.");
            if (_charges <= 0)
                Debug.LogWarning("No se puede activar el power up: no hay cargas.");
            return false;
        } 
        Debug.Log("Activando el power up: " + _strategy.GetType().Name);
        _charges--;
        _timer = 0;
        _isActive = true;
        _strategy.Activate();
        return true;
    }
    // Timer acumulado en Update: cuando llega a la Duration configurada en el
    // PowerUpConfig, el efecto termina solo
    private void Update()
    {
        if (!_isActive) return;
        _timer += Time.deltaTime;
        if (_timer >= _config.Duration)
        {
            _isActive = false;
            _strategy.Deactivate();
        }
    }
    // Corte inmediato (muerte del user): apaga el efecto sin esperar el timer.
    public void ForceCancel()
    {
        if (!_isActive) return;
        _isActive = false;
        _strategy.Deactivate();
    }
    public void Notify(PlayerEvent actions)
    {
        if (_actions.ContainsKey(actions))
            _actions[actions].Invoke();
    }
    public void Pause()
    {
        enabled = false;
    }
    public void Resume()
    {
        enabled = true;
    }
}
//Flujo del power up para el player:
//1) Creo el asset (PowerUpConfig) y se lo asigno a un PowerUpPickup del mapa.
//   el asset NO contiene la estrategia, sabe FABRICARLA (CreateStrategy).
//2) Al pisarlo, el pickup busca el IPowerUpCollector en el collider que entro
//   (el root del player, donde vive este controller) y llama AddCharge(config).
//3) AddCharge equipa la config y fabrica la estrategia de ESTE controller,
//   construida con las deps que recibio en Init (user = el duenio del controller).
//4) Con Space, PlayerController llama _model.UsePowerUp() y el model notifica
//   el evento UsePowerUp a TODOS sus observers; este controller lo tiene
//   mapeado a TryActivate() en su diccionario.
//5) TryActivate consume una carga y llama strategy.Activate().
//   ShieldPowerUpStrategy llama ActivateShield() del IShieldable que le
//   inyectaron al construirla: el del user de ESTE controller. es decir el del player
//6) Player.ActivateShield() -> _model.SetInvulnerable(true): el model corta
//   el danio en TakeDamage y notifica ShieldOn/ShieldOff, y el view muestra
//   u oculta la burbuja. El efecto termina por timer (Duration) o por muerte
//   (evento Death -> ForceCancel).

//FLUJO DEL COMPOSITE (como llegan a llamarse sus metodos):
//el controller NUNCA sabe que existe el composite; para el es un
//IPowerUpStrategy mas.
//1) Piso el pickup del combinado -> AddCharge recibe el CompositePowerUpConfig
//2) AddCharge llama config.CreateStrategy(deps) como siempre, pero la version
//   del composite recorre sus _parts y le pide a CADA parte que arme su estrategia
//   (pasandole las MISMAS deps a todas, por eso todas apuntan al mismo user).
//   Con esa lista arma UNA CompositePowerUpStrategy y devuelve esa.
//3) Con Space, TryActivate llama _strategy.Activate() sin saber que es un
//   composite. El Activate del CompositePowerUpStrategy hace foreach y llama el Activate
//   de cada parte: HealPowerUpStrategy cura (instantaneo) y
//   SpeedPowerUpStrategy aplica el multiplicador.
//4) Cuando el timer llega a la Duration (LA DEL ASSET COMPOSITE, las de las
//   partes se ignoran), Update llama _strategy.Deactivate() -> foreach ->
//   Deactivate de cada parte: Speed vuelve el multiplicador a 1 y Heal no
//   hace nada (su Deactivate esta vacio).
//5) La muerte (Death -> ForceCancel) recorre el mismo camino que el punto 4.


//NOTA (por que se activa el escudo del player y no el de otra entidad):
//el escudo que se activa es el del player NO por ser el collector, sino
//porque la estrategia se fabrica con el IShieldable del user que este
//controller recibio en Init(). El asset (config) es compartido, pero cada
//controller fabrica SU PROPIA instancia de estrategia apuntando a su duenio.
//Esa propiedad es la que permite que maniana un enemigo use el mismo asset
//sin que se pisen: su controller fabricaria OTRA estrategia apuntando a el.
