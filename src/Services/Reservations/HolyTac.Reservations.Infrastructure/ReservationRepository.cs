using Dapper;
using HolyTac.Reservations.Application;
using HolyTac.Reservations.Domain;
using HolyTac.SharedKernel;

namespace HolyTac.Reservations.Infrastructure;

public class ReservationRepository(IDbConnectionFactory connectionFactory) : IReservationRepository
{
    public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            var row = await connection.QuerySingleOrDefaultAsync<ReservationRow>(
                """
                SELECT Id, CustomerName, Phone, Email, PartySize, ReservationAtUtc, TableNumber, Status, Notes, CreatedAtUtc
                FROM Reservations WHERE Id = @Id
                """,
                new { Id = id });
            return row?.ToDomain();
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<Reservation>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        return await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var end = start.AddDays(1);

            var rows = await connection.QueryAsync<ReservationRow>(
                """
                SELECT Id, CustomerName, Phone, Email, PartySize, ReservationAtUtc, TableNumber, Status, Notes, CreatedAtUtc
                FROM Reservations
                WHERE ReservationAtUtc >= @Start AND ReservationAtUtc < @End
                """,
                new { Start = start, End = end });

            return (IReadOnlyList<Reservation>)rows.Select(r => r.ToDomain()).ToList();
        }, cancellationToken);
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                """
                INSERT INTO Reservations
                    (Id, CustomerName, Phone, Email, PartySize, ReservationAtUtc, TableNumber, Status, Notes, CreatedAtUtc)
                VALUES
                    (@Id, @CustomerName, @Phone, @Email, @PartySize, @ReservationAtUtc, @TableNumber, @Status, @Notes, @CreatedAtUtc)
                """,
                new
                {
                    reservation.Id,
                    reservation.CustomerName,
                    reservation.Phone,
                    reservation.Email,
                    reservation.PartySize,
                    reservation.ReservationAtUtc,
                    reservation.TableNumber,
                    Status = (int)reservation.Status,
                    reservation.Notes,
                    reservation.CreatedAtUtc
                });
        }, cancellationToken);
    }

    public async Task UpdateStatusAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                "UPDATE Reservations SET Status = @Status WHERE Id = @Id",
                new { reservation.Id, Status = (int)reservation.Status });
        }, cancellationToken);
    }
}
