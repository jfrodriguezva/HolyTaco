"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { promotionsApi, type PromotionDto } from "@/lib/api";
import { PromotionCard } from "@/components/promotion-card";

export default function Home() {
  const [featured, setFeatured] = useState<PromotionDto[]>([]);

  useEffect(() => {
    let cancelled = false;
    promotionsApi
      .getFeatured()
      .then((data) => {
        if (!cancelled) setFeatured(data);
      })
      .catch(() => {
        if (!cancelled) setFeatured([]);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  return (
    <div>
      <section className="relative overflow-hidden bg-agave text-cream">
        <div className="bg-noise absolute inset-0 opacity-20" />
        <div className="relative mx-auto flex max-w-6xl flex-col items-start gap-6 px-6 py-28">
          <span className="rounded-full border border-cream/30 px-4 py-1 text-xs font-semibold uppercase tracking-[0.2em] text-cream/80">
            Tacos · Bar · Antro
          </span>
          <h1 className="font-display max-w-2xl text-5xl font-semibold italic leading-[1.05] sm:text-6xl">
            Reserva tu mesa,
            <br /> vive la noche.
          </h1>
          <p className="max-w-lg text-lg text-cream/85">
            Tacos de autor, cantina y coctelería en un mismo lugar. Aparta tu mesa en segundos y entérate
            de las promociones de la semana antes que nadie.
          </p>
          <div className="flex flex-wrap gap-4 pt-2">
            <Link
              href="/reservar"
              className="rounded-full bg-terracota px-7 py-3 text-sm font-semibold uppercase tracking-wide text-cream transition-colors hover:bg-terracota-dark"
            >
              Reservar mesa
            </Link>
            <Link
              href="/promociones"
              className="rounded-full border border-cream/40 px-7 py-3 text-sm font-semibold uppercase tracking-wide text-cream transition-colors hover:bg-cream/10"
            >
              Ver promociones
            </Link>
          </div>
        </div>
      </section>

      {featured.length > 0 && (
        <section className="mx-auto max-w-6xl px-6 py-20">
          <div className="mb-8 flex items-end justify-between">
            <div>
              <span className="text-xs font-semibold uppercase tracking-[0.2em] text-terracota">
                Esta semana
              </span>
              <h2 className="font-display mt-1 text-3xl font-semibold text-agave">Promociones destacadas</h2>
            </div>
            <Link href="/promociones" className="hidden text-sm font-semibold text-terracota sm:block">
              Ver todas →
            </Link>
          </div>
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {featured.map((promo) => (
              <PromotionCard key={promo.id} promotion={promo} />
            ))}
          </div>
        </section>
      )}

      <section className="bg-cream-dark/60 py-20">
        <div className="mx-auto grid max-w-6xl gap-8 px-6 sm:grid-cols-3">
          <div className="rounded-2xl border border-agave/10 bg-white/60 p-6">
            <h2 className="font-display text-xl font-semibold text-terracota">Tacos de autor</h2>
            <p className="mt-2 text-sm leading-relaxed text-ink-soft">
              Trompo, suadero y birria trabajados a fuego lento, servidos como en la calle de León.
            </p>
          </div>
          <div className="rounded-2xl border border-agave/10 bg-white/60 p-6">
            <h2 className="font-display text-xl font-semibold text-terracota">Cantina & coctelería</h2>
            <p className="mt-2 text-sm leading-relaxed text-ink-soft">
              Mezcal, tequila y cervezas artesanales para acompañar cada plato hasta la madrugada.
            </p>
          </div>
          <div className="rounded-2xl border border-agave/10 bg-white/60 p-6">
            <h2 className="font-display text-xl font-semibold text-terracota">Ambiente de antro</h2>
            <p className="mt-2 text-sm leading-relaxed text-ink-soft">
              Luz cálida, música en vivo los fines de semana y mesas para compartir.
            </p>
          </div>
        </div>
      </section>

      <section className="py-20">
        <div className="mx-auto flex max-w-6xl flex-col items-center gap-4 px-6 text-center">
          <h2 className="font-display text-3xl font-semibold text-agave">¿Antojo de algo puntual?</h2>
          <p className="max-w-xl text-ink-soft">
            Si ya estás en tu mesa, revisa el menú completo y arma tu pedido desde el celular.
          </p>
          <Link href="/menu" className="mt-2 text-sm font-semibold text-terracota hover:underline">
            Ver el menú completo →
          </Link>
        </div>
      </section>
    </div>
  );
}
