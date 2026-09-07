using HolyTac.Menu.Domain;

namespace HolyTac.Menu.Infrastructure;

internal class MenuItemRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public string Currency { get; set; } = default!;
    public int Category { get; set; }
    public bool IsSpicy { get; set; }
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }

    public MenuItem ToDomain() => MenuItem.Load(
        Id, Name, Description, Price, Currency, (MenuCategory)Category, IsSpicy, IsAvailable, ImageUrl);
}
