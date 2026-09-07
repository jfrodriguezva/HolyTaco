using HolyTac.Promotions.Domain;

namespace HolyTac.Promotions.Application;

public record PromotionDto(
    Guid Id,
    string Title,
    string Description,
    string? ImageUrl,
    string DiscountType,
    decimal? DiscountValue,
    decimal? ComboPrice,
    IReadOnlyList<Guid> MenuItemIds,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    bool IsFeatured,
    bool IsActive,
    bool IsCurrentlyValid)
{
    public static PromotionDto FromDomain(Promotion p) => new(
        p.Id, p.Title, p.Description, p.ImageUrl, p.DiscountType.ToString(), p.DiscountValue, p.ComboPrice,
        p.MenuItemIds, p.StartsAtUtc, p.EndsAtUtc, p.IsFeatured, p.IsActive, p.IsCurrentlyValid(DateTime.UtcNow));
}
