"use client";

import { useEffect, useState } from "react";
import { reservationsApi, type ReservationDto } from "@/lib/api";
import { formatLongDate, formatTime, todayIso } from "@/lib/datetime";

export default function ReservarPage() {
  const [date, setDate] = useState(todayIso());
  const [partySize, setPartySize] = useState(2);
  const [slots, setSlots] = useState<string[]>([]);
  const [selectedSlot, setSelectedSlot] = useState<string | null>(null);
  const [loadingSlots, setLoadingSlots] = useState(true);

  const [customerName, setCustomerName] = useState("");
  const [phone, setPhone] = useState("");
  const [email, setEmail] = useState("");
  const [notes, setNotes] = useState("");

  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [confirmed, setConfirmed] = useState<ReservationDto | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoadingSlots(true);
    setSelectedSlot(null);
    reservationsApi
      .getAvailability(date, partySize)
      .then((data) => {
        if (!cancelled) setSlots(data);
      })
      .catch(() => {
        if (!cancelled) setSlots([]);
      })
      .finally(() => {
        if (!cancelled) setLoadingSlots(false);
      });
    return () => {
      cancelled = true;
    };
  }, [date, partySize]);

  const handleSubmit = async () => {
    setError(null);

    if (!selectedSlot) {
      setError("Elige un horario disponible.");
      return;
    }
    if (!customerName.trim() || !phone.trim()) {
      setError("Indica tu nombre y teléfono.");
      return;
    }

    setSubmitting(true);
    try {
      const reservationAtUtc = `${date}T${selectedSlot}:00Z`;
      const id = await reservationsApi.create({
        customerName,
        phone,
        email: email || null,
        partySize,
        reservationAtUtc,
        notes: notes || null,
      });
      const reservation = await reservationsApi.getById(id);
      setConfirmed(reservation);
    } catch (err) {
      setError(err instanceof Error ? err.message : "No pudimos crear tu reservación.");
    } finally {
      setSubmitting(false);
    }
  };

  if (confirmed) {
    return (
      <div className="mx-auto max-w-xl px-6 py-24 text-center">
        <h1 className="font-display text-3xl font-semibold text-agave">¡Mesa reservada!</h1>
        <p className="mt-3 text-ink-soft">
          Te esperamos el{" "}
          <span className="font-semibold text-ink">{formatLongDate(confirmed.reservationAtUtc)}</span>{" "}
          a las <span className="font-semibold text-ink">{formatTime(confirmed.reservationAtUtc)}</span> — mesa{" "}
          <span className="font-semibold text-terracota">#{confirmed.tableNumber}</span> para{" "}
          {confirmed.partySize} personas.
        </p>
        <p className="mt-4 text-sm text-ink-soft">
          Folio: <span className="font-mono text-terracota">{confirmed.id}</span>
        </p>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-2xl px-6 py-14">
      <header className="mb-10 text-center">
        <span className="rounded-full bg-agave/10 px-4 py-1 text-xs font-semibold uppercase tracking-[0.2em] text-agave">
          Reserva tu mesa
        </span>
        <h1 className="font-display mt-3 text-4xl font-semibold text-agave">Aparta tu lugar</h1>
        <p className="mt-2 text-ink-soft">Elige fecha, horario y cuéntanos cuántos son.</p>
      </header>

      <div className="space-y-6 rounded-2xl border border-agave/10 bg-white/50 p-6">
        <div className="grid gap-4 sm:grid-cols-2">
          <label className="flex flex-col gap-1 text-sm text-ink-soft">
            Fecha
            <input
              type="date"
              min={todayIso()}
              value={date}
              onChange={(e) => setDate(e.target.value)}
              className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
            />
          </label>
          <label className="flex flex-col gap-1 text-sm text-ink-soft">
            Personas
            <input
              type="number"
              min={1}
              max={20}
              value={partySize}
              onChange={(e) => setPartySize(Number(e.target.value) || 1)}
              className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
            />
          </label>
        </div>

        <div>
          <p className="mb-2 text-sm text-ink-soft">Horario disponible</p>
          {loadingSlots ? (
            <p className="text-sm text-ink-soft">Buscando horarios…</p>
          ) : slots.length === 0 ? (
            <p className="text-sm text-terracota-dark">No hay horarios disponibles para esa fecha y tamaño de grupo.</p>
          ) : (
            <div className="flex flex-wrap gap-2">
              {slots.map((slot) => (
                <button
                  key={slot}
                  onClick={() => setSelectedSlot(slot)}
                  className={`rounded-full border px-4 py-1.5 text-sm font-medium transition-colors ${
                    selectedSlot === slot
                      ? "border-terracota bg-terracota text-cream"
                      : "border-agave/20 bg-cream text-ink-soft hover:border-terracota/50"
                  }`}
                >
                  {slot}
                </button>
              ))}
            </div>
          )}
        </div>

        <div className="grid gap-4 sm:grid-cols-2">
          <label className="flex flex-col gap-1 text-sm text-ink-soft">
            Nombre
            <input
              type="text"
              value={customerName}
              onChange={(e) => setCustomerName(e.target.value)}
              className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
              placeholder="¿Cómo te llamas?"
            />
          </label>
          <label className="flex flex-col gap-1 text-sm text-ink-soft">
            Teléfono
            <input
              type="tel"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
              placeholder="10 dígitos"
            />
          </label>
          <label className="flex flex-col gap-1 text-sm text-ink-soft">
            Email (opcional)
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
              placeholder="tu@correo.com"
            />
          </label>
          <label className="flex flex-col gap-1 text-sm text-ink-soft">
            Notas (opcional)
            <input
              type="text"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
              placeholder="Cumpleaños, alergias, etc."
            />
          </label>
        </div>

        {error && <p className="text-sm font-medium text-terracota-dark">{error}</p>}

        <button
          onClick={handleSubmit}
          disabled={submitting}
          className="w-full rounded-full bg-terracota py-3 text-sm font-semibold uppercase tracking-wide text-cream transition-colors hover:bg-terracota-dark disabled:opacity-60"
        >
          {submitting ? "Reservando…" : "Confirmar reservación"}
        </button>
      </div>
    </div>
  );
}
