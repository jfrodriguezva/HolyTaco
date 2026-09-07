using System.Net.Http.Json;
using HolyTac.Orders.Application;

namespace HolyTac.Orders.Infrastructure;

/// <summary>
/// Cliente HTTP hacia el microservicio de Menú. La política de reintentos y el circuit breaker
/// se configuran a nivel de HttpClient con Microsoft.Extensions.Http.Resilience (Polly) en DependencyInjection.
/// </summary>
public class MenuCatalogClient(HttpClient httpClient) : IMenuCatalogClient
{
    public async Task<MenuCatalogItem?> GetItemAsync(Guid menuItemId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"api/menu/{menuItemId}", cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var dto = await response.Content.ReadFromJsonAsync<MenuItemResponse>(cancellationToken: cancellationToken);
        return dto is null ? null : new MenuCatalogItem(dto.Id, dto.Name, dto.Price, dto.IsAvailable);
    }

    private record MenuItemResponse(Guid Id, string Name, decimal Price, bool IsAvailable);
}
