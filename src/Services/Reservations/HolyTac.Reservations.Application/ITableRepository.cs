using HolyTac.Reservations.Domain;

namespace HolyTac.Reservations.Application;

public interface ITableRepository
{
    Task<IReadOnlyList<Table>> GetAllAsync(CancellationToken cancellationToken = default);
}
