using System.Net;
using System.Net.Http.Json;
using HolyTac.Orders.Application;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace HolyTac.Orders.Infrastructure;

/// <summary>
/// Cliente HTTP hacia el microservicio de Menú. La política de reintentos y el circuit breaker
/// se configuran a nivel de HttpClient con Microsoft.Extensions.Http.Resilience (Polly) en DependencyInjection.
/// </summary>
public class MenuCatalogClient(HttpClient httpClient) : IMenuCatalogClient
{
    public async Task<MenuCatalogItem?> GetItemAsync(Guid menuItemId, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync($"api/menu/{menuItemId}", cancellationToken);
        }
        catch (Exception ex) when (
            ex is BrokenCircuitException or TimeoutRejectedException or HttpRequestException
            // TaskCanceledException también la lanza el propio HttpClient.Timeout cuando Menu no
            // responde nada (p. ej. está caído). Solo se re-clasifica así cuando NO fue el caller
            // quien canceló la petición; si sí lo fue, se deja propagar tal cual.
            || (ex is TaskCanceledException && !cancellationToken.IsCancellationRequested))
        {
            throw new MenuServiceUnavailableException("El servicio de menú no está disponible en este momento.", ex);
        }

        // Un 404 real significa "el producto no existe"; cualquier otro código de error es una
        // falla del servicio de Menú, no debe confundirse con lo primero.
        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        if (!response.IsSuccessStatusCode)
            throw new MenuServiceUnavailableException(
                $"El servicio de menú respondió con un error inesperado ({(int)response.StatusCode}).");

        var dto = await response.Content.ReadFromJsonAsync<MenuItemResponse>(cancellationToken: cancellationToken);
        return dto is null ? null : new MenuCatalogItem(dto.Id, dto.Name, dto.Price, dto.IsAvailable);
    }

    private record MenuItemResponse(Guid Id, string Name, decimal Price, bool IsAvailable);
}
