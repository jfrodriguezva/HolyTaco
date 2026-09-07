using HolyTac.SharedKernel;

namespace HolyTac.Orders.Domain;

public class OrderItem : Entity<Guid>
{
    public Guid MenuItemId { get; private set; }
    public string MenuItemName { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public string? Notes { get; private set; }

    public decimal Subtotal => UnitPrice * Quantity;

    private OrderItem() { }

    public static OrderItem Create(Guid menuItemId, string menuItemName, decimal unitPrice, int quantity, string? notes = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));

        return new OrderItem
        {
            Id = Guid.NewGuid(),
            MenuItemId = menuItemId,
            MenuItemName = menuItemName,
            UnitPrice = unitPrice,
            Quantity = quantity,
            Notes = notes
        };
    }

    public static OrderItem Load(Guid id, Guid menuItemId, string menuItemName, decimal unitPrice, int quantity, string? notes)
        => new()
        {
            Id = id,
            MenuItemId = menuItemId,
            MenuItemName = menuItemName,
            UnitPrice = unitPrice,
            Quantity = quantity,
            Notes = notes
        };
}
