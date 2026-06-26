using System.Collections.Generic;
public class MementoState<T>
{
    private Stack<T> _stateStock;
    public int memoriesAmount => _stateStock.Count;
    public MementoState()
    {
        _stateStock = new Stack<T>();
    }
    public void SaveMemory(T memory)
    {
        _stateStock.Push(memory);
    }
    public T LoadMemory()
    {
        return _stateStock.Pop();
    }
}