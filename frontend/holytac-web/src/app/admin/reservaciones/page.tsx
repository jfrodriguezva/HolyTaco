"use client";

import { useEffect, useState, useCallback } from "react";
import { reservationsApi, type ReservationDto } from "@/lib/api";
import { formatTime } from "@/lib/datetime";

function todayIso(): string {
  return new Date().toISOString().slice(0, 10);
}

const statusStyles: Record<string, string> = {
  Pendiente: "bg-chile/15 text-chile",
  Confirmada: "bg-agave/15 text-agave",
  Cancelada: "bg-terracota/15 text-terracota-dark",
  Completada: "bg-ink-soft/15 text-ink-soft",
};

export default function AdminReservacionesPage() {
  const [date, setDate] = useState(todayIso());
  const [reservations, setReservations] = useState<ReservationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [actioningId, setActioningId] = useState<string | null>(null);

  const load = useCallback(() => {
    setLoading(true);
    setError(null);
    reservationsApi
      .getByDate(date)
      .then(setReservations)
      .catch(() => setError("No pudimos cargar las reservaciones."))
      .finally(() => setLoading(false));
  }, [date]);

  useEffect(() => {
    load();
  }, [load]);

  const handleConfirm = async (id: string) => {
    setActioningId(id);
    try {
      await reservationsApi.confirm(id);
      load();
    } catch {
      setError("No pudimos confirmar la reservación.");
    } finally {
      setActioningId(null);
    }
  };

  const handleCancel = async (id: string) => {
    setActioningId(id);
    try {
      await reservationsApi.cancel(id);
      load();
    } catch {
      setError("No pudimos cancelar la reservación.");
    } finally {
      setActioningId(null);
    }
  };

  return (
    <div className="mx-auto max-w-5xl px-6 py-14">
      <header className="mb-8 flex flex-wrap items-center justify-between gap-4">
        <div>
          <span className="text-xs font-semibold uppercase tracking-[0.2em] text-terracota">Staff</span>
          <h1 className="font-display text-3xl font-semibold text-agave">Reservaciones</h1>
        </div>
        <input
          type="date"
          value={date}
          onChange={(e) => setDate(e.target.value)}
          className="rounded-lg border border-agave/20 bg-white/60 px-3 py-2 text-ink outline-none focus:border-terracota"
        />
      </header>

      {loading && <p className="text-ink-soft">Cargando…</p>}
      {error && <p className="text-terracota-dark">{error}</p>}

      {!loading && !error && (
        <div className="overflow-x-auto rounded-2xl border border-agave/10 bg-white/50">
          <table className="w-full min-w-[720px] text-left text-sm">
            <thead className="border-b border-agave/10 text-xs uppercase tracking-wide text-ink-soft">
              <tr>
                <th className="px-4 py-3">Hora</th>
                <th className="px-4 py-3">Cliente</th>
                <th className="px-4 py-3">Personas</th>
                <th className="px-4 py-3">Mesa</th>
                <th className="px-4 py-3">Estatus</th>
                <th className="px-4 py-3">Notas</th>
                <th className="px-4 py-3">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {reservations.map((r) => {
                const time = formatTime(r.reservationAtUtc);
                return (
                  <tr key={r.id} className="border-b border-agave/5 last:border-0">
                    <td className="px-4 py-3 font-medium text-ink">{time}</td>
                    <td className="px-4 py-3">
                      {r.customerName}
                      <div className="text-xs text-ink-soft">{r.phone}</div>
                    </td>
                    <td className="px-4 py-3">{r.partySize}</td>
                    <td className="px-4 py-3">#{r.tableNumber}</td>
                    <td className="px-4 py-3">
                      <span className={`rounded-full px-2 py-1 text-xs font-semibold ${statusStyles[r.status] ?? ""}`}>
                        {r.status}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-ink-soft">{r.notes ?? "—"}</td>
                    <td className="px-4 py-3">
                      <div className="flex gap-2">
                        {r.status === "Pendiente" && (
                          <button
                            onClick={() => handleConfirm(r.id)}
                            disabled={actioningId === r.id}
                            className="rounded-full bg-agave px-3 py-1 text-xs font-semibold text-cream hover:bg-agave-dark disabled:opacity-50"
                          >
                            Confirmar
                          </button>
                        )}
                        {(r.status === "Pendiente" || r.status === "Confirmada") && (
                          <button
                            onClick={() => handleCancel(r.id)}
                            disabled={actioningId === r.id}
                            className="rounded-full border border-terracota px-3 py-1 text-xs font-semibold text-terracota hover:bg-terracota hover:text-cream disabled:opacity-50"
                          >
                            Cancelar
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                );
              })}
              {reservations.length === 0 && (
                <tr>
                  <td colSpan={7} className="px-4 py-8 text-center text-ink-soft">
                    No hay reservaciones para esta fecha.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
