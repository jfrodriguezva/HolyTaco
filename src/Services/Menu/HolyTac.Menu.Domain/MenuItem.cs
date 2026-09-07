using HolyTac.SharedKernel;

namespace HolyTac.Menu.Domain;

public class MenuItem : Entity<Guid>
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public Money Price { get; private set; }
    public MenuCategory Category { get; private set; }
    public bool IsSpicy { get; private set; }
    public bool IsAvailable { get; private set; }
    public string? ImageUrl { get; private set; }

    private MenuItem() { }

    public static MenuItem Create(
        string name,
        string description,
        Money price,
        MenuCategory category,
        bool isSpicy = false,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del platillo es obligatorio.", nameof(name));

        if (price.Amount <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.", nameof(price));

        return new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            Category = category,
            IsSpicy = isSpicy,
            IsAvailable = true,
            ImageUrl = imageUrl
        };
    }

    /// <summary>Reconstruye una instancia desde persistencia (Dapper no usa este dominio como modelo anémico de escritura).</summary>
    public static MenuItem Load(
        Guid id,
        string name,
        string description,
        decimal price,
        string currency,
        MenuCategory category,
        bool isSpicy,
        bool isAvailable,
        string? imageUrl) => new()
    {
        Id = id,
        Name = name,
        Description = description,
        Price = new Money(price, currency),
        Category = category,
        IsSpicy = isSpicy,
        IsAvailable = isAvailable,
        ImageUrl = imageUrl
    };

    public void MarkAsUnavailable() => IsAvailable = false;

    public void MarkAsAvailable() => IsAvailable = true;

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.", nameof(newPrice));

        Price = newPrice;
    }
}
