using Microsoft.Extensions.DependencyInjection;

namespace HolyTac.Reservations.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddReservationsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddScoped<AvailabilityService>();
        return services;
    }
}
