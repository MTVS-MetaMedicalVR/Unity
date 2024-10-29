// ProcedureSystem/Core/Events/EventManager.cs
using System.Collections.Generic;
using System;

public static class EventManager
{
    private static readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();

    public static void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type))
        {
            _handlers[type] = new List<Delegate>();
        }
        _handlers[type].Add(handler);
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (_handlers.ContainsKey(type))
        {
            _handlers[type].Remove(handler);
        }
    }

    public static void Publish<T>(T eventData)
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var handlers))
        {
            foreach (var handler in handlers.ToArray())
            {
                ((Action<T>)handler)(eventData);
            }
        }
    }

    public static void Clear()
    {
        _handlers.Clear();
    }
}