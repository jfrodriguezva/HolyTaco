import type { PromotionDto } from "@/lib/api";
import { formatShortDate } from "@/lib/datetime";

function discountLabel(promo: PromotionDto): string {
  switch (promo.discountType) {
    case "ComboPrice":
      return `$${promo.comboPrice?.toFixed(2)}`;
    case "Percentage":
      return `${promo.discountValue}% OFF`;
    case "FixedAmount":
      return `-$${promo.discountValue?.toFixed(2)}`;
    default:
      return "";
  }
}

function formatDateRange(_startsAtUtc: string, endsAtUtc: string): string {
  return `Vigente hasta el ${formatShortDate(endsAtUtc)}`;
}

export function PromotionCard({ promotion }: { promotion: PromotionDto }) {
  return (
    <article className="flex flex-col justify-between overflow-hidden rounded-2xl border border-agave/10 bg-white/60 shadow-sm transition-shadow hover:shadow-md">
      <div className="h-2 bg-chile" />
      <div className="flex flex-1 flex-col gap-3 p-6">
        <div className="flex items-start justify-between gap-3">
          <h3 className="font-display text-xl font-semibold text-agave">{promotion.title}</h3>
          <span className="whitespace-nowrap rounded-full bg-terracota px-3 py-1 text-sm font-bold text-cream">
            {discountLabel(promotion)}
          </span>
        </div>
        <p className="flex-1 text-sm leading-relaxed text-ink-soft">{promotion.description}</p>
        <p className="text-xs font-medium uppercase tracking-wide text-chile">
          {formatDateRange(promotion.startsAtUtc, promotion.endsAtUtc)}
        </p>
      </div>
    </article>
  );
}
