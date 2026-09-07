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
