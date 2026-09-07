using Dapper;
using HolyTac.Reservations.Application;
using HolyTac.Reservations.Domain;
using HolyTac.SharedKernel;

namespace HolyTac.Reservations.Infrastructure;

public class TableRepository(IDbConnectionFactory connectionFactory) : ITableRepository
{
    public async Task<IReadOnlyList<Table>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            var rows = await connection.QueryAsync<TableRow>(
                "SELECT Id, Number, Capacity, Zone FROM Tables ORDER BY Capacity, Number");
            return (IReadOnlyList<Table>)rows.Select(r => r.ToDomain()).ToList();
        }, cancellationToken);
    }
}
