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
    // Un slot por TIPO de power up agarrado: la config, su estrategia ya
    // fabricada y cuantas cargas tengo.
    private class PowerUpSlot
    {
        public PowerUpConfig config;
        public IPowerUpStrategy strategy;
        public int charges;
    }
    private Dictionary<PlayerEvent, Action> _actions = new Dictionary<PlayerEvent, Action>();
    private PowerUpStrategyDependencies _deps;
    private List<PowerUpSlot> _slots = new List<PowerUpSlot>();
    // Indice del slot seleccionado: es el que activa Space. F lo cicla.
    private int _selectedIndex;
    // El efecto QUE ESTA CORRIENDO se guarda aparte del slot, porque el
    // slot puede salir de la lista (ultima carga usada) mientras el efecto
    // sigue vivo, y asi agarrar o ciclar nunca pisa lo que esta corriendo.
    private IPowerUpStrategy _activeStrategy;
    private PowerUpConfig _activeConfig;
    private float _timer;
    private bool _isActive;
    // Para una futura UI: cargas y nombre del seleccionado.
    public int Charges => _slots.Count > 0 ? _slots[_selectedIndex].charges : 0;
    public string SelectedName => _slots.Count > 0 ? _slots[_selectedIndex].config.name : "";
    // user = el duenio de este controller.
    public void Init(GameObject user)
    {
        _deps = new PowerUpStrategyDependencies { user = user };
        FillDictionary();
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.UsePowerUp, () => TryActivate());
        _actions.Add(PlayerEvent.CyclePowerUp, CycleSelected);
        _actions.Add(PlayerEvent.Death, ForceCancel);
    }
    //cuando un pickup aniade una carga: si ya hay slot de esa config le sumo
    //una carga; si es tipo nuevo, fabrico SU estrategia una sola vez y lo
    //agrego al final de la mochila.agarrar NO cambia la seleccion
    //(salvo mochila vacia): la seleccion la maneja el jugador con F.
    public bool AddCharge(PowerUpConfig config)
    {
        if (config == null) return false;
        PowerUpSlot slot = _slots.Find(s => s.config == config);
        if (slot != null)
        {
            slot.charges++;
            return true;
        }
        _slots.Add(new PowerUpSlot
        {
            config = config,
            strategy = config.CreateStrategy(_deps),
            charges = 1
        });
        if (_slots.Count == 1) _selectedIndex = 0;
        return true;
    }
    // Cambio el indice del tipo de PowerUp de forma circular.
    // Ejemplo con 2 PowerUps (0 = speed, 1 = shield):
    // (0 + 1 + 2) % 2 = 1 -> pasa a shield.
    // (1 + 1 + 2) % 2 = 0 -> vuelve a speed.
    private void CycleSelected()
    {
        if (_slots.Count <= 1) return;
        _selectedIndex = (_selectedIndex + 1) % _slots.Count;
        Debug.Log("Power up seleccionado: " + SelectedName + " x" + Charges);
    }
    // Consume una carga del slot SELECCIONADO y prende su efecto.
    // Devuelve bool para que un enemigo (o una UI) sepa si pudo.
    public bool TryActivate()
    {
        if (_isActive || _slots.Count == 0) return false;
        PowerUpSlot slot = _slots[_selectedIndex];
        slot.charges--;
        // sin cargas el slot sale de la mochila; el efecto puede seguir
        // corriendo igual porque guardo su estrategia y config aparte.
        // Al removerlo, la seleccion pasa sola al siguiente de la lista
        // (mismo indice) y solo hay que envolver si era el ultimo.
        if (slot.charges <= 0)
        {
            _slots.RemoveAt(_selectedIndex);
            if (_selectedIndex >= _slots.Count) _selectedIndex = 0;
        }
        _activeStrategy = slot.strategy;
        _activeConfig = slot.config;
        _timer = 0;
        _isActive = true;
        _activeStrategy.Activate();
        return true;
    }
    // Timer acumulado en Update: cuando llega a la Duration configurada en el
    // PowerUpConfig, el efecto termina solo
    private void Update()
    {
        if (!_isActive) return;
        _timer += Time.deltaTime;
        if (_timer >= _activeConfig.Duration)
        {
            _isActive = false;
            _activeStrategy.Deactivate();
        }
    }
    // Corte inmediato (muerte del user): apaga el efecto sin esperar el timer.
    public void ForceCancel()
    {
        if (!_isActive) return;
        _isActive = false;
        _activeStrategy.Deactivate();
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
//3) AddCharge guarda la carga en la MOCHILA (lista de slots): si la config ya
//   tiene slot le suma carga; si es nueva, fabrica su estrategia con las deps
//   que recibio en Init (user = el duenio) y la agrega al final.
//4) Con F, PlayerController -> _model.CyclePowerUp() -> evento CyclePowerUp
//   -> CycleSelected() mueve el indice circular (igual que las armas del Gun).
//5) Con Space, PlayerController -> _model.UsePowerUp() -> evento UsePowerUp
//   -> TryActivate() consume una carga del slot SELECCIONADO y llama
//   strategy.Activate(). ShieldPowerUpStrategy llama ActivateShield() del
//   IShieldable que le inyectaron al construirla: el del user de ESTE
//   controller, es decir el del player. Un slot sin cargas sale de la lista.
//6) Player.ActivateShield() -> _model.SetInvulnerable(true): el model corta
//   el danio en TakeDamage y notifica ShieldOn/ShieldOff, y el view muestra
//   u oculta la burbuja. El efecto termina por timer (Duration) o por muerte
//   (evento Death -> ForceCancel).
//
//NOTA (por que se activa el escudo del player y no el de otra entidad):
//el escudo que se activa es el del player NO por ser el collector, sino
//porque la estrategia se fabrica con el IShieldable del user que este
//controller recibio en Init(). El asset (config) es compartido, pero cada
//controller fabrica SU PROPIA instancia de estrategia apuntando a su duenio.
//Esa propiedad es la que permite que maniana un enemigo use el mismo asset
//sin que se pisen: su controller fabricaria OTRA estrategia apuntando a el.
//
//FLUJO DEL COMPOSITE (como llegan a llamarse sus metodos):
//el controller NUNCA sabe que existe el composite; para el es un
//IPowerUpStrategy mas. La magia pasa en la fabricacion y en el foreach:
//1) Piso el pickup del combinado -> AddCharge recibe el CompositePowerUpConfig
//   (viaja como PowerUpConfig, el pickup y el controller no distinguen).
//2) AddCharge llama config.CreateStrategy(deps) como siempre, pero la version
//   del composite recorre sus _parts y le pide a CADA parte su estrategia
//   (pasandole las MISMAS deps, por eso todas apuntan al mismo user).
//   Con esa lista arma UNA CompositePowerUpStrategy y devuelve esa.
//3) Con Space, TryActivate llama Activate() sin saber que es un composite.
//   El Activate del composite hace foreach y llama el Activate de cada parte:
//   HealPowerUpStrategy cura (instantaneo) y SpeedPowerUpStrategy acelera.
//4) Cuando el timer llega a la Duration (LA DEL ASSET COMPOSITE, las de las
//   partes se ignoran), Update llama Deactivate() -> foreach -> Deactivate
//   de cada parte: Speed vuelve el multiplicador a 1 y Heal no hace nada.
//5) La muerte (Death -> ForceCancel) recorre el mismo camino que el punto 4.
