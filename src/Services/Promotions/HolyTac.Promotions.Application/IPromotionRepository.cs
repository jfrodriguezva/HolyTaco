using HolyTac.Promotions.Domain;

namespace HolyTac.Promotions.Application;

public interface IPromotionRepository
{
    Task<IReadOnlyList<Promotion>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default);
    Task UpdateActiveFlagAsync(Promotion promotion, CancellationToken cancellationToken = default);
}
