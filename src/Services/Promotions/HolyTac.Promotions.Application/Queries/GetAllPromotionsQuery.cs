using MediatR;

namespace HolyTac.Promotions.Application.Queries;

public record GetAllPromotionsQuery : IRequest<IReadOnlyList<PromotionDto>>;

public class GetAllPromotionsQueryHandler(IPromotionRepository repository)
    : IRequestHandler<GetAllPromotionsQuery, IReadOnlyList<PromotionDto>>
{
    public async Task<IReadOnlyList<PromotionDto>> Handle(GetAllPromotionsQuery request, CancellationToken cancellationToken)
    {
        var promotions = await repository.GetAllAsync(cancellationToken);
        return promotions
            .OrderByDescending(p => p.StartsAtUtc)
            .Select(PromotionDto.FromDomain)
            .ToList();
    }
}
