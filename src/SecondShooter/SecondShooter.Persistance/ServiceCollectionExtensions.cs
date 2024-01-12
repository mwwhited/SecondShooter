using Microsoft.Extensions.DependencyInjection;

namespace SecondShooter.Persistance;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection TryAddPersistanceServices(this IServiceCollection services)
    {
        return services;
    }
}
