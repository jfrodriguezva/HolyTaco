using HolyTac.Reservations.Domain;

namespace HolyTac.Reservations.Application;

/// <summary>
/// Calcula horarios disponibles para mostrarlos al cliente. Es solo informativo: la disponibilidad
/// real y definitiva se re-verifica de forma atómica en <see cref="IReservationRepository.CreateAsync"/>
/// al momento de crear la reservación, para evitar condiciones de carrera entre esta consulta y el envío
/// del formulario.
/// </summary>
public class AvailabilityService(ITableRepository tableRepository, IReservationRepository reservationRepository)
{
    public async Task<IReadOnlyList<TimeSpan>> GetAvailableSlotsAsync(
        DateOnly date, int partySize, CancellationToken cancellationToken = default)
    {
        var tables = await tableRepository.GetAllAsync(cancellationToken);
        var reservationsThatDay = (await reservationRepository.GetByDateAsync(date, cancellationToken))
            .Where(r => r.IsActive)
            .ToList();

        var candidateTables = tables.Where(t => t.Capacity >= partySize).ToList();
        if (candidateTables.Count == 0) return [];

        var available = new List<TimeSpan>();
        foreach (var slot in BusinessHours.GetSlotsForDay())
        {
            var slotUtc = date.ToDateTime(TimeOnly.FromTimeSpan(slot), DateTimeKind.Utc);
            var hasFreeTable = candidateTables.Any(t => reservationsThatDay
                .Where(r => r.TableNumber == t.Number)
                .All(r => !r.OverlapsWith(slotUtc)));

            if (hasFreeTable) available.Add(slot);
        }

        return available;
    }
}
