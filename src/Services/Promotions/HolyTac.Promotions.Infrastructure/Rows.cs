using HolyTac.Promotions.Domain;

namespace HolyTac.Promotions.Infrastructure;

internal class PromotionRow
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public int DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal? ComboPrice { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }

    public Promotion ToDomain(IEnumerable<Guid> menuItemIds) => Promotion.Load(
        Id, Title, Description, ImageUrl, (DiscountType)DiscountType, DiscountValue, ComboPrice,
        menuItemIds, StartsAtUtc, EndsAtUtc, IsFeatured, IsActive);
}

internal class PromotionMenuItemRow
{
    public Guid PromotionId { get; set; }
    public Guid MenuItemId { get; set; }
}
