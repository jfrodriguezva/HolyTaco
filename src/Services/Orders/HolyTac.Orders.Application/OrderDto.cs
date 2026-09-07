using HolyTac.Orders.Domain;

namespace HolyTac.Orders.Application;

public record OrderItemDto(Guid Id, Guid MenuItemId, string MenuItemName, decimal UnitPrice, int Quantity, decimal Subtotal, string? Notes)
{
    public static OrderItemDto FromDomain(OrderItem item) => new(
        item.Id, item.MenuItemId, item.MenuItemName, item.UnitPrice, item.Quantity, item.Subtotal, item.Notes);
}

public record OrderDto(
    Guid Id,
    int TableNumber,
    string? CustomerName,
    string Status,
    DateTime CreatedAtUtc,
    decimal Total,
    IReadOnlyList<OrderItemDto> Items)
{
    public static OrderDto FromDomain(Order order) => new(
        order.Id,
        order.TableNumber,
        order.CustomerName,
        order.Status.ToString(),
        order.CreatedAtUtc,
        order.Total,
        order.Items.Select(OrderItemDto.FromDomain).ToList());
}
