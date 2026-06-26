using System.Collections.Generic;
using UnityEngine;
public enum SaveEvent
{
    Save,
    Load
}
public class SaveManager : MonoBehaviour , IObservable<SaveEvent>
{
    private List<IObserver<SaveEvent>> _myobservers = new List<IObserver<SaveEvent>>();
    public static SaveManager instance;
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
    }
    private void Update()
    {
        if (PlayerInputsManager.instance.SaveAction())
            Save();
        if (PlayerInputsManager.instance.LoadAction())
            Load();
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
}