using Microsoft.Extensions.DependencyInjection;

namespace HolyTac.Menu.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMenuApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        return services;
    }
}
