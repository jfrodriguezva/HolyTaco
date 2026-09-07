using MediatR;

namespace HolyTac.Promotions.Application.Queries;

public record GetPromotionByIdQuery(Guid Id) : IRequest<PromotionDto?>;

public class GetPromotionByIdQueryHandler(IPromotionRepository repository)
    : IRequestHandler<GetPromotionByIdQuery, PromotionDto?>
{
    public async Task<PromotionDto?> Handle(GetPromotionByIdQuery request, CancellationToken cancellationToken)
    {
        var promotion = await repository.GetByIdAsync(request.Id, cancellationToken);
        return promotion is null ? null : PromotionDto.FromDomain(promotion);
    }
}
