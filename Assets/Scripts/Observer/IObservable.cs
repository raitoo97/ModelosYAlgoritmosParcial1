using System;
public interface IObservable<T> where T : Enum
{
    void Subscribe(IObserver<T> observer);
    void Unsubscribe(IObserver<T> observer);
    void NotifyObservers(T action);
}
