"use client";

import { useEffect, useState, useCallback } from "react";
import { menuApi, promotionsApi, type DiscountType, type MenuItemDto, type PromotionDto } from "@/lib/api";
import { addDaysIso, todayIso } from "@/lib/datetime";
import { useAuth } from "@/lib/auth-context";
import { RequireStaffAuth } from "@/components/require-staff-auth";

function AdminPromocionesContent() {
  const { session } = useAuth();
  const [promotions, setPromotions] = useState<PromotionDto[]>([]);
  const [menuItems, setMenuItems] = useState<MenuItemDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [actioningId, setActioningId] = useState<string | null>(null);

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [discountType, setDiscountType] = useState<DiscountType>("Percentage");
  const [discountValue, setDiscountValue] = useState("20");
  const [comboPrice, setComboPrice] = useState("149");
  const [selectedItems, setSelectedItems] = useState<string[]>([]);
  const [startsAt, setStartsAt] = useState(todayIso());
  const [endsAt, setEndsAt] = useState(addDaysIso(30));
  const [isFeatured, setIsFeatured] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  const load = useCallback(() => {
    setLoading(true);
    setError(null);
    Promise.all([promotionsApi.getAll(), menuApi.getAll()])
      .then(([promos, items]) => {
        setPromotions(promos);
        setMenuItems(items);
      })
      .catch(() => setError("No pudimos cargar las promociones o el menú."))
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  const handleDeactivate = async (id: string) => {
    if (!session) return;
    setActioningId(id);
    try {
      await promotionsApi.deactivate(id, session.token);
      load();
    } catch {
      setError("No pudimos desactivar la promoción.");
    } finally {
      setActioningId(null);
    }
  };

  const handleCreate = async () => {
    if (!session) return;
    setFormError(null);
    if (!title.trim()) {
      setFormError("El título es obligatorio.");
      return;
    }

    setSubmitting(true);
    try {
      await promotionsApi.create(
        {
          title,
          description,
          discountType,
          discountValue: discountType === "ComboPrice" ? null : Number(discountValue),
          comboPrice: discountType === "ComboPrice" ? Number(comboPrice) : null,
          menuItemIds: selectedItems,
          startsAtUtc: `${startsAt}T00:00:00Z`,
          endsAtUtc: `${endsAt}T23:59:59Z`,
          isFeatured,
        },
        session.token
      );
      setTitle("");
      setDescription("");
      setSelectedItems([]);
      load();
    } catch (err) {
      setFormError(err instanceof Error ? err.message : "No pudimos crear la promoción.");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="mx-auto max-w-5xl px-6 py-14">
      <header className="mb-8">
        <span className="text-xs font-semibold uppercase tracking-[0.2em] text-terracota">Staff</span>
        <h1 className="font-display text-3xl font-semibold text-agave">Promociones</h1>
      </header>

      <div className="mb-10 space-y-4 rounded-2xl border border-agave/10 bg-white/50 p-6">
        <h2 className="font-display text-lg font-semibold text-ink">Nueva promoción</h2>

        <div className="grid gap-4 sm:grid-cols-2">
          <label className="flex flex-col gap-1 text-sm text-ink-soft">
            Título
            <input
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
              placeholder="Ej. Happy Hour Coctelería"
            />
          </label>
          <label className="flex flex-col gap-1 text-sm text-ink-soft">
            Tipo de descuento
            <select
              value={discountType}
              onChange={(e) => setDiscountType(e.target.value as DiscountType)}
              className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
            >
              <option value="Percentage">Porcentaje</option>
              <option value="FixedAmount">Monto fijo</option>
              <option value="ComboPrice">Precio de combo</option>
            </select>
          </label>
        </div>

        <label className="flex flex-col gap-1 text-sm text-ink-soft">
          Descripción
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            rows={2}
            className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
          />
        </label>

        <div className="grid gap-4 sm:grid-cols-3">
          {discountType === "ComboPrice" ? (
            <label className="flex flex-col gap-1 text-sm text-ink-soft">
              Precio del combo
              <input
                type="number"
                value={comboPrice}
                onChange={(e) => setComboPrice(e.target.value)}
                className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
              />
            </label>
          ) : (
            <label className="flex flex-col gap-1 text-sm text-ink-soft">
              Valor del descuento
              <input
                type="number"
                value={discountValue}
                onChange={(e) => setDiscountValue(e.target.value)}
                className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
              />
            </label>
          )}
          <label className="flex flex-col gap-1 text-sm text-ink-soft">
            Desde
            <input
              type="date"
              value={startsAt}
              onChange={(e) => setStartsAt(e.target.value)}
              className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
            />
          </label>
          <label className="flex flex-col gap-1 text-sm text-ink-soft">
            Hasta
            <input
              type="date"
              value={endsAt}
              onChange={(e) => setEndsAt(e.target.value)}
              className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
            />
          </label>
        </div>

        <label className="flex flex-col gap-1 text-sm text-ink-soft">
          Productos incluidos
          <select
            multiple
            value={selectedItems}
            onChange={(e) => setSelectedItems(Array.from(e.target.selectedOptions, (o) => o.value))}
            className="h-32 rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
          >
            {menuItems.map((item) => (
              <option key={item.id} value={item.id}>
                {item.name}
              </option>
            ))}
          </select>
        </label>

        <label className="flex items-center gap-2 text-sm text-ink-soft">
          <input type="checkbox" checked={isFeatured} onChange={(e) => setIsFeatured(e.target.checked)} />
          Destacar en el home
        </label>

        {formError && <p className="text-sm font-medium text-terracota-dark">{formError}</p>}

        <button
          onClick={handleCreate}
          disabled={submitting}
          className="rounded-full bg-terracota px-6 py-2.5 text-sm font-semibold uppercase tracking-wide text-cream transition-colors hover:bg-terracota-dark disabled:opacity-60"
        >
          {submitting ? "Creando…" : "Crear promoción"}
        </button>
      </div>

      {loading && <p className="text-ink-soft">Cargando…</p>}
      {error && <p className="text-terracota-dark">{error}</p>}

      {!loading && !error && (
        <div className="space-y-3">
          {promotions.map((promo) => (
            <div
              key={promo.id}
              className="flex items-center justify-between gap-4 rounded-2xl border border-agave/10 bg-white/50 p-4"
            >
              <div>
                <p className="font-display font-semibold text-ink">
                  {promo.title}{" "}
                  {!promo.isActive && (
                    <span className="ml-2 rounded-full bg-ink-soft/15 px-2 py-0.5 text-xs font-semibold text-ink-soft">
                      Inactiva
                    </span>
                  )}
                  {promo.isActive && !promo.isCurrentlyValid && (
                    <span className="ml-2 rounded-full bg-chile/15 px-2 py-0.5 text-xs font-semibold text-chile">
                      Fuera de vigencia
                    </span>
                  )}
                </p>
                <p className="text-sm text-ink-soft">{promo.description}</p>
              </div>
              {promo.isActive && (
                <button
                  onClick={() => handleDeactivate(promo.id)}
                  disabled={actioningId === promo.id}
                  className="rounded-full border border-terracota px-3 py-1 text-xs font-semibold text-terracota hover:bg-terracota hover:text-cream disabled:opacity-50"
                >
                  Desactivar
                </button>
              )}
            </div>
          ))}
          {promotions.length === 0 && <p className="text-ink-soft">Aún no hay promociones.</p>}
        </div>
      )}
    </div>
  );
}

export default function AdminPromocionesPage() {
  return (
    <RequireStaffAuth requiredRole="Gerente">
      <AdminPromocionesContent />
    </RequireStaffAuth>
  );
}
