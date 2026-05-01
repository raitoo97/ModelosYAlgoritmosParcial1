using System;
public interface IObserver<in T> where T : Enum
{
    void Notify(T Actions);
}
