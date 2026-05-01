using System.Collections.Generic;
public enum EventType
{
    PlayerDeath,
}
public static class EventManager
{
    public delegate void MethodToSuscribe(params object[] parameters);
    public static Dictionary<EventType, MethodToSuscribe> _events;
    public static void SubscribeToEvent(EventType eventType, MethodToSuscribe method)
    {
        if (_events == null) _events = new Dictionary<EventType, MethodToSuscribe>();
        if(!_events.ContainsKey(eventType))
            _events.Add(eventType, method);
        _events[eventType] += method;
    }
    public static void UnsubscribeToEvent(EventType eventType, MethodToSuscribe method)
    {
        if (_events == null) return;
        if(!_events.ContainsKey(eventType)) return;
        _events[eventType] -= method;
    }
    public static void TriggerEvent(EventType eventType, params object[] parameters)
    {
        if (_events == null) return;
        if(!_events.ContainsKey(eventType)) return;
        if (_events[eventType] == null) return;
        _events[eventType].Invoke(parameters);
    }
}
