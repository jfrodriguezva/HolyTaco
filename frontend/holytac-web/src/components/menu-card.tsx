"use client";

import type { MenuItemDto } from "@/lib/api";
import { useCart } from "@/lib/cart-context";

export function MenuCard({ item }: { item: MenuItemDto }) {
  const { addItem } = useCart();

  return (
    <article className="group flex flex-col justify-between rounded-2xl border border-agave/10 bg-white/60 p-5 shadow-sm transition-shadow hover:shadow-md">
      <div>
        <div className="flex items-start justify-between gap-3">
          <h3 className="font-display text-lg font-semibold text-ink">{item.name}</h3>
          {item.isSpicy && (
            <span title="Picante" className="text-lg leading-none text-chile">
              🌶
            </span>
          )}
        </div>
        <p className="mt-1 text-sm leading-relaxed text-ink-soft">{item.description}</p>
      </div>

      <div className="mt-4 flex items-center justify-between">
        <span className="font-display text-lg font-semibold text-terracota">
          ${item.price.toFixed(2)}
        </span>
        <button
          onClick={() => addItem(item)}
          disabled={!item.isAvailable}
          className="rounded-full bg-terracota px-4 py-1.5 text-sm font-semibold text-cream transition-colors hover:bg-terracota-dark disabled:cursor-not-allowed disabled:bg-ink-soft/30 disabled:text-ink-soft"
        >
          {item.isAvailable ? "Agregar" : "Agotado"}
        </button>
      </div>
    </article>
  );
}
