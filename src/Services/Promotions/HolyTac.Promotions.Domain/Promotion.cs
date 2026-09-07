using HolyTac.SharedKernel;

namespace HolyTac.Promotions.Domain;

public class Promotion : Entity<Guid>
{
    private readonly List<Guid> _menuItemIds = [];

    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string? ImageUrl { get; private set; }
    public DiscountType DiscountType { get; private set; }
    public decimal? DiscountValue { get; private set; }
    public decimal? ComboPrice { get; private set; }
    public IReadOnlyList<Guid> MenuItemIds => _menuItemIds.AsReadOnly();
    public DateTime StartsAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }
    public bool IsFeatured { get; private set; }
    public bool IsActive { get; private set; }

    public bool IsCurrentlyValid(DateTime nowUtc) => IsActive && nowUtc >= StartsAtUtc && nowUtc <= EndsAtUtc;

    private Promotion() { }

    public static Promotion Create(
        string title,
        string description,
        string? imageUrl,
        DiscountType discountType,
        decimal? discountValue,
        decimal? comboPrice,
        IEnumerable<Guid> menuItemIds,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        bool isFeatured)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("El título de la promoción es obligatorio.", nameof(title));

        if (endsAtUtc <= startsAtUtc)
            throw new ArgumentException("La fecha de fin debe ser posterior a la de inicio.", nameof(endsAtUtc));

        switch (discountType)
        {
            case DiscountType.ComboPrice when comboPrice is null or <= 0:
                throw new ArgumentException("Una promoción de combo requiere un precio de combo mayor a cero.", nameof(comboPrice));
            case DiscountType.Percentage or DiscountType.FixedAmount when discountValue is null or <= 0:
                throw new ArgumentException("El valor del descuento debe ser mayor a cero.", nameof(discountValue));
        }

        var promotion = new Promotion
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            ImageUrl = imageUrl,
            DiscountType = discountType,
            DiscountValue = discountValue,
            ComboPrice = comboPrice,
            StartsAtUtc = startsAtUtc,
            EndsAtUtc = endsAtUtc,
            IsFeatured = isFeatured,
            IsActive = true
        };
        promotion._menuItemIds.AddRange(menuItemIds);
        return promotion;
    }

    public static Promotion Load(
        Guid id, string title, string description, string? imageUrl, DiscountType discountType,
        decimal? discountValue, decimal? comboPrice, IEnumerable<Guid> menuItemIds,
        DateTime startsAtUtc, DateTime endsAtUtc, bool isFeatured, bool isActive)
    {
        var promotion = new Promotion
        {
            Id = id,
            Title = title,
            Description = description,
            ImageUrl = imageUrl,
            DiscountType = discountType,
            DiscountValue = discountValue,
            ComboPrice = comboPrice,
            StartsAtUtc = startsAtUtc,
            EndsAtUtc = endsAtUtc,
            IsFeatured = isFeatured,
            IsActive = isActive
        };
        promotion._menuItemIds.AddRange(menuItemIds);
        return promotion;
    }

    public void Deactivate() => IsActive = false;
}
