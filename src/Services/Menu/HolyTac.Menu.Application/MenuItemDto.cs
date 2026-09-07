using HolyTac.Menu.Domain;

namespace HolyTac.Menu.Application;

public record MenuItemDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string Category,
    bool IsSpicy,
    bool IsAvailable,
    string? ImageUrl)
{
    public static MenuItemDto FromDomain(MenuItem item) => new(
        item.Id,
        item.Name,
        item.Description,
        item.Price.Amount,
        item.Price.Currency,
        item.Category.ToString(),
        item.IsSpicy,
        item.IsAvailable,
        item.ImageUrl);
}
