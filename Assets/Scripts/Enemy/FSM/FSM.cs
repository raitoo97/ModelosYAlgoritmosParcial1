using System.Collections.Generic;
public class FSM
{
    public enum StateID
    {
        Chase,
        Attack,
    }
    private Dictionary<StateID, IState> _allStates = new Dictionary<StateID, IState>();
    private IState _currentState;
    public StateID currentStateID { get; private set; }
    public void AddState(StateID key, IState value)
    {
        if (_allStates.ContainsKey(key)) return;
        _allStates.Add(key, value);
    }
    public void RemoveState(StateID key)
    {
        if (!_allStates.ContainsKey(key)) return;
        _allStates.Remove(key);
    }
    public void ChangeState(StateID key)
    {
        if (!_allStates.ContainsKey(key)) return;
        _currentState?.OnExit();
        currentStateID = key;
        _currentState = _allStates[key];
        _currentState?.OnEnter();
    }
    public void OnUpdateState()
    {
        _currentState?.OnUpdate();
    }
}
