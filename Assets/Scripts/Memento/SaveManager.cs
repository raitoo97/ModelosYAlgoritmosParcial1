using UnityEngine;
public enum SaveEvent
{
    Save,
    Load
}
public class SaveManager : MonoBehaviour , IObservable<SaveEvent>,IPauseable
{
    private ObserverList<SaveEvent> _saveObservers = new ObserverList<SaveEvent>();
    public static SaveManager instance;
    private int _savesCount = 3;
    private int _loadCounts = 0;
    private bool _playerIsDead = false;
    public bool CanSave => _savesCount > 0 && !_playerIsDead;
    public bool CanLoad => _loadCounts > 0 && !_playerIsDead;
    private IController _controller;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        _controller = new SaveController();
        EventManager.SubscribeToEvent(EventType.PlayerDeath, OnPlayerDeath);
    }
    private void OnPlayerDeath(params object[] parameters)
    {
        _playerIsDead = true;
    }
    private void Update()
    {
        _controller.UpdateInputs();
    }
    public void Save()
    {
        _savesCount--;
        _loadCounts++;
        EventManager.TriggerEvent(EventType.UpdateSaves, _savesCount);
        EventManager.TriggerEvent(EventType.UpdateLoads, _loadCounts);
        NotifyObservers(SaveEvent.Save);
    }
    public void Load()
    {
        _loadCounts--;
        EventManager.TriggerEvent(EventType.UpdateLoads, _loadCounts);
        NotifyObservers(SaveEvent.Load);
    }
    public void Subscribe(IObserver<SaveEvent> observer)
    {
        _saveObservers.Subscribe(observer);
    }
    public void Unsubscribe(IObserver<SaveEvent> observer)
    {
        _saveObservers.Unsubscribe(observer);
    }
    public void NotifyObservers(SaveEvent action)
    {
        _saveObservers.NotifyObservers(action);
    }
    public void Pause()
    {
        enabled = false;
    }
    public void Resume()
    {
        enabled = true;
    }
    private void OnDestroy()
    {
        EventManager.UnsubscribeToEvent(EventType.PlayerDeath, OnPlayerDeath);
    }
}