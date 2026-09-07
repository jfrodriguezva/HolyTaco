using HolyTac.Promotions.Application;
using HolyTac.SharedKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HolyTac.Promotions.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPromotionsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PromotionsDb")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'PromotionsDb'.");

        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        services.AddScoped<IPromotionRepository, PromotionRepository>();

        return services;
    }
}
