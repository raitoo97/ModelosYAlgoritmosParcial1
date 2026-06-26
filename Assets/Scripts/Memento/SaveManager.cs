using System.Collections.Generic;
using UnityEngine;
public enum SaveEvent
{
    Save,
    Load
}
public class SaveManager : MonoBehaviour , IObservable<SaveEvent>,IPauseable
{
    private List<IObserver<SaveEvent>> _myobservers = new List<IObserver<SaveEvent>>();
    public static SaveManager instance;
    private int savesCount;
    private int loadCounts;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        savesCount = 3;
        loadCounts = 0;
    }
    private void Update()
    {
        if (GameState.IsPaused) return;
        if (PlayerInputsManager.instance.SaveAction() && savesCount > 0)
        {
            savesCount--;
            loadCounts++;
            EventManager.TriggerEvent(EventType.UpdateSaves, savesCount);
            Save();
        }
        if (PlayerInputsManager.instance.LoadAction() && loadCounts > 0)
        {
            loadCounts--;
            Load();
        }
    }
    public void Save()
    {
        Debug.Log("Guardo");
        NotifyObservers(SaveEvent.Save);
    }
    public void Load()
    {
        Debug.Log("Cargo");
        NotifyObservers(SaveEvent.Load);
    }
    public void Subscribe(IObserver<SaveEvent> observer)
    {
        if (!_myobservers.Contains(observer))
        {
            _myobservers.Add(observer);
        }
    }
    public void Unsubscribe(IObserver<SaveEvent> observer)
    {
        if (_myobservers.Contains(observer))
        {
            _myobservers.Remove(observer);
        }
    }
    public void NotifyObservers(SaveEvent action)
    {
        for (int i = _myobservers.Count - 1; i >= 0; i--)
        {
            _myobservers[i].Notify(action);
        }
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