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

    public async Task<Reservation> CreateAsync(
        string customerName,
        string phone,
        string? email,
        int partySize,
        DateTime reservationAtUtc,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        return await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            // Serializa la creación de reservaciones para este día: mientras esta transacción no
            // termine, cualquier otra petición para la misma fecha espera aquí antes de leer
            // disponibilidad, evitando que dos reservaciones concurrentes elijan la misma mesa.
            var lockKey = $"HolyTac:Reservations:{DateOnly.FromDateTime(reservationAtUtc):yyyyMMdd}";
            await connection.ExecuteAsync(
                """
                DECLARE @lockResult INT;
                EXEC @lockResult = sp_getapplock @Resource = @LockKey, @LockMode = 'Exclusive',
                    @LockOwner = 'Transaction', @LockTimeout = 10000;
                IF @lockResult < 0
                    THROW 51000, 'No se pudo bloquear el horario de reservación; intenta de nuevo.', 1;
                """,
                new { LockKey = lockKey },
                transaction);

            var start = DateOnly.FromDateTime(reservationAtUtc).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var end = start.AddDays(1);

            var candidateTables = await connection.QueryAsync<TableRow>(
                "SELECT Id, Number, Capacity, Zone FROM Tables WHERE Capacity >= @PartySize ORDER BY Capacity, Number",
                new { PartySize = partySize },
                transaction);

            var activeReservationsThatDay = (await connection.QueryAsync<ReservationRow>(
                """
                SELECT Id, CustomerName, Phone, Email, PartySize, ReservationAtUtc, TableNumber, Status, Notes, CreatedAtUtc
                FROM Reservations
                WHERE ReservationAtUtc >= @Start AND ReservationAtUtc < @End AND Status IN (1, 2)
                """,
                new { Start = start, End = end },
                transaction))
                .Select(r => r.ToDomain())
                .ToList();

            var table = candidateTables
                .Select(t => t.ToDomain())
                .FirstOrDefault(t => activeReservationsThatDay
                    .Where(r => r.TableNumber == t.Number)
                    .All(r => !r.OverlapsWith(reservationAtUtc)));

            if (table is null)
                throw new InvalidOperationException("No hay mesas disponibles para ese horario y tamaño de grupo.");

            var reservation = Reservation.Create(customerName, phone, email, partySize, reservationAtUtc, table.Number, notes);

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
                },
                transaction);

            transaction.Commit();
            return reservation;
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
