using MediatR;

namespace HolyTac.Promotions.Application.Queries;

public record GetActivePromotionsQuery : IRequest<IReadOnlyList<PromotionDto>>;

public class GetActivePromotionsQueryHandler(IPromotionRepository repository)
    : IRequestHandler<GetActivePromotionsQuery, IReadOnlyList<PromotionDto>>
{
    public async Task<IReadOnlyList<PromotionDto>> Handle(GetActivePromotionsQuery request, CancellationToken cancellationToken)
    {
        var promotions = await repository.GetAllAsync(cancellationToken);
        var now = DateTime.UtcNow;
        return promotions
            .Where(p => p.IsCurrentlyValid(now))
            .OrderBy(p => p.EndsAtUtc)
            .Select(PromotionDto.FromDomain)
            .ToList();
    }
}
