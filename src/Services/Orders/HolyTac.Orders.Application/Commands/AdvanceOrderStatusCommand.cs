using MediatR;

namespace HolyTac.Orders.Application.Commands;

public record AdvanceOrderStatusCommand(Guid OrderId) : IRequest;

public class AdvanceOrderStatusCommandHandler(IOrderRepository orderRepository) : IRequestHandler<AdvanceOrderStatusCommand>
{
    public async Task Handle(AdvanceOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró el pedido {request.OrderId}.");

        order.AdvanceStatus();
        await orderRepository.UpdateAsync(order, cancellationToken);
    }
}
