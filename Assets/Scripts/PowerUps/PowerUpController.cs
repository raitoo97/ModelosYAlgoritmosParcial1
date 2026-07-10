using System;
using System.Collections.Generic;
using UnityEngine;
// Runner GENERICO de power ups: duenio de las cargas, el timer y el estado activo.
// Las estrategias son efectos puros; este componente decide CUANDO corren.
// En el Player se activa via PlayerEvent (observer),
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
    // El duenio (Player o enemigo) arma las dependencias una sola vez.
    public void Init(GameObject user)
    {
        _deps = new PowerUpStrategyDependencies { user = user };
        FillDictionary();
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.UsePowerUp, () => TryActivate());
        _actions.Add(PlayerEvent.Death, ForceCancel);
    }
    // Llega desde un pickup del mapa. Si es el mismo power up suma carga;
    // si es uno distinto lo equipa y arranca con una carga.
    // Con el efecto corriendo NO se permite cambiar de tipo (la estrategia
    // que esta activa es la que debe apagarse despues); en ese caso el
    // pickup no se consume y queda en el mapa.
    public bool AddCharge(PowerUpConfig config)
    {
        if (config == null) return false;
        if (config == _config)
        {
            _charges++;
            return true;
        }
        if (_isActive) return false;
        _config = config;
        _strategy = config.CreateStrategy(_deps);
        Debug.Log("Se creo la estrategia: " + _strategy.GetType().Name);
        _charges = 1;
        return true;
    }
    // Consume una carga y prende el efecto. Devuelve bool para que
    // una IA (o una UI) sepa si pudo activarlo.
    public bool TryActivate()
    {
        if (_isActive || _charges <= 0 || _strategy == null) return false;
        _charges--;
        _timer = 0;
        _isActive = true;
        _strategy.Activate();
        return true;
    }
    // Timer acumulado en Update y NO con corutinas: la pausa del juego no usa
    // timeScale, apaga los IPauseable (enabled = false) y eso congela este
    // Update. Una corutina o Time.time seguirian corriendo en pausa.
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
    // Corte inmediato (muerte del duenio): apaga el efecto sin esperar el timer.
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
