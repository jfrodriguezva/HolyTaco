using HolyTac.Reservations.Domain;

namespace HolyTac.Reservations.Application;

/// <summary>Calcula qué mesa (si alguna) está libre para un horario y tamaño de grupo dados.</summary>
public class AvailabilityService(ITableRepository tableRepository, IReservationRepository reservationRepository)
{
    public async Task<Table?> FindAvailableTableAsync(
        DateTime reservationAtUtc, int partySize, CancellationToken cancellationToken = default)
    {
        var date = DateOnly.FromDateTime(reservationAtUtc);
        var tables = await tableRepository.GetAllAsync(cancellationToken);
        var reservationsThatDay = await reservationRepository.GetByDateAsync(date, cancellationToken);
        var activeReservations = reservationsThatDay.Where(r => r.IsActive).ToList();

        return tables
            .Where(t => t.Capacity >= partySize)
            .OrderBy(t => t.Capacity)
            .FirstOrDefault(t => activeReservations
                .Where(r => r.TableNumber == t.Number)
                .All(r => !r.OverlapsWith(reservationAtUtc)));
    }

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
