namespace HolyTac.Reservations.Domain;

/// <summary>Horario fijo de operación usado para calcular los horarios de reservación disponibles.</summary>
public static class BusinessHours
{
    public static readonly TimeSpan OpensAt = new(13, 0, 0);
    public static readonly TimeSpan ClosesAt = new(23, 0, 0);
    public const int SlotIntervalMinutes = 30;
    public const int ReservationDurationMinutes = 90;

    /// <summary>Genera los horarios del día que dejan tiempo suficiente para una reservación antes del cierre.</summary>
    public static IEnumerable<TimeSpan> GetSlotsForDay()
    {
        var lastPossibleSlot = ClosesAt - TimeSpan.FromMinutes(ReservationDurationMinutes);
        for (var slot = OpensAt; slot <= lastPossibleSlot; slot += TimeSpan.FromMinutes(SlotIntervalMinutes))
        {
            yield return slot;
        }
    }
}
