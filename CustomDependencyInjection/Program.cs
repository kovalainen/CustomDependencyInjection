// See https://aka.ms/new-console-template for more information

namespace CustomDependencyInjection;

public interface ISingletonService
{
    DateTime GetCreationDateTime();
}

public class SingletonService : ISingletonService
{
    private readonly DateTime _creationDateTime = DateTime.Now;
    
    public DateTime GetCreationDateTime()
    {
        return _creationDateTime;
    }
}

public interface IService
{
    DateTime GetCreationDateTime();
}

public class Service : IService
{
    private readonly ISingletonService _singletonService;

    public Service(ISingletonService singletonService)
    {
        _singletonService = singletonService;
    }

    public DateTime GetCreationDateTime()
    {
        return _singletonService.GetCreationDateTime();
    }
}

public static class Program
{
    public static async Task Main()
    {
        var serviceContainer = new ServiceContainer();
        
        serviceContainer.AddService<IService, Service>();
        serviceContainer.AddSingleton<ISingletonService, SingletonService>();

        var service = serviceContainer.GetService<IService>();

        Console.WriteLine(service?.GetCreationDateTime());

        await Task.Delay(2000);
        
        service = serviceContainer.GetService<IService>();
        
        Console.WriteLine(service?.GetCreationDateTime());
    }
}