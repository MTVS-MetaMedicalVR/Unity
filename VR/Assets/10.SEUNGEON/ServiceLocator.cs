// ProcedureSystem/Core/Services/ServiceLocator.cs
using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public static void RegisterService<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    public static T GetService<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out object service))
        {
            return (T)service;
        }
        throw new System.Exception($"Service of type {typeof(T)} not registered");
    }

    public static void Clear()
    {
        _services.Clear();
    }
}
