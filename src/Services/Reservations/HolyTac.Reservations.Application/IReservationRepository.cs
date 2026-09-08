using HolyTac.Reservations.Domain;

namespace HolyTac.Reservations.Application;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reservation>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca una mesa disponible y crea la reservación de forma atómica (lock exclusivo por fecha +
    /// misma transacción), evitando que dos reservaciones concurrentes para el mismo horario elijan
    /// la misma mesa antes de que cualquiera de las dos se inserte.
    /// </summary>
    Task<Reservation> CreateAsync(
        string customerName,
        string phone,
        string? email,
        int partySize,
        DateTime reservationAtUtc,
        string? notes,
        CancellationToken cancellationToken = default);

    Task UpdateStatusAsync(Reservation reservation, CancellationToken cancellationToken = default);
}
