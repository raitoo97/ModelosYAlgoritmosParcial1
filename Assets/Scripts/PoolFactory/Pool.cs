using System;
using System.Collections.Generic;
public class Pool <T>
{
    private Func<T> _factoryMethod;
    private Action<T> _turnOnCallBack;
    private Action<T> _turnOffCallBack;
    private List<T> _currentStack;
    public Pool(Func<T> factoryMethod, Action<T> turnOnCallBack, Action<T> turnOffCallBack,int initSize)
    {
        _currentStack = new List<T>();
        _factoryMethod = factoryMethod;
        _turnOnCallBack = turnOnCallBack;
        _turnOffCallBack = turnOffCallBack;
        for (int i = 0; i < initSize; i++)
        {
            T obj = _factoryMethod();
            _turnOffCallBack(obj);
            _currentStack.Add(obj);
        }
    }
    public T GetObject()
    {
        T result = default(T);
        if(_currentStack.Count == 0)
        {
            result = _factoryMethod();
        }
        else
        {
            result = _currentStack[0];
            _currentStack.RemoveAt(0);
        }
        _turnOnCallBack(result);
        return result;
    }
    public void ReturnObject(T obj)
    {
        _turnOffCallBack(obj);
        _currentStack.Add(obj);
    }
}
