using HolyTac.Reservations.Domain;

namespace HolyTac.Reservations.Application;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reservation>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Reservation reservation, CancellationToken cancellationToken = default);
}
