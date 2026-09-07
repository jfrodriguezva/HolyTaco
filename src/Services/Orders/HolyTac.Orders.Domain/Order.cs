using HolyTac.SharedKernel;

namespace HolyTac.Orders.Domain;

public class Order : Entity<Guid>
{
    private readonly List<OrderItem> _items = [];

    public int TableNumber { get; private set; }
    public string? CustomerName { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(i => i.Subtotal);

    private Order() { }

    public static Order Create(int tableNumber, string? customerName = null)
    {
        if (tableNumber <= 0)
            throw new ArgumentException("El número de mesa debe ser mayor a cero.", nameof(tableNumber));

        return new Order
        {
            Id = Guid.NewGuid(),
            TableNumber = tableNumber,
            CustomerName = customerName,
            Status = OrderStatus.Pendiente,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public static Order Load(Guid id, int tableNumber, string? customerName, OrderStatus status, DateTime createdAtUtc, IEnumerable<OrderItem> items)
    {
        var order = new Order
        {
            Id = id,
            TableNumber = tableNumber,
            CustomerName = customerName,
            Status = status,
            CreatedAtUtc = createdAtUtc
        };
        order._items.AddRange(items);
        return order;
    }

    public void AddItem(Guid menuItemId, string menuItemName, decimal unitPrice, int quantity, string? notes = null)
    {
        if (Status != OrderStatus.Pendiente)
            throw new InvalidOperationException("Solo se pueden agregar productos a un pedido pendiente.");

        _items.Add(OrderItem.Create(menuItemId, menuItemName, unitPrice, quantity, notes));
    }

    public void AdvanceStatus()
    {
        Status = Status switch
        {
            OrderStatus.Pendiente => OrderStatus.EnPreparacion,
            OrderStatus.EnPreparacion => OrderStatus.Listo,
            OrderStatus.Listo => OrderStatus.Entregado,
            _ => throw new InvalidOperationException($"No se puede avanzar un pedido en estado {Status}.")
        };
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Entregado or OrderStatus.Cancelado)
            throw new InvalidOperationException($"No se puede cancelar un pedido en estado {Status}.");

        Status = OrderStatus.Cancelado;
    }
}
