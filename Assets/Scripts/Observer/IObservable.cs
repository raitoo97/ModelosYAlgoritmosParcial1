using System;
public interface IObservable <in T> where T : Enum
{
    void Subscribe(IObserver observer);
    void Unsubscribe(IObserver observer);
    void NotifyObservers(T action);
}
