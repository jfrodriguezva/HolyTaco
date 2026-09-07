using MediatR;

namespace HolyTac.Orders.Application.Commands;

public record CancelOrderCommand(Guid OrderId) : IRequest;

public class CancelOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<CancelOrderCommand>
{
    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró el pedido {request.OrderId}.");

        order.Cancel();
        await orderRepository.UpdateAsync(order, cancellationToken);
    }
}
