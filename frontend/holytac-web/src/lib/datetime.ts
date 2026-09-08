/**
 * El backend serializa DateTime sin sufijo "Z" (Kind=Unspecified), pero todo el sistema trata esos
 * valores como el horario "de pared" del restaurante (sin conversión real de zona horaria — ver
 * ROADMAP.md). Sin forzar UTC aquí, `new Date(...)` los interpretaría como hora LOCAL del navegador,
 * desfasando la hora mostrada. Se fuerza "Z" y se muestran siempre con timeZone: "UTC".
 */
export function parseApiDate(isoString: string): Date {
  return new Date(isoString.endsWith("Z") ? isoString : `${isoString}Z`);
}

export function formatTime(isoString: string): string {
  return parseApiDate(isoString).toISOString().slice(11, 16);
}

export function formatLongDate(isoString: string): string {
  return parseApiDate(isoString).toLocaleDateString("es-MX", {
    weekday: "long",
    day: "numeric",
    month: "long",
    timeZone: "UTC",
  });
}

export function formatShortDate(isoString: string): string {
  return parseApiDate(isoString).toLocaleDateString("es-MX", {
    day: "numeric",
    month: "long",
    timeZone: "UTC",
  });
}

function toIsoDate(d: Date): string {
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, "0");
  const day = String(d.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

/**
 * Fecha de calendario "de hoy" según la hora LOCAL del navegador (no `toISOString()`, que usa UTC:
 * como León/CDMX es UTC-6, entre las 18:00 y la medianoche locales `toISOString()` ya reporta el día
 * siguiente, rompiendo los selectores de fecha justo en horas pico de cena).
 */
export function todayIso(): string {
  return toIsoDate(new Date());
}

/** Igual que {@link todayIso} pero desplazada `days` días hacia adelante. */
export function addDaysIso(days: number): string {
  const d = new Date();
  d.setDate(d.getDate() + days);
  return toIsoDate(d);
}
