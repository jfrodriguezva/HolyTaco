using HolyTac.Orders.Application;
using HolyTac.SharedKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace HolyTac.Orders.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrdersInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OrdersDb")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'OrdersDb'.");

        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        services.AddScoped<IOrderRepository, OrderRepository>();

        var menuServiceBaseUrl = configuration["Services:MenuApiBaseUrl"]
            ?? throw new InvalidOperationException("No se encontró la configuración 'Services:MenuApiBaseUrl'.");

        services.AddHttpClient<IMenuCatalogClient, MenuCatalogClient>(client =>
            {
                client.BaseAddress = new Uri(menuServiceBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddResilienceHandler("menu-catalog-pipeline", builder =>
            {
                builder.AddRetry(new Polly.Retry.RetryStrategyOptions<HttpResponseMessage>
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromMilliseconds(200)
                });
                builder.AddCircuitBreaker(new Polly.CircuitBreaker.CircuitBreakerStrategyOptions<HttpResponseMessage>
                {
                    FailureRatio = 0.5,
                    MinimumThroughput = 5,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    BreakDuration = TimeSpan.FromSeconds(15)
                });
                builder.AddTimeout(TimeSpan.FromSeconds(5));
            });

        return services;
    }
}
