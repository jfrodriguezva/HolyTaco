"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { promotionsApi, type PromotionDto } from "@/lib/api";
import { PromotionCard } from "@/components/promotion-card";

export default function PromocionesPage() {
  const [promotions, setPromotions] = useState<PromotionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    promotionsApi
      .getActive()
      .then((data) => {
        if (!cancelled) setPromotions(data);
      })
      .catch(() => {
        if (!cancelled) setError("No pudimos cargar las promociones en este momento.");
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  return (
    <div className="mx-auto max-w-6xl px-6 py-14">
      <header className="mb-10 text-center">
        <span className="rounded-full bg-terracota/10 px-4 py-1 text-xs font-semibold uppercase tracking-[0.2em] text-terracota">
          Ofertas de la casa
        </span>
        <h1 className="font-display mt-3 text-4xl font-semibold text-agave">Promociones</h1>
        <p className="mt-2 text-ink-soft">Combos, happy hour y descuentos vigentes esta temporada.</p>
      </header>

      {loading && <p className="text-center text-ink-soft">Cargando promociones…</p>}
      {error && <p className="text-center text-terracota-dark">{error}</p>}

      {!loading && !error && (
        <>
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {promotions.map((promo) => (
              <PromotionCard key={promo.id} promotion={promo} />
            ))}
          </div>
          {promotions.length === 0 && (
            <p className="text-center text-ink-soft">
              Por ahora no hay promociones activas. Vuelve pronto o{" "}
              <Link href="/reservar" className="font-semibold text-terracota">
                reserva tu mesa
              </Link>
              .
            </p>
          )}
        </>
      )}
    </div>
  );
}
