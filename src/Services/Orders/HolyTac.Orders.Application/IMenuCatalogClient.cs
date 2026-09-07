namespace HolyTac.Orders.Application;

public record MenuCatalogItem(Guid Id, string Name, decimal Price, bool IsAvailable);

/// <summary>Cliente hacia el microservicio de Menú (HTTP + Polly), consumido por Orders para validar productos.</summary>
public interface IMenuCatalogClient
{
    Task<MenuCatalogItem?> GetItemAsync(Guid menuItemId, CancellationToken cancellationToken = default);
}
