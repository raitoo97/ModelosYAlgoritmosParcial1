using System;
using System.Collections.Generic;
public class ObserverList<T> where T : Enum
{
    private List<IObserver<T>> _observers = new();
    public void Subscribe(IObserver<T> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }
    public void Unsubscribe(IObserver<T> observer)
    {
        if (_observers.Contains(observer))
            _observers.Remove(observer);
    }
    public void NotifyObservers(T action)
    {
        for (int i = _observers.Count - 1; i >= 0; i--)
            _observers[i].Notify(action);
    }
}
