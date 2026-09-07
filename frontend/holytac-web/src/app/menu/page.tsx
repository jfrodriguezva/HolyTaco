"use client";

import { useEffect, useMemo, useState } from "react";
import { menuApi, type MenuCategory, type MenuItemDto } from "@/lib/api";
import { MenuCard } from "@/components/menu-card";

const categories: { value: MenuCategory | "Todos"; label: string }[] = [
  { value: "Todos", label: "Todos" },
  { value: "Tacos", label: "Tacos" },
  { value: "Antojitos", label: "Antojitos" },
  { value: "Platillos", label: "Platillos" },
  { value: "Cocteleria", label: "Coctelería" },
  { value: "Cervezas", label: "Cervezas" },
  { value: "Bebidas", label: "Bebidas" },
  { value: "Postres", label: "Postres" },
];

export default function MenuPage() {
  const [items, setItems] = useState<MenuItemDto[]>([]);
  const [category, setCategory] = useState<(typeof categories)[number]["value"]>("Todos");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);

    menuApi
      .getAll()
      .then((data) => {
        if (!cancelled) setItems(data);
      })
      .catch(() => {
        if (!cancelled) setError("No pudimos cargar el menú. Verifica que el Gateway y los servicios estén corriendo.");
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => {
      cancelled = true;
    };
  }, []);

  const filtered = useMemo(
    () => (category === "Todos" ? items : items.filter((i) => i.category === category)),
    [items, category]
  );

  return (
    <div className="mx-auto max-w-6xl px-6 py-14">
      <header className="mb-10 text-center">
        <h1 className="font-display text-4xl font-semibold text-agave">Nuestro Menú</h1>
        <p className="mt-2 text-ink-soft">Tacos, antojitos, platillos fuertes y coctelería de la casa.</p>
      </header>

      <div className="mb-10 flex flex-wrap justify-center gap-2">
        {categories.map((c) => (
          <button
            key={c.value}
            onClick={() => setCategory(c.value)}
            className={`rounded-full border px-4 py-1.5 text-sm font-medium transition-colors ${
              category === c.value
                ? "border-terracota bg-terracota text-cream"
                : "border-agave/20 bg-white/50 text-ink-soft hover:border-terracota/50"
            }`}
          >
            {c.label}
          </button>
        ))}
      </div>

      {loading && <p className="text-center text-ink-soft">Cargando menú…</p>}
      {error && <p className="text-center text-terracota-dark">{error}</p>}

      {!loading && !error && (
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {filtered.map((item) => (
            <MenuCard key={item.id} item={item} />
          ))}
          {filtered.length === 0 && (
            <p className="col-span-full text-center text-ink-soft">No hay productos en esta categoría.</p>
          )}
        </div>
      )}
    </div>
  );
}
