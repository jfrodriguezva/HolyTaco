using HolyTac.Promotions.Domain;
using MediatR;

namespace HolyTac.Promotions.Application.Commands;

public record CreatePromotionCommand(
    string Title,
    string Description,
    string? ImageUrl,
    DiscountType DiscountType,
    decimal? DiscountValue,
    decimal? ComboPrice,
    List<Guid> MenuItemIds,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    bool IsFeatured) : IRequest<Guid>;

public class CreatePromotionCommandHandler(IPromotionRepository repository) : IRequestHandler<CreatePromotionCommand, Guid>
{
    public async Task<Guid> Handle(CreatePromotionCommand request, CancellationToken cancellationToken)
    {
        var promotion = Promotion.Create(
            request.Title,
            request.Description,
            request.ImageUrl,
            request.DiscountType,
            request.DiscountValue,
            request.ComboPrice,
            request.MenuItemIds,
            request.StartsAtUtc,
            request.EndsAtUtc,
            request.IsFeatured);

        await repository.AddAsync(promotion, cancellationToken);
        return promotion.Id;
    }
}
