using Dapper;
using HolyTac.Promotions.Application;
using HolyTac.Promotions.Domain;
using HolyTac.SharedKernel;

namespace HolyTac.Promotions.Infrastructure;

public class PromotionRepository(IDbConnectionFactory connectionFactory) : IPromotionRepository
{
    public async Task<IReadOnlyList<Promotion>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                """
                SELECT Id, Title, Description, ImageUrl, DiscountType, DiscountValue, ComboPrice,
                       StartsAtUtc, EndsAtUtc, IsFeatured, IsActive
                FROM Promotions ORDER BY StartsAtUtc DESC;
                SELECT PromotionId, MenuItemId FROM PromotionMenuItems;
                """);

            var promoRows = (await multi.ReadAsync<PromotionRow>()).ToList();
            var itemLinks = (await multi.ReadAsync<PromotionMenuItemRow>()).ToLookup(l => l.PromotionId);

            return (IReadOnlyList<Promotion>)promoRows
                .Select(p => p.ToDomain(itemLinks[p.Id].Select(l => l.MenuItemId)))
                .ToList();
        }, cancellationToken);
    }

    public async Task<Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                """
                SELECT Id, Title, Description, ImageUrl, DiscountType, DiscountValue, ComboPrice,
                       StartsAtUtc, EndsAtUtc, IsFeatured, IsActive
                FROM Promotions WHERE Id = @Id;
                SELECT MenuItemId FROM PromotionMenuItems WHERE PromotionId = @Id;
                """,
                new { Id = id });

            var promo = await multi.ReadSingleOrDefaultAsync<PromotionRow>();
            if (promo is null) return null;

            var menuItemIds = await multi.ReadAsync<Guid>();
            return promo.ToDomain(menuItemIds);
        }, cancellationToken);
    }

    public async Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default)
    {
        await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(
                """
                INSERT INTO Promotions
                    (Id, Title, Description, ImageUrl, DiscountType, DiscountValue, ComboPrice,
                     StartsAtUtc, EndsAtUtc, IsFeatured, IsActive)
                VALUES
                    (@Id, @Title, @Description, @ImageUrl, @DiscountType, @DiscountValue, @ComboPrice,
                     @StartsAtUtc, @EndsAtUtc, @IsFeatured, @IsActive)
                """,
                new
                {
                    promotion.Id,
                    promotion.Title,
                    promotion.Description,
                    promotion.ImageUrl,
                    DiscountType = (int)promotion.DiscountType,
                    promotion.DiscountValue,
                    promotion.ComboPrice,
                    promotion.StartsAtUtc,
                    promotion.EndsAtUtc,
                    promotion.IsFeatured,
                    promotion.IsActive
                },
                transaction);

            foreach (var menuItemId in promotion.MenuItemIds)
            {
                await connection.ExecuteAsync(
                    "INSERT INTO PromotionMenuItems (PromotionId, MenuItemId) VALUES (@PromotionId, @MenuItemId)",
                    new { PromotionId = promotion.Id, MenuItemId = menuItemId },
                    transaction);
            }

            transaction.Commit();
        }, cancellationToken);
    }

    public async Task UpdateActiveFlagAsync(Promotion promotion, CancellationToken cancellationToken = default)
    {
        await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                "UPDATE Promotions SET IsActive = @IsActive WHERE Id = @Id",
                new { promotion.Id, promotion.IsActive });
        }, cancellationToken);
    }
}
