using HolyTac.Menu.Application;
using HolyTac.SharedKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HolyTac.Menu.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMenuInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MenuDb")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'MenuDb'.");

        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();

        return services;
    }
}
