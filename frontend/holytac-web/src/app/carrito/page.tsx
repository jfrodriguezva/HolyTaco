"use client";

import { useState } from "react";
import Link from "next/link";
import { useCart } from "@/lib/cart-context";
import { ordersApi } from "@/lib/api";

export default function CarritoPage() {
  const { lines, updateQuantity, removeItem, clear, total } = useCart();
  const [tableNumber, setTableNumber] = useState("");
  const [customerName, setCustomerName] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [confirmedOrderId, setConfirmedOrderId] = useState<string | null>(null);

  const handleSubmit = async () => {
    setError(null);

    const table = Number(tableNumber);
    if (!table || table <= 0) {
      setError("Indica un número de mesa válido.");
      return;
    }
    if (lines.length === 0) {
      setError("Tu carrito está vacío.");
      return;
    }

    setSubmitting(true);
    try {
      const orderId = await ordersApi.create({
        tableNumber: table,
        customerName: customerName || null,
        items: lines.map((l) => ({ menuItemId: l.menuItemId, quantity: l.quantity, notes: l.notes ?? null })),
      });
      setConfirmedOrderId(orderId);
      clear();
    } catch {
      setError("No pudimos enviar tu pedido. Intenta de nuevo en unos segundos.");
    } finally {
      setSubmitting(false);
    }
  };

  if (confirmedOrderId) {
    return (
      <div className="mx-auto max-w-xl px-6 py-24 text-center">
        <h1 className="font-display text-3xl font-semibold text-agave">¡Pedido enviado!</h1>
        <p className="mt-3 text-ink-soft">
          Tu orden fue enviada a cocina y barra. Folio: <span className="font-mono text-terracota">{confirmedOrderId}</span>
        </p>
        <Link
          href="/menu"
          className="mt-8 inline-block rounded-full bg-terracota px-7 py-3 text-sm font-semibold uppercase tracking-wide text-cream transition-colors hover:bg-terracota-dark"
        >
          Pedir algo más
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-3xl px-6 py-14">
      <h1 className="font-display mb-8 text-4xl font-semibold text-agave">Tu Carrito</h1>

      {lines.length === 0 ? (
        <div className="rounded-2xl border border-agave/10 bg-white/50 p-10 text-center text-ink-soft">
          Aún no agregas nada.{" "}
          <Link href="/menu" className="font-semibold text-terracota">
            Ir al menú
          </Link>
        </div>
      ) : (
        <div className="space-y-4">
          {lines.map((line) => (
            <div
              key={line.menuItemId}
              className="flex items-center justify-between gap-4 rounded-2xl border border-agave/10 bg-white/50 p-4"
            >
              <div>
                <p className="font-display font-semibold text-ink">{line.name}</p>
                <p className="text-sm text-ink-soft">${line.unitPrice.toFixed(2)} c/u</p>
              </div>

              <div className="flex items-center gap-3">
                <div className="flex items-center rounded-full border border-agave/20">
                  <button
                    onClick={() => updateQuantity(line.menuItemId, line.quantity - 1)}
                    className="px-3 py-1 text-ink-soft hover:text-terracota"
                    aria-label="Disminuir cantidad"
                  >
                    −
                  </button>
                  <span className="w-6 text-center text-sm font-semibold">{line.quantity}</span>
                  <button
                    onClick={() => updateQuantity(line.menuItemId, line.quantity + 1)}
                    className="px-3 py-1 text-ink-soft hover:text-terracota"
                    aria-label="Aumentar cantidad"
                  >
                    +
                  </button>
                </div>
                <span className="w-20 text-right font-semibold text-terracota">
                  ${(line.unitPrice * line.quantity).toFixed(2)}
                </span>
                <button
                  onClick={() => removeItem(line.menuItemId)}
                  className="text-ink-soft hover:text-terracota-dark"
                  aria-label="Quitar producto"
                >
                  ✕
                </button>
              </div>
            </div>
          ))}

          <div className="flex items-center justify-between border-t border-agave/15 pt-4 text-lg">
            <span className="font-semibold text-ink">Total</span>
            <span className="font-display font-semibold text-terracota">${total.toFixed(2)}</span>
          </div>

          <div className="mt-8 grid gap-4 rounded-2xl border border-agave/10 bg-white/50 p-6 sm:grid-cols-2">
            <label className="flex flex-col gap-1 text-sm text-ink-soft">
              Número de mesa
              <input
                type="number"
                min={1}
                value={tableNumber}
                onChange={(e) => setTableNumber(e.target.value)}
                className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
                placeholder="Ej. 12"
              />
            </label>
            <label className="flex flex-col gap-1 text-sm text-ink-soft">
              Nombre (opcional)
              <input
                type="text"
                value={customerName}
                onChange={(e) => setCustomerName(e.target.value)}
                className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
                placeholder="¿Cómo te llamamos?"
              />
            </label>
          </div>

          {error && <p className="text-sm font-medium text-terracota-dark">{error}</p>}

          <button
            onClick={handleSubmit}
            disabled={submitting}
            className="w-full rounded-full bg-agave py-3 text-sm font-semibold uppercase tracking-wide text-cream transition-colors hover:bg-agave-dark disabled:opacity-60"
          >
            {submitting ? "Enviando pedido…" : "Enviar pedido a cocina y barra"}
          </button>
        </div>
      )}
    </div>
  );
}
