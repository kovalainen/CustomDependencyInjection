namespace CustomDependencyInjection;

public class ServiceContainer
{
    private readonly Dictionary<Type, Func<object?>?> _serviceDictionary = new();
    private readonly Dictionary<Type, Func<object?>?> _singletonDictionary = new();

    public void AddService<TService, TImpl>() where TImpl : class, TService
    {
        _serviceDictionary.Add(typeof(TService), CreateServiceInstance<TImpl>);
    }

    public void AddService<TService>(Func<TService> factory) where TService : class
    {
        _serviceDictionary.Add(typeof(TService), factory);
    }
    
    public void AddSingleton<TService>(Func<TService> factory) where TService : class
    {
        var lazy = new Lazy<TService?>(factory);
        _singletonDictionary.Add(typeof(TService), () => lazy.Value);
    }
    
    public void AddSingleton<TService, TImpl>() where TImpl : class, TService
    {
        var lazy = new Lazy<TImpl?>(() => CreateSingletonInstance<TImpl>() as TImpl);
        _singletonDictionary.Add(typeof(TService), () => lazy.Value);
    }

    public T? GetService<T>() where T : class
    {
        var type = typeof(T);

        return GetService(type) as T;
    }

    private object? GetService(Type type)
    {
        if (_serviceDictionary.TryGetValue(type, out var factory))
            return factory!();
        if (_singletonDictionary.TryGetValue(type, out factory))
            return factory!();
        throw new Exception("No registration for " + type);
    }

    private object? CreateServiceInstance<T>() where T : class
    {
        var type = typeof(T);

        var ctor = type.GetConstructors().Single();

        var dependencies = ctor
            .GetParameters()
            .Select(p => GetService(p.ParameterType))
            .ToArray();

        return Activator.CreateInstance(type, dependencies);
    }

    private object? CreateSingletonInstance<T>() where T : class
    {
        var type = typeof(T);

        var ctor = type.GetConstructors().Single();

        var dependencies = ctor
            .GetParameters()
            .Select(p =>
            {
                if (_singletonDictionary.TryGetValue(p.ParameterType, out var factory))
                {
                    return factory!();
                }

                throw new Exception("No registration for " + type);
            })
            .ToArray();

        return Activator.CreateInstance(type, dependencies);
    }
}
