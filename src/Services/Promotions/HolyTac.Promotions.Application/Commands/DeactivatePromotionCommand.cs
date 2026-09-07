using MediatR;

namespace HolyTac.Promotions.Application.Commands;

public record DeactivatePromotionCommand(Guid PromotionId) : IRequest;

public class DeactivatePromotionCommandHandler(IPromotionRepository repository) : IRequestHandler<DeactivatePromotionCommand>
{
    public async Task Handle(DeactivatePromotionCommand request, CancellationToken cancellationToken)
    {
        var promotion = await repository.GetByIdAsync(request.PromotionId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró la promoción {request.PromotionId}.");

        promotion.Deactivate();
        await repository.UpdateActiveFlagAsync(promotion, cancellationToken);
    }
}
