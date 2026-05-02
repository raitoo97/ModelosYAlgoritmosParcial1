using System.Collections.Generic;
public enum EventType
{
    PlayerDeath,
}
public static class EventManager
{
    public delegate void EventDelegate(params object[] parameters);
    private static readonly Dictionary<EventType, EventDelegate> _events = new Dictionary<EventType, EventDelegate>();
    public static void SubscribeToEvent(EventType eventType, EventDelegate method)
    {
        if(!_events.ContainsKey(eventType))
            _events.Add(eventType, method);
        else
            _events[eventType] += method;
    }
    public static void UnsubscribeToEvent(EventType eventType, EventDelegate method)
    {
        if (!_events.ContainsKey(eventType)) return;
        _events[eventType] -= method;
        if (_events[eventType] == null)
            _events.Remove(eventType);
    }
    public static void TriggerEvent(EventType eventType, params object[] parameters)
    {
        if (_events.TryGetValue(eventType, out EventDelegate _event))
        {
            _event?.Invoke(parameters);
        }
    }
}