using MediatR;

namespace HolyTac.Orders.Application.Queries;

public record GetActiveOrdersQuery : IRequest<IReadOnlyList<OrderDto>>;

public class GetActiveOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetActiveOrdersQuery, IReadOnlyList<OrderDto>>
{
    public async Task<IReadOnlyList<OrderDto>> Handle(GetActiveOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetActiveOrdersAsync(cancellationToken);
        return orders.Select(OrderDto.FromDomain).ToList();
    }
}
