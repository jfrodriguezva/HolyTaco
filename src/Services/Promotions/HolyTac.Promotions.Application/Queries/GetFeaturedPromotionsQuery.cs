using MediatR;

namespace HolyTac.Promotions.Application.Queries;

public record GetFeaturedPromotionsQuery(int Take = 3) : IRequest<IReadOnlyList<PromotionDto>>;

public class GetFeaturedPromotionsQueryHandler(IPromotionRepository repository)
    : IRequestHandler<GetFeaturedPromotionsQuery, IReadOnlyList<PromotionDto>>
{
    public async Task<IReadOnlyList<PromotionDto>> Handle(GetFeaturedPromotionsQuery request, CancellationToken cancellationToken)
    {
        var promotions = await repository.GetAllAsync(cancellationToken);
        var now = DateTime.UtcNow;
        return promotions
            .Where(p => p.IsFeatured && p.IsCurrentlyValid(now))
            .OrderBy(p => p.EndsAtUtc)
            .Take(request.Take)
            .Select(PromotionDto.FromDomain)
            .ToList();
    }
}
