using HolyTac.Orders.Domain;

namespace HolyTac.Orders.Infrastructure;

internal class OrderRow
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public string? CustomerName { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

internal class OrderItemRow
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid MenuItemId { get; set; }
    public string MenuItemName { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }

    public OrderItem ToDomain() => OrderItem.Load(Id, MenuItemId, MenuItemName, UnitPrice, Quantity, Notes);
}

internal static class OrderMapper
{
    public static Order ToDomain(this OrderRow row, IEnumerable<OrderItemRow> items) => Order.Load(
        row.Id, row.TableNumber, row.CustomerName, (OrderStatus)row.Status, row.CreatedAtUtc,
        items.Select(i => i.ToDomain()));
}
