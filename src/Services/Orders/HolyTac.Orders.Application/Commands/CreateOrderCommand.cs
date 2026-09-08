using HolyTac.Orders.Domain;
using MediatR;

namespace HolyTac.Orders.Application.Commands;

public record CreateOrderItemRequest(Guid MenuItemId, int Quantity, string? Notes);

public record CreateOrderCommand(int TableNumber, string? CustomerName, List<CreateOrderItemRequest> Items) : IRequest<Guid>;

public class CreateOrderCommandHandler(IOrderRepository orderRepository, IMenuCatalogClient menuCatalogClient)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.Create(request.TableNumber, request.CustomerName);

        // Se validan todas las líneas contra Menu en paralelo (en vez de una petición HTTP a la
        // vez) para no pagar N round-trips secuenciales en pedidos con varios productos.
        var menuItems = await Task.WhenAll(
            request.Items.Select(line => menuCatalogClient.GetItemAsync(line.MenuItemId, cancellationToken)));

        for (var i = 0; i < request.Items.Count; i++)
        {
            var line = request.Items[i];
            var menuItem = menuItems[i]
                ?? throw new InvalidOperationException($"El producto {line.MenuItemId} no existe en el menú.");

            if (!menuItem.IsAvailable)
                throw new InvalidOperationException($"El producto '{menuItem.Name}' no está disponible.");

            order.AddItem(menuItem.Id, menuItem.Name, menuItem.Price, line.Quantity, line.Notes);
        }

        await orderRepository.AddAsync(order, cancellationToken);
        return order.Id;
    }
}
